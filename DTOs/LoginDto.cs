using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
public class LoginDto
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    [DefaultValue("admin@gmail.com")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [DefaultValue("admins")]
    public string Password { get; set; } = null!;   // ← ahora es propiedad
}
