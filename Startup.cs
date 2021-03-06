﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle;
using Swashbuckle.AspNetCore.Swagger;
using webapi_jwt_template.Common;

namespace webapi_jwt_template
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var key = Configuration["SecurityKey"]; // 你的加密key，可以配置文件里写，也可以另外找地方保存
            Logger.Info("SecurityKey {0}", key);

            //添加jwt验证：
            services.AddAuthentication (options => {
                //认证middleware配置
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer (options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = true, //是否验证Issuer
                ValidateAudience = true, //是否验证Audience
                ValidateLifetime = false, //是否验证失效时间
                ValidateIssuerSigningKey = true, //是否验证SecurityKey
                ValidAudience = "chsarptools.com", //Audience
                ValidIssuer = "chsarptools.com", //Issuer，这两项和前面签发jwt的设置一致
                IssuerSigningKey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (key)) //拿到SecurityKey
                };
            });

            services.AddSwaggerGen (c => {
                c.SwaggerDoc ("v1", new Info {
                    Title = "chsarptools",
                    Version = "v1",
                    Description = "chsarptools API ",
                });

                //处理复杂名称
                c.CustomSchemaIds ((type) => type.FullName);
                c.OperationFilter<AddAuthTokenHeaderParameter>();
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication ();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger (c => {
                //设置json路径
                c.RouteTemplate = "docs/{documentName}/swagger.json";
            });
            app.UseSwaggerUI (c => {
                //访问swagger UI的路由，如http://localhost:端口/docs
                c.RoutePrefix = "docs";
                c.SwaggerEndpoint ("/docs/v1/swagger.json", "Swagger测试V1");
                //更改UI样式
                c.InjectStylesheet ("/swagger-ui/custom.css");
                //引入UI变更js
                c.InjectOnCompleteJavaScript ("/swagger-ui/custom.js");
            });
        }
    }
}
