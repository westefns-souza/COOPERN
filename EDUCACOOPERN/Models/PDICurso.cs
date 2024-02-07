using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDUCACOOPERN.Models;

public class PDICurso
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int? PDId { get; set; }

    [ForeignKey("PDId")]
    public PDI? PDI { get; set; }

    [Required]
    public int CursoId { get; set; }

    [ForeignKey("CursoId")]
    public Curso? Curso { get; set; }
}