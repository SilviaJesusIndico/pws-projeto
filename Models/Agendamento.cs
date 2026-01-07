using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetShop.Models;

/// <summary>
/// Modelo que representa um Agendamento de serviço para um animal
/// 
/// Responsabilidades:
/// - Registar quando um cliente agenda um serviço para o seu animal
/// - Armazenar data, hora e status do agendamento
/// - Manter referências ao animal, serviço e técnico responsável
/// 
/// Relacionamentos:
/// - N:1 com Animal (vários agendamentos podem ser do mesmo animal)
/// - N:1 com Servico (vários agendamentos podem ser do mesmo serviço)
/// - N:1 com Tecnico (vários agendamentos podem ser com o mesmo técnico)
/// </summary>
public class Agendamento
{
    /// <summary>
    /// Identificador único do agendamento
    /// Chave primária - auto-incrementada
    /// </summary>
    public int AgendamentoID { get; set; }

    /// <summary>
    /// ID do animal que receberá o serviço
    /// Chave estrangeira que referencia a tabela Animal
    /// </summary>
    [Required(ErrorMessage = "O animal é obrigatório")]
    public int AnimalID { get; set; }

    /// <summary>
    /// ID do serviço que será prestado
    /// Chave estrangeira que referencia a tabela Servico
    /// </summary>
    [Required(ErrorMessage = "O serviço é obrigatório")]
    public int ServicoID { get; set; }

    /// <summary>
    /// ID do técnico responsável pelo agendamento
    /// Pode ser NULL até que um técnico seja atribuído
    /// Chave estrangeira que referencia a tabela Tecnico
    /// </summary>
    public int? TecnicoID { get; set; }

    /// <summary>
    /// Data e hora do agendamento
    /// Exemplo: 2025-12-24 14:30:00
    /// </summary>
    [Required(ErrorMessage = "A data e hora são obrigatórias")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Data e Hora")]
    public DateTime DataHora { get; set; }

    /// <summary>
    /// Status do agendamento: Pendente, Confirmado, Em Progresso, Concluído, Cancelado
    /// Controlado por um dropdown no frontend
    /// </summary>
    [Required(ErrorMessage = "O status é obrigatório")]
    [StringLength(50, ErrorMessage = "O status não pode exceder 50 caracteres")]
    [Display(Name = "Status")]
    public string Status { get; set; } = "Pendente";

    /// <summary>
    /// Observações do cliente ou notas especiais para o técnico
    /// Exemplo: "Animal nervoso", "Preferir banho morno", etc.
    /// </summary>
    [StringLength(500, ErrorMessage = "As observações não podem exceder 500 caracteres")]
    [Display(Name = "Observações")]
    public string? Observacoes { get; set; }

    /// <summary>
    /// Preço final cobrado pelo serviço
    /// Pode diferir do preço base se houver ajustes
    /// </summary>
    [Range(0, 10000, ErrorMessage = "O preço deve ser válido")]
    [Display(Name = "Preço Final (€)")]
    public decimal? PrecoFinal { get; set; }

    /// <summary>
    /// Data de criação do agendamento no sistema
    /// </summary>
    [DataType(DataType.DateTime)]
    [Display(Name = "Data de Criação")]
    public DateTime DataCriacao { get; set; } = DateTime.Now;

    /// <summary>
    /// Navegação para o animal que receberá o serviço
    /// </summary>
    [ForeignKey("AnimalID")]
    public Animal? Animal { get; set; }

    /// <summary>
    /// Navegação para o serviço agendado
    /// </summary>
    [ForeignKey("ServicoID")]
    public Servico? Servico { get; set; }

    /// <summary>
    /// Navegação para o técnico responsável (pode ser null)
    /// </summary>
    [ForeignKey("TecnicoID")]
    public Tecnico? Tecnico { get; set; }
}
