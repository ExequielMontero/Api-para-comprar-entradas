using Api_entradas.Atributes;
using Api_entradas.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Api_entradas.DTOs
{
    public class UpdateRoleDto
    {   
        [ValoresNumericos(ErrorMessage = "Solo se permiten valores entre 0 y 2.")]
        [Required]
        public UserRole Role { get; set; }
    }
}
