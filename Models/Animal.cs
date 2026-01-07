using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace PetShop.Models;

/// <summary>
/// Modelo que representa um Animal de estimação registado na PetShop
/// 
/// Responsabilidades:
/// - Armazenar informações do animal (nome, espécie, raça, data de nascimento)
/// - Manter referência ao cliente proprietário
/// - Relacionar-se com agendamentos de serviços
/// 
/// Relacionamentos:
/// - N:1 com Cliente (vários animais podem pertencer a um cliente)
/// - 1:N com Agendamento (um animal pode ter vários agendamentos)
/// </summary>
public class Animal
{
    /// <summary>
    /// Identificador único do animal
    /// Chave primária - auto-incrementada
    /// </summary>
    public int AnimalID { get; set; }

    /// <summary>
    /// Nome do animal de estimação
    /// Exemplo: "Fluffy", "Rex", "Mimi"
    /// </summary>
    [Required(ErrorMessage = "O nome do animal é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
    public required string Nome { get; set; }

    /// <summary>
    /// ID do cliente proprietário deste animal
    /// Chave estrangeira que referencia a tabela Cliente
    /// </summary>
    [Required]
    public int ClienteID { get; set; }

    /// <summary>
    /// Espécie do animal: Cão, Gato, Coelho, Hamster, etc.
    /// </summary>
    [Required(ErrorMessage = "A espécie é obrigatória")]
    [StringLength(50, ErrorMessage = "A espécie não pode exceder 50 caracteres")]
    public required string Especie { get; set; }

    /// <summary>
    /// Raça do animal
    /// Exemplo: "Labrador", "Persa", "Ouro Chinês"
    /// </summary>
    [Required(ErrorMessage = "A raça é obrigatória")]
    [StringLength(50, ErrorMessage = "A raça não pode exceder 50 caracteres")]
    public required string Raca { get; set; }

    /// <summary>
    /// Data de nascimento do animal
    /// Calculada em relação à data atual para determinar idade
    /// </summary>
    [Required(ErrorMessage = "A data de nascimento é obrigatória")]
    [DataType(DataType.Date)]
    [Display(Name = "Data de Nascimento")]
    public DateTime DataNascimento { get; set; }

    /// <summary>
    /// Peso do animal em quilogramas
    /// Útil para cálculo de dosagens e atribuição de tarefas
    /// </summary>
    [Range(0.1, 500, ErrorMessage = "O peso deve estar entre 0.1 e 500 kg")]
    public decimal? Peso { get; set; }

    /// <summary>
    /// Observações adicionais sobre o animal
    /// Alergias, comportamentos especiais, preferências, etc.
    /// </summary>
    [StringLength(500, ErrorMessage = "As observações não podem exceder 500 caracteres")]
    [Display(Name = "Observações Especiais")]
    public string? ObservacoesMedicas { get; set; }

    /// <summary>
    /// Navegação para o cliente proprietário
    /// Esta propriedade estabelece o relacionamento com a tabela Cliente
    /// </summary>
    [ForeignKey("ClienteID")]
    public Cliente? Cliente { get; set; }

    /// <summary>
    /// Coleção de agendamentos para este animal
    /// Relacionamento 1:N (um animal pode ter vários agendamentos)
    /// </summary>
    public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
}
