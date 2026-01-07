using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.Models;

namespace PetShop.Pages.Clientes;

[Authorize]

/// <summary>
/// PageModel: Clientes - Index
/// 
/// Responsabilidades:
/// - Listar todos os clientes do sistema
/// - Implementar filtro por nome/email
/// - Implementar paginação (exemplo: 5 clientes por página)
/// - Ordenação por diferentes campos
/// 
/// Ligações de autorização:
/// - PÚBLICO: Qualquer pessoa pode ver a lista de clientes
/// 
/// Métodos principais:
/// - OnGetAsync(): Carrega a página, aplica filtros e ordenação
/// </summary>
public class IndexModel : PageModel
{
    /// <summary>
    /// DbContext injetado automaticamente pelo ASP.NET Core
    /// Utilizado para aceder à base de dados
    /// </summary>
    private readonly PetShop.Data.PetShopContext _context;

    /// <summary>
    /// Construtor que recebe o DbContext via injeção de dependência
    /// </summary>
    public IndexModel(PetShop.Data.PetShopContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Propriedade de ligação para ordenação por nome
    /// </summary>
    public string? OrdenacaoNome { get; set; }

    /// <summary>
    /// Propriedade para guardar ordenação actual
    /// </summary>
    public string? OrdenacaoCorrente { get; set; }

    /// <summary>
    /// Propriedade para guardar filtro actual (texto de busca)
    /// </summary>
    public string? FiltroCorrente { get; set; }

    /// <summary>
    /// Lista de clientes para exibição
    /// Utiliza PaginaList para implementar paginação
    /// </summary>
    public PaginaList<Cliente> Clientes { get; set; } = default!;

    /// <summary>
    /// Método chamado quando a página é acedida via GET
    /// 
    /// Parâmetros:
    /// - ordenacao: Campo para ordenar ("nome_desc", "email", etc.)
    /// - filtroTexto: Texto para filtrar clientes
    /// - indexPagina: Número da página a exibir (padrão: 1)
    /// </summary>
    public async Task OnGetAsync(
        string ordenacao,
        string filtroTexto,
        string FiltroCorrente,
        int? indexPagina)
    {
        // Define ordenação padrão: nome crescente ou decrescente
        OrdenacaoCorrente = ordenacao;
        OrdenacaoNome = string.IsNullOrEmpty(ordenacao) ? "nome_desc" : "";

        // Se novo filtro, volta para página 1
        if (filtroTexto != null)
        {
            indexPagina = 1;
        }
        else
        {
            // Mantém o filtro anterior se não houver novo
            filtroTexto = FiltroCorrente;
        }

        FiltroCorrente = filtroTexto;

        // Query base: todos os clientes
        IQueryable<Cliente> clienteIQ = from s in _context.Clientes select s;

        // Aplicar filtro por texto (nome ou email)
        if (!String.IsNullOrEmpty(filtroTexto))
        {
            clienteIQ = clienteIQ.Where(s =>
                s.Nome.Contains(filtroTexto) ||
                s.Apelido.Contains(filtroTexto) ||
                s.Email.Contains(filtroTexto)
            );
        }

        // Aplicar ordenação
        switch (ordenacao)
        {
            case "nome_desc":
                clienteIQ = clienteIQ.OrderByDescending(s => s.Nome);
                break;
            case "email":
                clienteIQ = clienteIQ.OrderBy(s => s.Email);
                break;
            case "email_desc":
                clienteIQ = clienteIQ.OrderByDescending(s => s.Email);
                break;
            default:
                clienteIQ = clienteIQ.OrderBy(s => s.Nome);
                break;
        }

        // Aplicar paginação: 5 clientes por página
        int tamanhoPagina = 5;
        Clientes = await PaginaList<Cliente>.CreateAsync(
            clienteIQ.AsNoTracking(),
            indexPagina ?? 1,
            tamanhoPagina
        );
    }
}
