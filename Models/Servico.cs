using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PetShop.Models;

/// <summary>
/// Modelo que representa um Serviço oferecido pela PetShop
/// 
/// Responsabilidades:
/// - Definir os tipos de serviços disponíveis (banho, tosa, consulta, vacinação, etc.)
/// - Armazenar preço e descrição de cada serviço
/// - Manter relacionamento com agendamentos e técnicos
/// 
/// Relacionamentos:
/// - 1:N com Agendamento (um serviço pode ter vários agendamentos)
/// - N:N com Tecnico via AtribuicaoTecnico (vários técnicos podem fazer o mesmo serviço)
/// </summary>
public class Servico
{
    /// <summary>
    /// Identificador único do serviço
    /// Chave primária - gerada manualmente (permite controlar IDs de serviços)
    /// </summary>
    public int ServicoID { get; set; }

    /// <summary>
    /// Nome do serviço
    /// Exemplo: "Banho e Tosa", "Limpeza de Ouvidos", "Consulta Veterinária"
    /// </summary>
    [Required(ErrorMessage = "O nome do serviço é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
    [Display(Name = "Nome do Serviço")]
    public required string NomeServico { get; set; }

    /// <summary>
    /// Descrição detalhada do serviço
    /// Informa ao cliente o que está incluído no serviço
    /// </summary>
    [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres")]
    public string? Descricao { get; set; }

    /// <summary>
    /// Preço base do serviço em euros
    /// Exemplo: 25.00 para banho e tosa
    /// </summary>
    [Required(ErrorMessage = "O preço é obrigatório")]
    [Range(0.01, 10000, ErrorMessage = "O preço deve ser entre 0.01 e 10000")]
    [Display(Name = "Preço (€)")]
    public decimal Preco { get; set; }

    /// <summary>
    /// Duração estimada do serviço em minutos
    /// Utilizado para agendamento de turnos (slots)
    /// </summary>
    [Required(ErrorMessage = "A duração é obrigatória")]
    [Range(15, 480, ErrorMessage = "A duração deve estar entre 15 minutos e 8 horas")]
    [Display(Name = "Duração (minutos)")]
    public int DuracaoMinutos { get; set; }

    /// <summary>
    /// Coleção de agendamentos que utilizam este serviço
    /// Relacionamento 1:N
    /// </summary>
    public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();

    /// <summary>
    /// Coleção de atribuições de técnicos a este serviço
    /// Relacionamento N:N via AtribuicaoTecnico
    /// </summary>
    public ICollection<AtribuicaoTecnico> AtribuicoesTecnicos { get; set; } = new List<AtribuicaoTecnico>();
}
