using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDUCACOOPERN.Models;

public class UsuarioPDI
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string? UsuarioId { get; set; }

    [ForeignKey("UsuarioId")]
    public ApplicationUser? Usuario { get; set; }

    [Required]
    public int PDIId { get; set; }

    [ForeignKey("PDIId")]
    public PDI? PDI { get; set; }
}
