using System.ComponentModel.DataAnnotations;
using Api_entradas.Atributes;

namespace Api_entradas.Models
{
    public class UserEvent
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public User User { get; set; } = null!;

        [Required]
        public Guid EventId { get; set; }

        [Required]
        public Event Event { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de compra es obligatoria.")]
        [DataType(DataType.DateTime)]
        [DateValidation(ErrorMessage = "La fecha de compra no puede ser futura.")]
        public DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "La cantidad de tickets comprados es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de tickets comprados debe ser mayor que 0.")]
        public int TicketsBought { get; set; }
    }
}
