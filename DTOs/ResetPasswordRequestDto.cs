using System.ComponentModel.DataAnnotations;

namespace Api_entradas.DTOs
{
    public class ResetPasswordRequestDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [MinLength(4, ErrorMessage = "El nombre de usuario debe tener al menos 4 caracteres.")]
        [MaxLength(20, ErrorMessage = "El nombre de usuario no debe superar los 20 caracteres.")]
        public string User { get; set; } = null!;
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Password { get; set; } = null!;
    }
}
