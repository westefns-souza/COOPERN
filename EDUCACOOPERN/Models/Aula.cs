using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDUCACOOPERN.Models;

public class Aula
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Curso")]
    [Required(ErrorMessage = "O {0} do curso é obrigatória!")]
    public int CursoId { get; set; }
    
    [ForeignKey("CursoId")]
    public Curso? Curso { get; set; }

    [Display(Name = "Professor")]
    [Required(ErrorMessage = "O {0} do curso é obrigatória!")]
    public string ProfessorId { get; set; }

    [ForeignKey("ProfessorId")]
    public ApplicationUser? Professor { get; set; }

    [Display(Name = "Data de início")]
    [Required(ErrorMessage = "A {0} do curso é obrigatória!")]
    public DateTime DataInicio { get; set; }

    [Display(Name = "Data de fim")]
    [Required(ErrorMessage = "A {0} do curso é obrigatória!")]
    public DateTime DataFim { get; set; }

    public decimal? Receita { get; set; }

    public List<Custos>? Custos { get; set; }
    public List<Matricula>? Matriculas { get; set; }

    [Required(ErrorMessage = "O {0} do curso é obrigatória!")]
    public EStatusAula Status { get; set; }


}
