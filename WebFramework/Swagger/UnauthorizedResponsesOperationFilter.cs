using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace WebFramework.Swagger
{
    public class UnauthorizedResponsesOperationFilter : IOperationFilter
    {
        private readonly bool _includeUnauthorizedAndForbiddenResponses;
        private readonly string _schemeName;

        public UnauthorizedResponsesOperationFilter(bool includeUnauthorizedAndForbiddenResponses, string schemeName = "Bearer")
        {
            _includeUnauthorizedAndForbiddenResponses = includeUnauthorizedAndForbiddenResponses;
            _schemeName = schemeName;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filters = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var metadta = context.ApiDescription.ActionDescriptor.EndpointMetadata;

            var hasAnonymous = filters.Any(p => p.Filter is AllowAnonymousFilter) || metadta.Any(p => p is AllowAnonymousAttribute);
            if (hasAnonymous) return;

            var hasAuthorize = filters.Any(p => p.Filter is AuthorizeFilter) || metadta.Any(p => p is AuthorizeAttribute);
            if (!hasAuthorize) return;

            if (_includeUnauthorizedAndForbiddenResponses)
            {
                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
            }

            // Get Authorize attribute
            var attributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>().ToList();

            if (attributes?.Any()??false)
            {
                var attr = attributes[0];

                // Add what should be show inside the security section
                IList<string> securityInfos = new List<string>();
                securityInfos.Add($"{nameof(AuthorizeAttribute.Policy)}:{attr.Policy}");
                securityInfos.Add($"{nameof(AuthorizeAttribute.Roles)}:{attr.Roles}");
                securityInfos.Add($"{nameof(AuthorizeAttribute.AuthenticationSchemes)}:{attr.AuthenticationSchemes}");

                switch (attr.AuthenticationSchemes)
                {
                    case var p when p == AuthenticationScheme.Basic:
                        operation.Security.Add(new OpenApiSecurityRequirement()
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    //Scheme = AuthenticationScheme.Basic,
                                    Reference = new OpenApiReference
                                    {
                                        Id = "basic", // Must fit the defined Id of SecurityDefinition in global configuration
                                        Type = ReferenceType.SecurityScheme,
                                    }
                                },
                                securityInfos
                            }
                        });
                        break;
                    //case var p when p == AuthenticationScheme.Bearer:
                    default:
                        operation.Security.Add(new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Scheme = _schemeName,
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "OAuth2"
                                    }
                                },
                                Array.Empty<string>() //new[] { "readAccess", "writeAccess" }
                            }
                        });
                        break;
                }
            }
            else
            {
                //operation.Security.Clear();
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Scheme = _schemeName,
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "OAuth2"
                            }
                        },
                        Array.Empty<string>() //new[] { "readAccess", "writeAccess" }
                    }
                });
            }


            /*operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "basic"
                        }
                    },
                    Array.Empty<string>() //new[] { "readAccess", "writeAccess" }
                }
            });
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Scheme = _schemeName,
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "OAuth2"
                        }
                    },
                    Array.Empty<string>() //new[] { "readAccess", "writeAccess" }
                }
            });*/
        }
    }
    public static class AuthenticationScheme
    {
        /// <summary>
        /// Bearer-token authentication
        /// </summary>
        /// <remarks>Equals JwtBearerDefaults.AuthenticationScheme</remarks>
        public static string Bearer = "Bearer";

        /// <summary>
        /// Basic (Id/Pwd) authentication
        /// </summary>
        public static string Basic = "BasicAuthentication";
    }
}
