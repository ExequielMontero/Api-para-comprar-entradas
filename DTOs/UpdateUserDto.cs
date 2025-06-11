using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Api_entradas.Atributes;

public class UpdateUserDto
{
    [Display(Description = "Quitar aquel atributo que no quieran actualizar del body")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres.")]
    [DefaultValue("")]
    public string Usuario { get; set; } = null!;

    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    [DefaultValue("")]
    public string Email { get; set; } = null!;

    [DateValidation(ErrorMessage = "La fecha debe tener el formato yyyy-mm-d y debe ser pasada")]
    [DefaultValue("")]
    public DateTime? FechaNacimiento { get; set; } = null;



}
