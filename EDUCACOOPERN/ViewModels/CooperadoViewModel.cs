using EDUCACOOPERN.Models;
using System.ComponentModel.DataAnnotations;
using X.PagedList;

namespace EDUCACOOPERN.ViewModels;

public class HomeViewModel
{
    public CooperadoHomeViewModel? CooperadoHome { get; set; }
    
    public ProfessorHomeViewModel? ProfessorHome { get; set; }

    public AdministradorViewModel? AdministradorHome { get; set; }
}

public class ProfessorHomeViewModel
{
    public IList<Aula>? MeusCuros { get; set; }
}

public class AdministradorViewModel
{
    public Dictionary<string?, int>? QuantidadeDeInscritosPorAreaDeAtuacao { get; set; }
    public Dictionary<string?, int>? QuantidadeDeInscritosPorCurso { get; set; }
}

public class CooperadoHomeViewModel
{
    public PDI? PDI { get; set; }
    public int QuantidadeCursosRealizados { get; set; }
    public int QuantidadeCursosNaoRealizados { get; set; }
    public IList<int> CursosRealizados { get; set; }
    public IList<Aula>? CurosDoMes { get; set; }

    public IList<Aula>? MeusCuros { get; set; }
}

public class IndexCooperados
{
    public string? Nome { get; set; }
    public string? Perfil { get; set; }
    public int? Pagina { get; set; } = 1;

    public IPagedList<ApplicationUser>? Usuario { get; set; }
}

public class CooperadoViewModel
{
    public string? Id { get; set; }

    [Display(Name = "Nome completo")]
    [Required(ErrorMessage = "O {0} é obrigatório!")]
    public string? Nome { get; set; }

    [Display(Name = "E-mail")]
    [Required(ErrorMessage = "O {0} é obrigatório!")]
    [EmailAddress(ErrorMessage = "O {0} deve ser válido!")]
    public string? Email { get; set; }

    [Display(Name = "Celular")]
    [Required(ErrorMessage = "O {0} é obrigatório!")]
    public string? Celular { get; set; }

    [Display(Name = "Registro")]
    [Required(ErrorMessage = "O {0} é obrigatório!")]
    public string? Registro { get; set; }

    [Display(Name = "Áreas de atuação")]
    [Required(ErrorMessage = "O {0} é obrigatório!")]
    public List<AreaAtuacao>? AreasAtuacao { get; set; }

    [Display(Name = "PDI")]
    //[Required(ErrorMessage = "O {0} é obrigatório!")]
    public List<PDI>? PDIs { get; set; }

    [Display(Name = "Celular alternativo")]
    //[Required(ErrorMessage = "O {0} é obrigatório!")]
    public string? CelularAlternativo { get; set; }

    [Display(Name = "Nome para contato")]
    //[Required(ErrorMessage = "O {0} é obrigatório!")]
    public string? NomeAlternativo { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; }

    [Display(Name = "Professor(a)")]
    public bool Professor { get; set; }

    [Display(Name = "Formações")]
    //[Required(ErrorMessage = "O {0} é obrigatório!")]
    public List<Formacao>? Formacoes { get; set; }
}
