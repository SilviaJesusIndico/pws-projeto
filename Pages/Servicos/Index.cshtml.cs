using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.Models;

namespace PetShop.Pages.Servicos
{
    /// <summary>
    /// PageModel para listar todos os serviços oferecidos pela PetShop.
    /// Implementa paginação, filtros e ordenação.
    /// </summary>
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly PetShopContext _context;

        public IndexModel(PetShopContext context)
        {
            _context = context;
        }

        public PaginaList<Servico> Servicos { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? FiltroAtual { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? OrdenacaoAtual { get; set; }

        /// <summary>
        /// Carrega a lista de serviços com filtros, ordenação e paginação.
        /// </summary>
        public async Task OnGetAsync(int? indexPagina)
        {
            IQueryable<Servico> servicosQuery = _context.Servicos;

            // Filtrar por nome ou descrição do serviço
            if (!string.IsNullOrEmpty(FiltroAtual))
            {
                servicosQuery = servicosQuery.Where(s =>
                    s.NomeServico.Contains(FiltroAtual) ||
                    (s.Descricao != null && s.Descricao.Contains(FiltroAtual))
                );
            }

            // Aplicar ordenação
            servicosQuery = OrdenacaoAtual switch
            {
                "nome_desc" => servicosQuery.OrderByDescending(s => s.NomeServico),
                "preco" => servicosQuery.OrderBy(s => s.Preco),
                "preco_desc" => servicosQuery.OrderByDescending(s => s.Preco),
                "duracao" => servicosQuery.OrderBy(s => s.DuracaoMinutos),
                "duracao_desc" => servicosQuery.OrderByDescending(s => s.DuracaoMinutos),
                _ => servicosQuery.OrderBy(s => s.NomeServico),
            };

            int numeroPagina = indexPagina ?? 1;
            Servicos = await PaginaList<Servico>.CreateAsync(servicosQuery.AsNoTracking(), numeroPagina, 8);
        }
    }

    /// <summary>
    /// PageModel para criar um novo serviço.
    /// </summary>
    public class CreateModel : PageModel
    {
        private readonly PetShopContext _context;

        public CreateModel(PetShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Servico Servico { get; set; } = default!;

        public IActionResult OnGet()
        {
            return Page();
        }

        /// <summary>
        /// Processa a criação de um novo serviço.
        /// Valida os dados antes de adicionar à base de dados.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Servicos.Add(Servico);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }

    /// <summary>
    /// PageModel para editar um serviço existente.
    /// </summary>
    public class EditModel : PageModel
    {
        private readonly PetShopContext _context;

        public EditModel(PetShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Servico Servico { get; set; } = default!;

        /// <summary>
        /// Carrega os dados do serviço a editar.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servico = await _context.Servicos.FirstOrDefaultAsync(s => s.ServicoID == id);
            if (servico == null)
            {
                return NotFound();
            }

            Servico = servico;
            return Page();
        }

        /// <summary>
        /// Processa a atualização do serviço.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Servico).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServicoExists(Servico.ServicoID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ServicoExists(int id)
        {
            return _context.Servicos.Any(s => s.ServicoID == id);
        }
    }

    /// <summary>
    /// PageModel para eliminar um serviço.
    /// Verifica se existem agendamentos antes de permitir a eliminação.
    /// </summary>
    public class DeleteModel : PageModel
    {
        private readonly PetShopContext _context;

        public DeleteModel(PetShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Servico Servico { get; set; } = default!;

        public int TotalAgendamentos { get; set; }

        /// <summary>
        /// Carrega o serviço e conta quantos agendamentos estão associados.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servico = await _context.Servicos
                .Include(s => s.Agendamentos)
                .FirstOrDefaultAsync(s => s.ServicoID == id);

            if (servico == null)
            {
                return NotFound();
            }

            Servico = servico;
            TotalAgendamentos = servico.Agendamentos?.Count ?? 0;
            return Page();
        }

        /// <summary>
        /// Processa a eliminação do serviço.
        /// Nota: Se houver agendamentos, eles ficarão com ServicoID NULL (SetNull behavior).
        /// </summary>
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servico = await _context.Servicos.FindAsync(id);
            if (servico != null)
            {
                _context.Servicos.Remove(servico);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }

    /// <summary>
    /// PageModel para mostrar os detalhes de um serviço.
    /// Inclui estatísticas de agendamentos.
    /// </summary>
    public class DetailsModel : PageModel
    {
        private readonly PetShopContext _context;

        public DetailsModel(PetShopContext context)
        {
            _context = context;
        }

        public Servico Servico { get; set; } = default!;
        public int TotalAgendamentos { get; set; }
        public decimal? ReceitaTotal { get; set; }

        /// <summary>
        /// Carrega o serviço com estatísticas de uso.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servico = await _context.Servicos
                .Include(s => s.Agendamentos)
                .FirstOrDefaultAsync(s => s.ServicoID == id);

            if (servico == null)
            {
                return NotFound();
            }

            Servico = servico;
            TotalAgendamentos = servico.Agendamentos?.Count ?? 0;
            
            // Calcula receita total gerada por este serviço
            ReceitaTotal = servico.Agendamentos?
                .Where(a => a.PrecoFinal.HasValue)
                .Sum(a => a.PrecoFinal);

            return Page();
        }
    }
}
