using EDUCACOOPERN.Models;
using System.ComponentModel.DataAnnotations;

namespace EDUCACOOPERN.ViewModels;

public class PDIViewModel
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O {0} do PDI é obrigatória!")]
    [StringLength(24, MinimumLength = 3, ErrorMessage = "O {0} do PDI deve ter entre 3 e 24 caracteres!")]
    public string? Nome { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; }

    [Display(Name = "Áreas de atuação")]
    [Required(ErrorMessage = "O {0} é obrigatório!")]
    public List<AreaAtuacao>? AreasAtuacao { get; set; }
    
    [Display(Name = "Cursos")]
    public List<PDICursoViewModel>? Cursos { get; set; }
}
