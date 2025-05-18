using System.ComponentModel.DataAnnotations;
using Api_entradas.Enums;

namespace Api_entradas.Models
{
    public class User
    {

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Usuario { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public UserRole Role { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }
}
