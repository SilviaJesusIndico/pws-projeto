using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PetShop.Models;

/// <summary>
/// Modelo que representa um Cliente (dono de animais de estimação)
/// 
/// Responsabilidades:
/// - Armazenar informações do cliente (nome, apelido, email, telefone)
/// - Manter relacionamento com os animais que o cliente possui
/// 
/// Relacionamentos:
/// - 1:N com Animal (um cliente pode ter vários animais)
/// </summary>
public class Cliente
{
    /// <summary>
    /// Identificador único do cliente
    /// Chave primária - auto-incrementada
    /// </summary>
    public int ClienteID { get; set; }

    /// <summary>
    /// Nome do cliente
    /// Obrigatório com comprimento máximo de 100 caracteres
    /// </summary>
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
    public required string Nome { get; set; }

    /// <summary>
    /// Apelido/Sobrenome do cliente
    /// </summary>
    [Required(ErrorMessage = "O apelido é obrigatório")]
    [StringLength(100, ErrorMessage = "O apelido não pode exceder 100 caracteres")]
    public required string Apelido { get; set; }

    /// <summary>
    /// Email de contacto do cliente
    /// Validado como formato de email
    /// </summary>
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public required string Email { get; set; }

    /// <summary>
    /// Número de telefone do cliente
    /// </summary>
    [Required(ErrorMessage = "O telefone é obrigatório")]
    [Phone(ErrorMessage = "Formato de telefone inválido")]
    public required string Telefone { get; set; }

    /// <summary>
    /// Data de registo do cliente no sistema
    /// </summary>
    [DataType(DataType.Date)]
    [Display(Name = "Data de Registo")]
    public DateTime DataRegisto { get; set; }

    /// <summary>
    /// Propriedade calculada que retorna o nome completo (Nome + Apelido)
    /// </summary>
    public string NomeCompleto => $"{Nome} {Apelido}";

    /// <summary>
    /// Coleção de animais que pertencem a este cliente
    /// Relacionamento 1:N (um cliente pode ter vários animais)
    /// </summary>
    public ICollection<Animal> Animais { get; set; } = new List<Animal>();
}
