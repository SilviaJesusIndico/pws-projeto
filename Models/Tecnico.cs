using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PetShop.Models;

/// <summary>
/// Modelo que representa um Técnico/Funcionário da PetShop
/// 
/// Responsabilidades:
/// - Armazenar informações dos profissionais (nome, especialização, etc.)
/// - Manter histórico de agendamentos realizados
/// - Registar competências (serviços que o técnico sabe fazer)
/// 
/// Relacionamentos:
/// - 1:N com Agendamento (um técnico pode fazer vários agendamentos)
/// - N:N com Servico via AtribuicaoTecnico (um técnico pode fazer vários serviços)
/// </summary>
public class Tecnico
{
    /// <summary>
    /// Identificador único do técnico
    /// Chave primária - auto-incrementada
    /// </summary>
    public int TecnicoID { get; set; }

    /// <summary>
    /// Nome do técnico
    /// Obrigatório com comprimento máximo de 100 caracteres
    /// </summary>
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
    public required string Nome { get; set; }

    /// <summary>
    /// Apelido/Sobrenome do técnico
    /// </summary>
    [Required(ErrorMessage = "O apelido é obrigatório")]
    [StringLength(100, ErrorMessage = "O apelido não pode exceder 100 caracteres")]
    public required string Apelido { get; set; }

    /// <summary>
    /// Email de contacto do técnico
    /// Utilizado para comunicações internas
    /// </summary>
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public required string Email { get; set; }

    /// <summary>
    /// Especialização principal do técnico
    /// Exemplo: "Tosador", "Banho", "Consultor Comportamental"
    /// </summary>
    [Required(ErrorMessage = "A especialização é obrigatória")]
    [StringLength(100, ErrorMessage = "A especialização não pode exceder 100 caracteres")]
    [Display(Name = "Especialização")]
    public required string Especializacao { get; set; }

    /// <summary>
    /// Data de contratação do técnico
    /// </summary>
    [Required(ErrorMessage = "A data de contratação é obrigatória")]
    [DataType(DataType.Date)]
    [Display(Name = "Data de Contratação")]
    public DateTime DataContratacao { get; set; }

    /// <summary>
    /// Número de telefone do técnico para contactos emergenciais
    /// </summary>
    [Phone(ErrorMessage = "Formato de telefone inválido")]
    public string? Telefone { get; set; }

    /// <summary>
    /// Indicador se o/a técnico/a está ativo/a no sistema
    /// TRUE = ativo/a e disponível para agendamentos
    /// FALSE = técnico/a inativo/a (férias, saída, etc.)
    /// </summary>
    [Display(Name = "Ativo/a")]
    public bool Ativo { get; set; } = true;

    /// <summary>
    /// Propriedade calculada que retorna o nome completo do técnico
    /// Útil para exibição em formulários e listas
    /// </summary>
    [Display(Name = "Nome Completo")]
    public string NomeCompleto
    {
        get { return Nome + " " + Apelido; }
    }

    /// <summary>
    /// Coleção de agendamentos realizados por este técnico
    /// Relacionamento 1:N (um técnico pode ter vários agendamentos)
    /// </summary>
    public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();

    /// <summary>
    /// Coleção de atribuições de serviços a este técnico
    /// Relacionamento N:N via AtribuicaoTecnico
    /// Define quais os serviços que o técnico está qualificado a fazer
    /// </summary>
    public ICollection<AtribuicaoTecnico> AtribuicoesTecnicos { get; set; } = new List<AtribuicaoTecnico>();
}
