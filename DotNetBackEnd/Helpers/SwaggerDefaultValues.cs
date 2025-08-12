using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Reflection;
using Microsoft.OpenApi.Any;


namespace LibraryMgmt.Helpers
{
    public class SwaggerDefaultValues : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            foreach (var property in context.Type.GetProperties())
            {
                var defaultValue = property.GetCustomAttribute<DefaultValueAttribute>();
                if (defaultValue != null)
                {
                    var propName = property.Name.Substring(0, 1).ToLower() + property.Name.Substring(1);
                    if (schema.Properties.ContainsKey(propName))
                    {
                        schema.Properties[propName].Example = CreateOpenApiExample(defaultValue.Value);
                    }
                }
            }
        }

        private IOpenApiAny CreateOpenApiExample(object value)
        {
            return value switch
            {
                string s => new OpenApiString(s),
                int i => new OpenApiInteger(i),
                bool b => new OpenApiBoolean(b),
                double d => new OpenApiDouble(d),
                float f => new OpenApiFloat(f),
                long l => new OpenApiLong(l),
                _ => new OpenApiString(value?.ToString() ?? "")
            };
        }
    }
}
