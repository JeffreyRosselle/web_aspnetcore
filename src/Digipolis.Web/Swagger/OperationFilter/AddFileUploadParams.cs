using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Digipolis.Web.Swagger
{
    public class AddFileUploadParams : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                return;

            var formFileParams = context.MethodInfo.GetParameters()
                .Where(x => x.ParameterType.IsAssignableFrom(typeof(IFormFile)))
                .Select(x => x.Name)
                .ToList();

            var formFileSubParams = context.MethodInfo.GetParameters()
                .SelectMany(x => x.ParameterType.GetProperties())
                .Where(x => x.PropertyType.IsAssignableFrom(typeof(IFormFile)))
                .Select(x => x.Name)
                .ToList();

            var allFileParamNames = formFileParams.Union(formFileSubParams).ToList();

            if (!allFileParamNames.Any())
                return;

            var paramsToRemove = new List<OpenApiParameter>();
            foreach (var param in operation.Parameters)
            {
                paramsToRemove.AddRange(from fileParamName in allFileParamNames
                                        where param.Name.StartsWith(fileParamName + ".")
                                        select param);
            }
            paramsToRemove.ForEach(x => operation.Parameters.Remove(x));
            foreach (var paramName in allFileParamNames)
            {
                var fileParam = new OpenApiParameter
                {
                    Style = ParameterStyle.Form,
                    Name = paramName,
                    Required = true
                };
                operation.Parameters.Add(fileParam);
            }
            //foreach (IOpenApiElement param in operation.Parameters.Where(x => x.In.HasValue && x.In.Equals("form", StringComparison.CurrentCultureIgnoreCase)))
            //{
            //    param.In = ParameterLocation.
            //}

            //operation.Consumes = new List<string>() { "multipart/form-data" };
        }
    }
}