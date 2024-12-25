namespace EDUCACOOPERN.DTOs;

public class PDISintetico
{
    public int IdPDI { get; set; }
    public string? NomePDI { get; set; }
    public int IdCurso { get; set; }
    public string? NomeCurso { get; set; }
    
    public string? IdUsuario { get; set; }
    public string? NomeUsuario { get; set; }

    public int NaoRealizado { get; set; }
    public int Matriculado { get; set; }
    public int Realizado { get; set; }
    public int TotalPdi { get; set; }
}
