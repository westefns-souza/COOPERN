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
    [StringLength(100, ErrorMessage = "O {0} do usuário deve ter até 100 caracteres!")]
    public string? NomeAlternativo { get; set; }

    [Display(Name = "Conselho")]
    //[Required(ErrorMessage = "O {0} é obrigatório!")]
    [StringLength(24, ErrorMessage = "O {0} do usuário deve ter até 24 caracteres!")]
    public string? Conselho { get; set; }

    [Display(Name = "UF do conselho")]
    //[Required(ErrorMessage = "O {0} é obrigatório!")]
    [StringLength(2, ErrorMessage = "O {0} do usuário deve ter até 2 caracteres!")]
    public string? UFConselho { get; set; }

    [Display(Name = "Número do conselho")]
    //[Required(ErrorMessage = "O {0} é obrigatório!")]
    [StringLength(11, ErrorMessage = "O {0} do usuário deve ter até 11 caracteres!")]
    public string? NumeroConselho { get; set; }

    [Display(Name = "CPF")]
    //[Required(ErrorMessage = "O {0} é obrigatório!")]
    [StringLength(11, ErrorMessage = "O {0} do usuário deve ter até 11 caracteres!")]
    public string? CPF { get; set; }

    [Display(Name = "Profissão")]
    //[Required(ErrorMessage = "O {0} é obrigatório!")]
    [StringLength(100, ErrorMessage = "A {0} do usuário deve ter até 100 caracteres!")]
    public string? Profissião { get; set; }

    [Display(Name = "Data de Nascimento")]
    //[Required(ErrorMessage = "A {0} é obrigatório!")]
    public DateTime? DataNascimento { get; set; }
}