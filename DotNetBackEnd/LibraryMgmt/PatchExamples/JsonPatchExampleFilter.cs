using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using Swashbuckle.AspNetCore.SwaggerGen;

public class JsonPatchExampleFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.Name == "ReturnBook")
        {
            var today = DateTime.Now.ToString("o");

            var patchExample = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["op"] = new OpenApiString("replace"),
                    ["path"] = new OpenApiString("/DateReturned"),
                    ["value"] = new OpenApiString(today)
                }
            };

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = {
                    ["application/json-patch+json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "array",
                            Items = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = {
                                    ["op"] = new OpenApiSchema { Type = "string" },
                                    ["path"] = new OpenApiSchema { Type = "string" },
                                    ["value"] = new OpenApiSchema { Type = "string" }
                                },
                                Required = new HashSet<string> { "op", "path", "value" }
                            }
                        },
                        Example = patchExample
                    }
                }
            };
        }
    }
}
