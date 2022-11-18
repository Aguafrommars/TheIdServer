// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre

using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddApiSample(this WebApplicationBuilder webApplicationBuilder)
        {
            var services = webApplicationBuilder.Services;
            services.AddCors()
                .AddAuthorization()
                .AddControllers();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "https://localhost:5443";
                    options.Audience = "api1";
                });

            return webApplicationBuilder;
        }
    }
}