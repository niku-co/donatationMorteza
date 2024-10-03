using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using Mahak.Api.HostServices;
using Mahak.Api.Hubs;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using WebFramework.Configuration;
using WebFramework.CustomMapping;
using WebFramework.Middlewares;
using WebFramework.Swagger;
using Services.DynamicAuthorization;
using Services.DynamicAuthorization.Utilities;
using Services.Services;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);
//builder.Logging.AddFilter(DbLoggerCategory.Query.Name, LogLevel.Warning);
Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();
builder.Host.UseSerilog((ctx, lc) => lc
    //.WriteTo.Console()
    //.WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .ReadFrom.Configuration(ctx.Configuration));

var settingConfig = builder.Configuration.GetSection(nameof(SiteSettings));
builder.Services.Configure<SiteSettings>(settingConfig);
var siteSettings = settingConfig.Get<SiteSettings>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHostedService<CheckErrorService>(); 
//builder.Services.AddHostedService<CheckDiscounnectedService>(); 
builder.Services.AddSignalR();
builder.Services.InitializeAutoMapper();
builder.Services.AddDbContext(builder.Configuration);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(ConfigureContainer));
builder.Services.AddCustomIdentity(siteSettings.IdentitySettings);
builder.Services.AddMinimalMvc();
builder.Services.AddJwtAuthentication(siteSettings.JwtSettings);
builder.Services.AddCustomApiVersioning();
builder.Services.AddSwagger();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddSingleton<IApiUtility, ApiUtility>();
builder.Services.AddScoped<SmsService>();
builder.Services.AddSingleton<IAuthorizationHandler, DynamicAuthorizeHandler>();
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy(UserConstant.DynamicAuthorizationPolicy, policy => policy.AddRequirements(new DynamicAuthorizeRequirement()));
});

var app = builder.Build();

//// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging();
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
    {
        context.Request.Path = "/index.html";
        context.Response.StatusCode = 200;
        await next();
    }
});

app.UseCustomExceptionHandler();


app.UseStaticFiles();
FileExtensionContentTypeProvider contentTypes = new FileExtensionContentTypeProvider();
contentTypes.Mappings[".apk"] = "application/vnd.android.package-archive";
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = contentTypes
});
//var envDb = Environment.GetEnvironmentVariable("Connection_String");
//var envDb2 = Configuration["Logging:LogLevel"];

app.InitializeDatabaseAsync();
// Create a scope and get DbContext service here
//using (var scope = app.ApplicationServices.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    context.Database.Migrate(); // Call Database.Migrate() here
//}

app.UseSwaggerAndUI();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    //.WithOrigins("http://5.160.7.50:8008", "https://5.160.7.50:8008", "http://192.168.0.234:8008", "https://192.168.0.234:8008")
    //.WithMethods("GET", "POST", "PUT", "DELETE")
    .AllowAnyMethod()
    .AllowAnyHeader());
//.AllowCredentials());

//app.UseCors("mahak_policy");

app.UseRouting();

//Use this config just in Develoment (not in Production)
//app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chat");
app.MapDefaultControllerRoute();

app.Run();

static void ConfigureContainer(ContainerBuilder containerBuilder)
{
    // Register your Autofac modules or individual components here
    containerBuilder.AddServices();
}