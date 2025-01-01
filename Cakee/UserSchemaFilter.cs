using Cakee.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class UserSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(User))
        {
            schema.Properties.Remove("id");
            schema.Properties.Remove("role");
            schema.Properties.Remove("createdAt");
        }
    }
}
