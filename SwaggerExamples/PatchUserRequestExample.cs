using Microsoft.AspNetCore.JsonPatch.Operations;
using Swashbuckle.AspNetCore.Filters;

public class PatchUserRequestExample : IExamplesProvider<List<Operation>>
{
    public List<Operation> GetExamples()
    {
        return new List<Operation>
        {
            new Operation
            {
                op = "replace",
                path = "/Email",
                value = "nuevo@email.com"
            },
            new Operation
            {
                op = "replace",
                path = "/Usuario",
                value = "nuevoUsuario"
            },
            new Operation
            {
                op = "replace",
                path = "/FechaNacimiento",
                value = "2024-01-01"
            }
        };
    }
}