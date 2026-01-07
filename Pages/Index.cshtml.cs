/// ============================================================================
/// INDEX.CSHTML.CS - Página Inicial da Aplicação
/// ============================================================================
/// 
/// ALTERAÇÃO REALIZADA (05/01/2026):
/// - O método OnGet() foi modificado para redirecionar automaticamente
///   para a página do Portal do cliente (/Portal/Index)
/// - Quando o utilizador acede a http://localhost:5234/ é redirecionado
///   para a página do Portal do utilizador
/// 
/// Motivo: Melhorar a experiência do utilizador, apresentando
/// diretamente a página de serviços do PetShop
/// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PetShop.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// ALTERADO: Redireciona para a página do Portal do cliente
    /// Em vez de mostrar a página Index, redireciona para /Portal/Index
    /// </summary>
    public IActionResult OnGet()
    {
        return RedirectToPage("/Portal/Index");
    }
}
