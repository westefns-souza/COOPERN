using System.ComponentModel.DataAnnotations;

namespace EDUCACOOPERN.Models;
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

public class AreaAtuacao
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O {0} da área de atuação é obrigatória!")]
    [StringLength(24, MinimumLength = 3, ErrorMessage = "O {0} da área de atuação deve ter entre 3 e 24 caracteres!")]
    public string? Nome { get; set; }
    
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; }
}