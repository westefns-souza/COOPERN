using EDUCACOOPERN.Data.Migrations;
using System.ComponentModel.DataAnnotations;

namespace EDUCACOOPERN.Models;

public class PDI
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O {0} do PDI é obrigatória!")]
    [StringLength(24, MinimumLength = 3, ErrorMessage = "O {0} do PDI deve ter entre 3 e 24 caracteres!")]
    public string? Nome { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; }

    public IList<PDIAreaAtuacao>? PDIAreaAtuacoes { get; set; }
    public IList<PDICurso>? PDICursos { get; set; }
}
