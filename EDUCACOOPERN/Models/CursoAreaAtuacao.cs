using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDUCACOOPERN.Models;

public class CursoAreaAtuacao
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int? CursoId { get; set; }

    [ForeignKey("CursoId")]
    public Curso? Curso { get; set; }

    [Required]
    public int AreaAtuacaoId { get; set; }

    [ForeignKey("AreaAtuacaoId")]
    public AreaAtuacao? AreaAtuacao { get; set; }
}
