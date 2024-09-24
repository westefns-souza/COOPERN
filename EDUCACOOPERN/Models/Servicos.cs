using System.ComponentModel.DataAnnotations;

namespace EDUCACOOPERN.Models;

public class Servicos
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string? Nome { get; set; }

    public bool Ativo { get; set; }
}