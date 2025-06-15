using System.ComponentModel.DataAnnotations;
using Api_entradas.Atributes;

namespace Api_entradas.Models
{
    public class Evento
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(40, ErrorMessage = "El título no puede superar los 40 caracteres.")]
        public string Title { get; set; } = "";

        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0.")]
        public double Price { get; set; }

        [Url(ErrorMessage = "Debe ser una URL válida.")]
        public string? BannerEvento { get; set; }

        [Required(ErrorMessage = "La fecha del evento es obligatoria.")]
        [FutureDate(ErrorMessage = "La fecha del evento debe ser en el futuro.")]
        public DateTime Date { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El total de entradas debe ser al menos 1.")]
        public int TotalTickets { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Las entradas vendidas no pueden ser negativas.")]
        public int TicketsSold { get; set; }


        public ICollection<Venta> UserEvents { get; set; } = new List<Venta>();

    }

}
