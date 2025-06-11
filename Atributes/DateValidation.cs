using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Api_entradas.Atributes
{
    public class DateValidation : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return false;

            if (value is string dateString)
            {
                // Validar formato exacto "yyyy-MM-d" sin tiempo
                bool esValida = DateTime.TryParseExact(
                    dateString,
                    "yyyy-MM-d",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime date);

                if (!esValida) return false;

                // Validar que la fecha sea menor a hoy (fecha pasada)
                return date < DateTime.Today;
            }

            // Si te llega un DateTime directamente, se valida igual:
            if (value is DateTime dateValue)
            {
                return dateValue.Date < DateTime.Today;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage ?? $"El campo {name} debe ser una fecha válida en formato AAAA-MM-D y menor a hoy.";
        }
    }
}
