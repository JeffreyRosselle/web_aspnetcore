﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Digipolis.Web.Swagger
{
    // This filter is necessary to correct the $Ref in the swagger for response types that are generic types.
    public class ValidRefUri : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //If no responses, skip the operation.
            if (operation is null) return;
            //If there are Ref's in the response schema 
            foreach (var item in operation.Responses.Where(x => x.Value.Schema?.Ref != null && x.Value.Schema.Ref.Contains('[')))
            {
                CorrectResponse(item, context);
            }

            foreach (var item in context.SchemaRepository.Schemas.Where(x => x.Value?.Properties != null).ToList())
            {
                foreach (var property in item.Value.Properties.Where(x => x.Value.Ref != null && x.Value.Ref.Contains("[")))
                {
                    CorrectProperty(property, context);
                }
            }
        }
        private void CorrectResponse(KeyValuePair<string, OpenApiResponse> response, OperationFilterContext context)
        {
            context.SchemaGenerator.GenerateSchema(typeof(CustomType), context.SchemaRepository);
            var invalidRef = response.Value.Schema.Ref;
            var validRef = invalidRef.Replace("[", "_").Replace(",", "_").Replace("]", "");

            var schemaName = invalidRef.Split('/').LastOrDefault();
            var validSchemaName = validRef.Split('/').LastOrDefault();

            OpenApiSchema schema;
            if (context.SchemaRepository.Schemas.SelectMany(x => x.Value..Definitions.TryGetValue(schemaName, out schema)))
            {
                context.SchemaRegistry.Definitions.Remove(schemaName);
                if (!context.SchemaRegistry.Definitions.Keys.Contains(validSchemaName))
                {
                    context.SchemaRegistry.Definitions.Add(validSchemaName, schema);
                }
                response.Value.Schema.Ref = validRef;
            }
        }
        private void CorrectProperty(KeyValuePair<string, OpenApiSchema> prop, OperationFilterContext context)
        {
            var invalidRef = prop.Value.Reference.ExternalResource;
            var validRef = invalidRef.Replace("[", "_").Replace(",", "_").Replace("]", "");

            var schemaName = invalidRef.Split('/').LastOrDefault();
            var validSchemaName = validRef.Split('/').LastOrDefault();
            OpenApiSchema schema;
            if (context.SchemaRepository.Schemas.SelectMany(x => x.Value.SchemaRegistry.Definitions.TryGetValue(schemaName, out schema))
            {
                context.SchemaRegistry.Definitions.Remove(schemaName);
                if (!context.SchemaRegistry.Definitions.Keys.Contains(validSchemaName))
                {
                    context.SchemaRegistry.Definitions.Add(validSchemaName, schema);
                }
                prop.Value.Ref = validRef;
            }


        }
    }
}