using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDUCACOOPERN.Models;

public class Custos
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O {0} do custo é obrigatório!")]
    public int CursoId { get; set; }

    [ForeignKey("CursoId")]
    public Curso? Curso { get; set; }

    [Required(ErrorMessage = "O {0} do custo é obrigatório!")]
    public decimal Valor { get; set; }

    [Display(Name = "Classificação")]
    [Required(ErrorMessage = "A {0} do custo é obrigatória!")]
    public EClassificacao Classificacao { get; set; }
}