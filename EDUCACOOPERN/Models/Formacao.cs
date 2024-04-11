using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDUCACOOPERN.Models;

public class Formacao
{
    public int Id { get; set; }

    public string? UsuarioId { get; set; }

    [ForeignKey("UsuarioId")]
    public ApplicationUser? Usuario { get; set; }

    [Required]
    [StringLength(100)]
    public string? Nome { get; set; }

    [Required]
    public ETipoFormacao Tipo { get; set; }
}
