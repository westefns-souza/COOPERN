using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDUCACOOPERN.Models;

public class PDIAreaAtuacao
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int? PDIId { get; set; }

    [ForeignKey("PDIId")]
    public PDI? PDII { get; set; }

    [Required]
    public int AreaAtuacaoId { get; set; }

    [ForeignKey("AreaAtuacaoId")]
    public AreaAtuacao? AreaAtuacao { get; set; }
}
