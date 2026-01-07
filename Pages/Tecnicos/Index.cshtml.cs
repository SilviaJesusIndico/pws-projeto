using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.Models;

namespace PetShop.Pages.Tecnicos
{
    /// <summary>
    /// PageModel para listar todos os técnicos da PetShop.
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

        public PaginaList<Tecnico> Tecnicos { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? FiltroAtual { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? OrdenacaoAtual { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? MostrarInativos { get; set; }

        /// <summary>
        /// Carrega a lista de técnicos com filtros, ordenação e paginação.
        /// </summary>
        public async Task OnGetAsync(int? indexPagina)
        {
            IQueryable<Tecnico> tecnicosQuery = _context.Tecnicos;

            // Filtrar por ativos/inativos
            if (!MostrarInativos.GetValueOrDefault())
            {
                tecnicosQuery = tecnicosQuery.Where(t => t.Ativo);
            }

            // Filtrar por nome, email ou especialização
            if (!string.IsNullOrEmpty(FiltroAtual))
            {
                tecnicosQuery = tecnicosQuery.Where(t =>
                    t.Nome.Contains(FiltroAtual) ||
                    t.Apelido.Contains(FiltroAtual) ||
                    t.Email.Contains(FiltroAtual) ||
                    (t.Especializacao != null && t.Especializacao.Contains(FiltroAtual))
                );
            }

            // Aplicar ordenação
            tecnicosQuery = OrdenacaoAtual switch
            {
                "nome_desc" => tecnicosQuery.OrderByDescending(t => t.Nome),
                "especializacao" => tecnicosQuery.OrderBy(t => t.Especializacao),
                "especializacao_desc" => tecnicosQuery.OrderByDescending(t => t.Especializacao),
                "data" => tecnicosQuery.OrderBy(t => t.DataContratacao),
                "data_desc" => tecnicosQuery.OrderByDescending(t => t.DataContratacao),
                _ => tecnicosQuery.OrderBy(t => t.Nome),
            };

            int numeroPagina = indexPagina ?? 1;
            Tecnicos = await PaginaList<Tecnico>.CreateAsync(tecnicosQuery.AsNoTracking(), numeroPagina, 10);
        }
    }

    /// <summary>
    /// PageModel para criar um novo técnico.
    /// </summary>
    public class CreateModel : PageModel
    {
        private readonly PetShopContext _context;

        public CreateModel(PetShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tecnico Tecnico { get; set; } = default!;

        public IActionResult OnGet()
        {
            // Define valores padrão
            Tecnico = new Tecnico
            {
                Nome = string.Empty,
                Apelido = string.Empty,
                Email = string.Empty,
                Especializacao = string.Empty,
                DataContratacao = DateTime.Now,
                Ativo = true
            };
            return Page();
        }

        /// <summary>
        /// Processa a criação de um novo técnico.
        /// Valida os dados antes de adicionar à base de dados.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Tecnicos.Add(Tecnico);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }

    /// <summary>
    /// PageModel para editar um técnico existente.
    /// </summary>
    public class EditModel : PageModel
    {
        private readonly PetShopContext _context;

        public EditModel(PetShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tecnico Tecnico { get; set; } = default!;

        /// <summary>
        /// Carrega os dados do técnico a editar.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tecnico = await _context.Tecnicos.FirstOrDefaultAsync(t => t.TecnicoID == id);
            if (tecnico == null)
            {
                return NotFound();
            }

            Tecnico = tecnico;
            return Page();
        }

        /// <summary>
        /// Processa a atualização do técnico.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Tecnico).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TecnicoExists(Tecnico.TecnicoID))
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

        private bool TecnicoExists(int id)
        {
            return _context.Tecnicos.Any(t => t.TecnicoID == id);
        }
    }

    /// <summary>
    /// PageModel para eliminar um técnico.
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
        public Tecnico Tecnico { get; set; } = default!;

        public int TotalAgendamentos { get; set; }

        /// <summary>
        /// Carrega o técnico e conta quantos agendamentos estão associados.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tecnico = await _context.Tecnicos
                .Include(t => t.Agendamentos)
                .FirstOrDefaultAsync(t => t.TecnicoID == id);

            if (tecnico == null)
            {
                return NotFound();
            }

            Tecnico = tecnico;
            TotalAgendamentos = tecnico.Agendamentos?.Count ?? 0;
            return Page();
        }

        /// <summary>
        /// Processa a eliminação do técnico.
        /// Nota: Agendamentos associados ficarão com TecnicoID NULL (SetNull behavior).
        /// </summary>
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tecnico = await _context.Tecnicos.FindAsync(id);
            if (tecnico != null)
            {
                _context.Tecnicos.Remove(tecnico);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }

    /// <summary>
    /// PageModel para mostrar os detalhes de um técnico.
    /// Inclui histórico de agendamentos e especializações.
    /// </summary>
    public class DetailsModel : PageModel
    {
        private readonly PetShopContext _context;

        public DetailsModel(PetShopContext context)
        {
            _context = context;
        }

        public Tecnico Tecnico { get; set; } = default!;
        public int TotalAgendamentos { get; set; }
        public int AgendamentosConluidos { get; set; }

        /// <summary>
        /// Carrega o técnico com histórico de agendamentos.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tecnico = await _context.Tecnicos
                .Include(t => t.Agendamentos)
                    .ThenInclude(a => a.Servico)
                .Include(t => t.Agendamentos)
                    .ThenInclude(a => a.Animal)
                        .ThenInclude(an => an!.Cliente)
                .FirstOrDefaultAsync(t => t.TecnicoID == id);

            if (tecnico == null)
            {
                return NotFound();
            }

            Tecnico = tecnico;
            TotalAgendamentos = tecnico.Agendamentos?.Count ?? 0;
            AgendamentosConluidos = tecnico.Agendamentos?.Count(a => a.Status == "Concluído") ?? 0;

            return Page();
        }
    }
}
