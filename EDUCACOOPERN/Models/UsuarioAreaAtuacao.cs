using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDUCACOOPERN.Models;

public class UsuarioAreaAtuacao
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string? UsuarioId { get; set; }

    [ForeignKey("UsuarioId")]
    public ApplicationUser? Usuario { get; set; }
    
    [Required]
    public int AreaAtuacaoId { get; set; }

    [ForeignKey("AreaAtuacaoId")]
    public AreaAtuacao? AreaAtuacao { get; set; }

    public int? ServicosId { get; set; }

    [ForeignKey("ServicosId")]
    public Servicos? Servicos { get; set; }
}