using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Digipolis.Web.Swagger
{
    internal class EndPointPathsAndParamsToLower : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var newPaths = new OpenApiPaths();
            foreach (var path in swaggerDoc.Paths)
            {
                var res = HandlePath(path.Value);
                newPaths.Add(path.Key.ToLowerInvariant(), res);
            }
            swaggerDoc.Paths = newPaths;
        }

        private OpenApiPathItem HandlePath(OpenApiPathItem value)
        {
            value.Parameters = handleParameters(value.Parameters);

            foreach (var operation in value.Operations)
                switch (operation.Key)
                {
                    case OperationType.Get:
                        value.Operations[OperationType.Get].Parameters = handleParameters(value.Operations[OperationType.Get].Parameters);
                        break;
                    case OperationType.Post:
                        value.Operations[OperationType.Post].Parameters = handleParameters(value.Operations[OperationType.Post].Parameters);
                        break;
                    case OperationType.Put:
                        value.Operations[OperationType.Put].Parameters = handleParameters(value.Operations[OperationType.Put].Parameters);
                        break;
                    case OperationType.Patch:
                        value.Operations[OperationType.Patch].Parameters = handleParameters(value.Operations[OperationType.Patch].Parameters);
                        break;
                    case OperationType.Delete:
                        value.Operations[OperationType.Delete].Parameters = handleParameters(value.Operations[OperationType.Delete].Parameters);
                        break;
                    case OperationType.Head:
                        value.Operations[OperationType.Head].Parameters = handleParameters(value.Operations[OperationType.Head].Parameters);
                        break;
                    case OperationType.Options:
                        value.Operations[OperationType.Options].Parameters = handleParameters(value.Operations[OperationType.Options].Parameters);
                        break;
                }
            return value;
        }

        private IList<OpenApiParameter> handleParameters(IList<OpenApiParameter> parameters)
        {
            if (parameters == null) return null;
            foreach (var item in parameters.Where(x => x.In.HasValue && new Collection<ParameterLocation> { ParameterLocation.Query, ParameterLocation.Path }.Contains(x.In.Value)))
                item.Name = item.Name?.ToLowerInvariant();
            return parameters;
        }
    }
}