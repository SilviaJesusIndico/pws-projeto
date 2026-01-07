// ============================================================================
// PORTAL/SERVICOS.CSHTML.CS - Código por trás da Página de Serviços
// ============================================================================
//
// Este ficheiro contém a lógica C# para a página de listagem de serviços.
//
// Responsabilidades:
// - Carregar TODOS os serviços da base de dados (sem limite)
// - Ordenar serviços alfabeticamente
// - Preparar dados para apresentação na view
//
// Diferença do IndexModel:
// - IndexModel carrega apenas 6 serviços (página inicial)
// - ServicosModel carrega todos os serviços (página completa)
//
// Padrão utilizado: PageModel (Razor Pages)
// Base de dados: Acede ao PetShopContext via injeção de dependência
// ============================================================================

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.Models;

namespace PetShop.Pages.Portal
{
    /// <summary>
    /// PageModel para a página de listagem completa de serviços
    /// Apresenta todos os serviços disponíveis na PetShop
    /// </summary>
    public class ServicosModel : PageModel
    {
        // ===== DEPENDÊNCIAS =====
        // Contexto da base de dados injetado via construtor
        private readonly PetShopContext _context;

        /// <summary>
        /// Construtor - Recebe o contexto da BD via Dependency Injection
        /// </summary>
        /// <param name="context">Contexto do Entity Framework para aceder à BD</param>
        public ServicosModel(PetShopContext context)
        {
            _context = context;
        }

        // ===== PROPRIEDADES =====

        /// <summary>
        /// Lista de todos os serviços disponíveis
        /// Carregada da base de dados no método OnGetAsync
        /// </summary>
        public List<Servico> Servicos { get; set; } = new();

        // ===== MÉTODOS =====

        /// <summary>
        /// Método executado quando a página é carregada (HTTP GET)
        /// Carrega todos os serviços ordenados alfabeticamente
        /// </summary>
        public async Task OnGetAsync()
        {
            // Buscar TODOS os serviços da base de dados
            // - OrderBy: Ordena alfabeticamente pelo nome do serviço
            // - ToListAsync: Executa a query de forma assíncrona
            Servicos = await _context.Servicos
                .OrderBy(s => s.NomeServico)
                .ToListAsync();
        }
    }
}
