using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDUCACOOPERN.Models;

public class Certificado
{
    [Key]
    public int Id { get; set; }
    
    [Display(Name = "Descrição")]
    [Required(ErrorMessage = "A {0} do cestificado é obrigatória!")]
    public string? Descricao { get; set; }
    
    public string? Arquivo { get; set; }

    [Display(Name = "Extenção")]
    public string? Extencao { get; set; }
    
    public string? UsuarioId { get; set; }

    
    [ForeignKey(nameof(UsuarioId))]
    public ApplicationUser? Usuario { get; set; }
}
