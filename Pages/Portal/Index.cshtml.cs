// ============================================================================
// PORTAL/INDEX.CSHTML.CS - Código por trás da Página Principal do Portal
// ============================================================================
//
// Este ficheiro contém a lógica C# para a página inicial do portal do cliente.
//
// Responsabilidades:
// - Carregar os serviços disponíveis da base de dados
// - Limitar a lista a 6 serviços para não sobrecarregar a página
// - Preparar os dados para apresentação na view
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
    /// PageModel para a página inicial do Portal do Cliente
    /// Carrega os serviços principais para apresentar ao utilizador
    /// </summary>
    public class IndexModel : PageModel
    {
        // ===== DEPENDÊNCIAS =====
        // Contexto da base de dados injetado via construtor
        private readonly PetShopContext _context;

        /// <summary>
        /// Construtor - Recebe o contexto da BD via Dependency Injection
        /// </summary>
        /// <param name="context">Contexto do Entity Framework para aceder à BD</param>
        public IndexModel(PetShopContext context)
        {
            _context = context;
        }

        // ===== PROPRIEDADES =====
        
        /// <summary>
        /// Lista de serviços a apresentar na página
        /// Inicializada como lista vazia para evitar NullReferenceException
        /// </summary>
        public List<Servico> Servicos { get; set; } = new();

        // ===== MÉTODOS =====

        /// <summary>
        /// Método executado quando a página é carregada (HTTP GET)
        /// Carrega os 6 principais serviços ordenados por nome
        /// </summary>
        public async Task OnGetAsync()
        {
            // Buscar serviços da base de dados
            // - OrderBy: Ordena alfabeticamente pelo nome
            // - Take(6): Limita a 6 resultados para a página inicial
            // - ToListAsync: Executa a query de forma assíncrona
            Servicos = await _context.Servicos
                .OrderBy(s => s.NomeServico)
                .Take(6) // Mostrar apenas os 6 principais serviços na página inicial
                .ToListAsync();
        }
    }
}
