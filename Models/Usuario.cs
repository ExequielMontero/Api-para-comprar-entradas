using System.ComponentModel.DataAnnotations;
using Api_entradas.Enums;

namespace Api_entradas.Models
{
    public class Usuario
    {

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string User { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public UserRole Role { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public bool EstaVerificado { get; set; } = false;

        public ICollection<Venta> UserEvents { get; set; } = new List<Venta>();

    }
}
