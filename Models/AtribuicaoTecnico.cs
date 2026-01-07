using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetShop.Models;

/// <summary>
/// Modelo que representa a relação N:N entre Técnico e Serviço
/// 
/// Responsabilidades:
/// - Definir quais os serviços que cada técnico está qualificado a realizar
/// - Manter um histórico de competências/certificações
/// - Facilitar o agendamento atribuindo serviços apenas a técnicos qualificados
/// 
/// Relacionamentos:
/// - N:1 com Tecnico
/// - N:1 com Servico
/// - Chave composta: (TecnicoID, ServicoID)
/// </summary>
public class AtribuicaoTecnico
{
    /// <summary>
    /// ID do técnico
    /// Parte da chave composta
    /// Chave estrangeira que referencia a tabela Tecnico
    /// </summary>
    [Required]
    public int TecnicoID { get; set; }

    /// <summary>
    /// ID do serviço
    /// Parte da chave composta
    /// Chave estrangeira que referencia a tabela Servico
    /// </summary>
    [Required]
    public int ServicoID { get; set; }

    /// <summary>
    /// Nível de proficiência do técnico neste serviço
    /// Exemplo: "Junior", "Profissional", "Especialista"
    /// </summary>
    [StringLength(50, ErrorMessage = "O nível não pode exceder 50 caracteres")]
    [Display(Name = "Nível de Proficiência")]
    public string NivelProficiencia { get; set; } = "Profissional";

    /// <summary>
    /// Data em que o técnico foi certificado/atribuído para este serviço
    /// Útil para controle de formação
    /// </summary>
    [DataType(DataType.Date)]
    [Display(Name = "Data de Atribuição")]
    public DateTime DataAtribuicao { get; set; } = DateTime.Now;

    /// <summary>
    /// Observações sobre a qualificação
    /// Exemplo: "Certificado em Tosa Creativa", "Em Treinamento", etc.
    /// </summary>
    [StringLength(500, ErrorMessage = "As observações não podem exceder 500 caracteres")]
    [Display(Name = "Observações")]
    public string? Observacoes { get; set; }

    /// <summary>
    /// Navegação para o técnico
    /// Permite aceder aos dados do técnico
    /// </summary>
    [ForeignKey("TecnicoID")]
    public Tecnico? Tecnico { get; set; }

    /// <summary>
    /// Navegação para o serviço
    /// Permite aceder aos dados do serviço
    /// </summary>
    [ForeignKey("ServicoID")]
    public Servico? Servico { get; set; }
}
