using System.ComponentModel.DataAnnotations;

namespace EDUCACOOPERN.Models;

public enum ETipoFormacao
{
    [Display(Name = "Bacharelado")]
    Bacharelado,
    
    [Display(Name = "Mestrado")]
    Mestrado,
    
    [Display(Name = "Doutorado")]
    Doutorado,
    
    [Display(Name = "Especialização")]
    Especializacao,
    
    [Display(Name = "Técnico(a) em Enfermagem")]
    Tecnico
}