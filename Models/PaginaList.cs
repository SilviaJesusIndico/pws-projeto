using System;
using Microsoft.EntityFrameworkCore;

namespace PetShop.Models;

/// <summary>
/// Classe Genérica para Paginação de Dados
/// 
/// Responsabilidades:
/// - Estender a classe List<T> para incluir informações de paginação
/// - Calcular número total de páginas
/// - Verificar se existem páginas anteriores/posteriores
/// - Criar páginas de forma assíncrona a partir de queries
/// 
/// Uso:
/// Utilizada em páginas Index para exibir dados em lotes (ex: 3 animais por página)
/// 
/// Exemplo de uso:
/// var paginaAnimais = await PaginaList<Animal>.CreateAsync(
///     _context.Animal.OrderBy(a => a.Nome),
///     pageNumber: 1,
///     pageSize: 10
/// );
/// </summary>
public class PaginaList<T> : List<T>
{
    /// <summary>
    /// Número da página actual (começa em 1)
    /// </summary>
    public int PaginaIndex { get; private set; }

    /// <summary>
    /// Número total de páginas disponíveis
    /// Calculado como: Math.Ceiling(TotalItems / TamanhoPagina)
    /// </summary>
    public int TotalPaginas { get; private set; }

    /// <summary>
    /// Construtor que recebe os items, contagem total e informações de paginação
    /// </summary>
    public PaginaList(List<T> items, int contar, int indexPagina, int tamanhoPagina)
    {
        PaginaIndex = indexPagina;
        // Calcula o número de páginas necessárias para exibir todos os items
        TotalPaginas = (int)Math.Ceiling(contar / (double)tamanhoPagina);
        
        // Adiciona os items desta página à lista
        this.AddRange(items);
    }

    /// <summary>
    /// Propriedade que indica se existem páginas anteriores
    /// </summary>
    public bool HaPaginasAntes
    {
        get { return PaginaIndex > 1; }
    }

    /// <summary>
    /// Propriedade que indica se existem páginas posteriores
    /// </summary>
    public bool HaPaginasDepois
    {
        get { return PaginaIndex < TotalPaginas; }
    }

    /// <summary>
    /// Método assíncrono estático para criar uma página de dados
    /// 
    /// Processo:
    /// 1. Conta o número total de items na source
    /// 2. Salta (skip) os items das páginas anteriores
    /// 3. Pega (take) apenas os items desta página
    /// 4. Converte para lista e cria instância PaginaList
    /// </summary>
    public static async Task<PaginaList<T>> CreateAsync(
        IQueryable<T> source,
        int indexPagina,
        int tamanhoPagina)
    {
        // Conta total de items (executado no BD)
        var contar = await source.CountAsync();
        
        // Pega os items desta página específica (executado no BD)
        // Skip: Salta (indexPagina - 1) * tamanhoPagina items
        // Take: Pega os próximos tamanhoPagina items
        var items = await source
            .Skip((indexPagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();
        
        // Retorna a página criada com os dados
        return new PaginaList<T>(items, contar, indexPagina, tamanhoPagina);
    }
}
