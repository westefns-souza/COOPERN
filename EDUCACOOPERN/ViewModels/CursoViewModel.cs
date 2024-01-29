using EDUCACOOPERN.Models;
using System.ComponentModel.DataAnnotations;

namespace EDUCACOOPERN.ViewModels;

public class CursoViewModel
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O {0} da área de atuação é obrigatória!")]
    [StringLength(24, MinimumLength = 3, ErrorMessage = "O {0} da área de atuação deve ter entre 3 e 24 caracteres!")]
    public string? Nome { get; set; }

    [Display(Name = "Descrição")]
    [Required(ErrorMessage = "O {0} da área de atuação é obrigatória!")]
    [StringLength(5000, MinimumLength = 3, ErrorMessage = "O {0} da área de atuação deve ter entre 3 e 5000 caracteres!")]
    public string? Descricao { get; set; }
    
    [Display(Name = "Áreas de atuação")]
    [Required(ErrorMessage = "O {0} é obrigatório!")]
    public List<AreaAtuacao>? AreasAtuacao { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; }
}