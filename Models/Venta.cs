﻿using System.ComponentModel.DataAnnotations;
using Api_entradas.Atributes;

namespace Api_entradas.Models
{
    public class Venta
    {
        [Key]
        [Required]
        public int Id { get; set; } 
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Usuario User { get; set; } = null!;

        [Required]
        public Guid EventId { get; set; }

        [Required]
        public Evento Event { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de compra es obligatoria.")]
        [DataType(DataType.DateTime)]
        [DateValidation(ErrorMessage = "La fecha de compra no puede ser futura.")]
        public DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "La cantidad de tickets comprados es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de tickets comprados debe ser mayor que 0.")]
        public int TicketsBought { get; set; }
    }
}
