using Api_entradas.Atributes;
using System.ComponentModel.DataAnnotations;

namespace Api_entradas.DTOs
{
    public class EventDto
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(40, ErrorMessage = "El título no puede superar los 40 caracteres.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "La fecha del evento es obligatoria.")]
        [FutureDate(ErrorMessage = "La fecha del evento debe ser en el futuro.")]
        public DateTime Date { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El total de entradas debe ser al menos 1.")]
        public int TotalTickets { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0.")]
        public double Price { get; set; }
    }
}
