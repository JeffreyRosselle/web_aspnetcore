using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace Digipolis.Web.Swagger
{
    internal class SetVersionInPaths : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Paths = swaggerDoc.Paths.ToDictionary(
                entry => entry.Key.Replace("{apiVersion}", swaggerDoc.Info.Version).Replace("{apiversion}", swaggerDoc.Info.Version),
                entry =>
                {
                    var pathItem = entry.Value;
                    RemoveVersionParamFrom(pathItem.Operations[OperationType.Get]);
                    RemoveVersionParamFrom(pathItem.Operations[OperationType.Put]);
                    RemoveVersionParamFrom(pathItem.Operations[OperationType.Post]);
                    RemoveVersionParamFrom(pathItem.Operations[OperationType.Delete]);
                    RemoveVersionParamFrom(pathItem.Operations[OperationType.Options]);
                    RemoveVersionParamFrom(pathItem.Operations[OperationType.Head]);
                    RemoveVersionParamFrom(pathItem.Operations[OperationType.Patch]);
                    return pathItem;
                });
        }

        private void RemoveVersionParamFrom(OpenApiOperation operation)
        {
            if (operation == null || operation.Parameters == null) return;

            var versionParam = operation.Parameters.FirstOrDefault(param => param.Name.Equals("apiVersion", StringComparison.CurrentCultureIgnoreCase));
            if (versionParam == null) return;

            operation.Parameters.Remove(versionParam);
        }
    }


}
