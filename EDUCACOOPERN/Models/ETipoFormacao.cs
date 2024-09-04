using System.ComponentModel.DataAnnotations;

namespace EDUCACOOPERN.Models;

public enum ETipoFormacao
{
    Bacharelado,
    Mestrado,
    Doutorado,
    [Display(Name = "Especialização")]
    Especializacao,
    [Display(Name = "Técnico")]
    Tecnico,
}