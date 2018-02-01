using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace webapi_jwt_template.Common {
    public class AddAuthTokenHeaderParameter : IOperationFilter {
        public void Apply (Operation operation, OperationFilterContext context) {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter> ();

            // 网上的例子里有对接口类型做判断后再添加参数
            // 这里默认全加了，反正真正业务代码里也都是要验证权限的
            operation.Parameters.Add (new NonBodyParameter () {
                Name = "Authorization",
                    In = "header",
                    Type = "string",
                    Required = false
            });
        }
    }

}