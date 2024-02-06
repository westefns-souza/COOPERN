using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EDUCACOOPERN.Models;

public class ApplicationUser : IdentityUser
{
    [Display(Name = "Nome completo")]
    [StringLength(256, MinimumLength = 3, ErrorMessage = "O {0} do usuário deve ter entre 3 e 256 caracteres!")]
    public string? FullName { get; set; }

    [StringLength(24, ErrorMessage = "O {0} do usuário deve ter até 24 caracteres!")]
    public string? Registro { get; set; }

    [Display(Name = "Celular alternativo")]
    [Required(ErrorMessage = "O {0} é obrigatório!")]
    [StringLength(24, ErrorMessage = "O {0} do usuário deve ter até 24 caracteres!")]
    public string? CelularAlternativo { get; set; }

    [Display(Name = "Nome para contato")]
    [Required(ErrorMessage = "O {0} é obrigatório!")]
    [StringLength(100, ErrorMessage = "O {0} do usuário deve ter até 24 caracteres!")]
    public string? NomeAlternativo { get; set; }
}