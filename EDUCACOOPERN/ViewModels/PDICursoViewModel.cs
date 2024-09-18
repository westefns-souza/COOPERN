using System.ComponentModel.DataAnnotations;

namespace EDUCACOOPERN.ViewModels;

public class PDICursoViewModel
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O {0} do curso é obrigatória!")]
    [StringLength(256, MinimumLength = 3, ErrorMessage = "O {0} do curso deve ter entre 3 e 256 caracteres!")]
    public string? Nome { get; set; }
}