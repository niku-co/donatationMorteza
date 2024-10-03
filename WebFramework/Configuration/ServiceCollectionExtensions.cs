﻿using Common;
using Common.Exceptions;
using Common.Utilities;
using Data;
using Data.Contracts;
using Entities.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace WebFramework.Configuration;

public static class ServiceCollectionExtensions
{
    public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlServer"),
                x => x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
            .ConfigureWarnings(builder => builder.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning));

            //Tips
            //Automatic client evaluation is no longer supported. This event is no longer generated.
            //This line is no longer needed.
            //.ConfigureWarnings(warning => warning.Throw(RelationalEventId.QueryClientEvaluationWarning));
        });
    }
    public static void AddMinimalMvc(this IServiceCollection services)
    {
        //https://github.com/aspnet/AspNetCore/blob/0303c9e90b5b48b309a78c2ec9911db1812e6bf3/src/Mvc/Mvc/src/MvcServiceCollectionExtensions.cs
        services.AddControllers(options =>
        {
            //options.Filters.Add(new AuthorizeFilter()); //Apply AuthorizeFilter as global filter to all actions

            //Like [ValidateAntiforgeryToken] attribute but dose not validatie for GET and HEAD http method
            //You can ingore validate by using [IgnoreAntiforgeryToken] attribute
            //Use this filter when use cookie 
            //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

            //options.UseYeKeModelBinder();
        }).AddNewtonsoftJson(option =>
        {
            option.SerializerSettings.Converters.Add(new StringEnumConverter());
            option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //option.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            //option.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        })
        .AddJsonOptions(options =>
        {
            // options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;//for camel case
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            //options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });
        services.AddSwaggerGenNewtonsoftSupport();
    }

    public static void AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
    {
        services.AddAuthentication(options =>
        {
            //options.AddScheme<BasicAuthenticationHandler>("BasicAuthentication", null);
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

        })
        .AddJwtBearer(options =>
        {
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
            var encryptionKey = Encoding.UTF8.GetBytes(jwtSettings.EncryptKey);

            var validationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero, // default: 5 min
                RequireSignedTokens = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ValidateAudience = true, //default : false
                ValidAudience = jwtSettings.Audience,

                ValidateIssuer = true, //default : false
                ValidIssuer = jwtSettings.Issuer,

                TokenDecryptionKey = new SymmetricSecurityKey(encryptionKey)
            };

            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = validationParameters;
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    //var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                    //logger.LogError("Authentication failed.", context.Exception);

                    if (context.Exception != null)
                        throw new AppException(ApiResultStatusCode.UnAuthorized, "Authentication failed.", HttpStatusCode.Unauthorized, context.Exception, null);

                    return Task.CompletedTask;
                },
                //OnTokenValidated = async context =>
                //{
                //    var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<User>>();
                //    var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

                //    var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                //    if (claimsIdentity.Claims?.Any() != true)
                //        context.Fail("This token has no claims.");

                //    var securityStamp = claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                //    if (!securityStamp.HasValue())
                //        context.Fail("This token has no security stamp");

                //    //Find user and token from database and perform your custom validation
                //    var userId = claimsIdentity.GetUserId<int>();
                //    var user = await userRepository.GetByIdAsync(context.HttpContext.RequestAborted, userId);

                //    //if (user.SecurityStamp != Guid.Parse(securityStamp))
                //    //    context.Fail("Token security stamp is not valid.");

                //    var validatedUser = await signInManager.ValidateSecurityStampAsync(context.Principal);
                //    if (validatedUser == null)
                //        context.Fail("Token security stamp is not valid.");

                //    if ((!user?.Active) ?? true)
                //        context.Fail("User is not active.");

                //    await userRepository.UpdateLastLoginDateAsync(user, context.HttpContext.RequestAborted);
                //},
                OnChallenge = context =>
                {
                    //var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                    //logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);

                    if (context.AuthenticateFailure != null)
                        throw new AppException(ApiResultStatusCode.UnAuthorized, "Authenticate failure.", HttpStatusCode.Unauthorized, context.AuthenticateFailure, null);
                    throw new AppException(ApiResultStatusCode.UnAuthorized, "You are unauthorized to access this resource.", HttpStatusCode.Unauthorized);

                    //return Task.CompletedTask;
                }
            };
        });
    }
    public static void AddCustomApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            //url segment => {version}
            options.AssumeDefaultVersionWhenUnspecified = true; //default => false;
            options.DefaultApiVersion = new ApiVersion(1, 0); //v1.0 == v1
            options.ReportApiVersions = true;

            //ApiVersion.TryParse("1.0", out var version10);
            //ApiVersion.TryParse("1", out var version1);
            //var a = version10 == version1;

            //options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
            // api/posts?api-version=1

            //options.ApiVersionReader = new UrlSegmentApiVersionReader();
            // api/v1/posts

            //options.ApiVersionReader = new HeaderApiVersionReader(new[] { "Api-Version" });
            // header => Api-Version : 1

            //options.ApiVersionReader = new MediaTypeApiVersionReader()

            //options.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version"), new UrlSegmentApiVersionReader())
            // combine of [querystring] & [urlsegment]
        });
    }
}
