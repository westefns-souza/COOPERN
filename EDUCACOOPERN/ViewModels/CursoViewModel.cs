using EDUCACOOPERN.Models;
using System.ComponentModel.DataAnnotations;

namespace EDUCACOOPERN.ViewModels;

public class CursoViewModel
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O {0} do curso é obrigatória!")]
    [StringLength(256, MinimumLength = 3, ErrorMessage = "O {0} do curso deve ter entre 3 e 256 caracteres!")]
    public string? Nome { get; set; }

    [Display(Name = "Descrição")]
    [Required(ErrorMessage = "O {0} do curso é obrigatória!")]
    [StringLength(5000, MinimumLength = 3, ErrorMessage = "O {0} do curso deve ter entre 3 e 5000 caracteres!")]
    public string? Descricao { get; set; }
    
    [Display(Name = "Áreas de atuação")]
    [Required(ErrorMessage = "O {0} é obrigatório!")]
    public List<AreaAtuacao>? AreasAtuacao { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; }
}
