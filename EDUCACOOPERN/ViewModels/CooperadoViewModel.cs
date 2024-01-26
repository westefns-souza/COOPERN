
using EDUCACOOPERN.Models;
using System.ComponentModel.DataAnnotations;

namespace EDUCACOOPERN.ViewModels;

public class CooperadoViewModel
{
    [Display(Name = "Nome completo")]
    [Required(ErrorMessage = "O {0} é obrigatório!")]
    public string Nome { get; set; }

    [Display(Name = "E-mail")]
    [Required(ErrorMessage = "O {0} é obrigatório!")]
    [EmailAddress(ErrorMessage = "O {0} deve ser válido!")]
    public string Email { get; set; }

    [Display(Name = "Celular")]
    [Required(ErrorMessage = "O {0} é obrigatório!")]
    public string Celular { get; set; }

    [Display(Name = "Registro")]
    [Required(ErrorMessage = "O {0} é obrigatório!")]
    public string Registro { get; set; }

    [Display(Name = "Áreas de atuação")]
    [Required(ErrorMessage = "O {0} é obrigatório!")]
    public List<AreaAtuacao> AreasAtuacao { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; }
}
