using System.ComponentModel.DataAnnotations;

namespace Api_entradas.Atributes
{
    public class FutureDateAttribute: ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return false; // Validar que no sea null

            if (value is DateTime date)
            {
                return date > DateTime.Now;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage ?? $"El campo {name} debe ser una fecha futura.";
        }
    }
}
