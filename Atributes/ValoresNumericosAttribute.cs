using System.ComponentModel.DataAnnotations;
using Api_entradas.Enums;

namespace Api_entradas.Atributes
{
    public class ValoresNumericosAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is UserRole) return true; // Valor ya válido

            if (value is int intValue)
                return Enum.IsDefined(typeof(UserRole), intValue);

            if (value is string strValue &&
                Enum.TryParse(typeof(UserRole), strValue, out var parsedValue))
                return Enum.IsDefined(typeof(UserRole), parsedValue);

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage ?? $"El campo {name} debe ser un número válido entre {(int)UserRole.Admin} y {(int)UserRole.Cliente}.";
        }
    }

}
