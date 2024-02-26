using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDUCACOOPERN.Models;

public class Matricula
{
    [Key]
    public int Id { get; set; }
    
    [Display(Name = "Aula")]
    public int AulaId { get; set; }

    [ForeignKey("AulaId")]
    public Aula? Aula { get; set; }

    [Display(Name = "Aluno")]
    public string? AlunoId { get; set; }

    [ForeignKey("AlunoId")]
    public ApplicationUser? Aluno { get; set; }

    [Display(Name = "Nota pré teste")]
    public decimal NotaPre { get; set; }

    [Display(Name = "Nota pós teste")]
    public decimal NotaPos { get; set; }

    [Display(Name = "Compareceu")]
    public bool Compareceu { get; set; }

    [Display(Name = "Situação")]
    public EStatusMatricula Status { get; set; }
}
