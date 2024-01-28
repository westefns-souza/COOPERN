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
}