using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.Models;

namespace PetShop.Pages.Agendamentos
{
    /// <summary>
    /// PageModel para listar todos os agendamentos.
    /// Implementa paginação, filtros por status e data, e ordenação.
    /// </summary>
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly PetShopContext _context;

        public IndexModel(PetShopContext context)
        {
            _context = context;
        }

        public PaginaList<Agendamento> Agendamentos { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? FiltroStatus { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? OrdenacaoAtual { get; set; }

        /// <summary>
        /// Carrega a lista de agendamentos com filtros, ordenação e paginação.
        /// Inclui dados relacionados (Animal, Cliente, Servico, Tecnico).
        /// </summary>
        public async Task OnGetAsync(int? indexPagina)
        {
            // Query inicial com Includes para carregar dados relacionados
            IQueryable<Agendamento> agendamentosQuery = _context.Agendamentos
                .Include(a => a.Animal)
                    .ThenInclude(an => an!.Cliente)
                .Include(a => a.Servico)
                .Include(a => a.Tecnico);

            // Filtrar por status se especificado
            if (!string.IsNullOrEmpty(FiltroStatus) && FiltroStatus != "Todos")
            {
                agendamentosQuery = agendamentosQuery.Where(a => a.Status == FiltroStatus);
            }

            // Aplicar ordenação
            agendamentosQuery = OrdenacaoAtual switch
            {
                "data_asc" => agendamentosQuery.OrderBy(a => a.DataHora),
                "animal" => agendamentosQuery.OrderBy(a => a.Animal!.Nome),
                "animal_desc" => agendamentosQuery.OrderByDescending(a => a.Animal!.Nome),
                "servico" => agendamentosQuery.OrderBy(a => a.Servico!.NomeServico),
                "servico_desc" => agendamentosQuery.OrderByDescending(a => a.Servico!.NomeServico),
                "status" => agendamentosQuery.OrderBy(a => a.Status),
                "status_desc" => agendamentosQuery.OrderByDescending(a => a.Status),
                _ => agendamentosQuery.OrderByDescending(a => a.DataHora), // Mais recentes primeiro
            };

            int numeroPagina = indexPagina ?? 1;
            Agendamentos = await PaginaList<Agendamento>.CreateAsync(agendamentosQuery.AsNoTracking(), numeroPagina, 10);
        }
    }

    /// <summary>
    /// PageModel para criar um novo agendamento.
    /// Carrega listas de Animais, Serviços e Técnicos para os dropdowns.
    /// </summary>
    public class CreateModel : PageModel
    {
        private readonly PetShopContext _context;

        public CreateModel(PetShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Agendamento Agendamento { get; set; } = default!;

        // SelectList para dropdowns
        public SelectList AnimaisSelectList { get; set; } = default!;
        public SelectList ServicosSelectList { get; set; } = default!;
        public SelectList TecnicosSelectList { get; set; } = default!;

        /// <summary>
        /// Carrega as listas para os dropdowns ao abrir o formulário.
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
            await CarregarSelectLists();
            
            // Define valores padrão
            Agendamento = new Agendamento
            {
                DataHora = DateTime.Now.AddDays(1),
                Status = "Agendado",
                DataCriacao = DateTime.Now
            };
            
            return Page();
        }

        /// <summary>
        /// Processa a criação de um novo agendamento.
        /// Define o preço final baseado no serviço selecionado.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CarregarSelectLists();
                return Page();
            }

            // Define a data de criação
            Agendamento.DataCriacao = DateTime.Now;
            
            // Garante que Observacoes não é NULL (restrição da BD)
            Agendamento.Observacoes ??= "";

            // Se PrecoFinal não foi definido, usa o preço do serviço
            if (!Agendamento.PrecoFinal.HasValue)
            {
                var servico = await _context.Servicos.FindAsync(Agendamento.ServicoID);
                if (servico != null)
                {
                    Agendamento.PrecoFinal = servico.Preco;
                }
            }

            _context.Agendamentos.Add(Agendamento);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Método auxiliar para carregar as SelectLists.
        /// </summary>
        private async Task CarregarSelectLists()
        {
            // Carrega animais com informação do cliente
            var animais = await _context.Animais
                .Include(a => a.Cliente)
                .OrderBy(a => a.Nome)
                .Select(a => new {
                    a.AnimalID,
                    DisplayText = $"{a.Nome} ({a.Especie}) - {(a.Cliente != null ? a.Cliente.NomeCompleto : "Sem cliente")}"
                })
                .ToListAsync();
            
            AnimaisSelectList = new SelectList(animais, "AnimalID", "DisplayText");

            // Carrega serviços
            var servicos = await _context.Servicos
                .OrderBy(s => s.NomeServico)
                .Select(s => new {
                    s.ServicoID,
                    DisplayText = $"{s.NomeServico} - {s.Preco:C} ({s.DuracaoMinutos} min)"
                })
                .ToListAsync();
            
            ServicosSelectList = new SelectList(servicos, "ServicoID", "DisplayText");

            // Carrega técnicos ativos
            var tecnicos = await _context.Tecnicos
                .Where(t => t.Ativo)
                .OrderBy(t => t.Nome)
                .Select(t => new {
                    t.TecnicoID,
                    DisplayText = $"{t.NomeCompleto} - {t.Especializacao}"
                })
                .ToListAsync();
            
            TecnicosSelectList = new SelectList(tecnicos, "TecnicoID", "DisplayText");
        }
    }

    /// <summary>
    /// PageModel para editar um agendamento existente.
    /// </summary>
    public class EditModel : PageModel
    {
        private readonly PetShopContext _context;

        public EditModel(PetShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Agendamento Agendamento { get; set; } = default!;

        public SelectList AnimaisSelectList { get; set; } = default!;
        public SelectList ServicosSelectList { get; set; } = default!;
        public SelectList TecnicosSelectList { get; set; } = default!;

        /// <summary>
        /// Carrega os dados do agendamento a editar.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendamento = await _context.Agendamentos.FirstOrDefaultAsync(a => a.AgendamentoID == id);
            if (agendamento == null)
            {
                return NotFound();
            }

            Agendamento = agendamento;
            await CarregarSelectLists();
            return Page();
        }

        /// <summary>
        /// Processa a atualização do agendamento.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CarregarSelectLists();
                return Page();
            }

            // Garante que Observacoes não é NULL (restrição da BD)
            Agendamento.Observacoes ??= "";

            _context.Attach(Agendamento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgendamentoExists(Agendamento.AgendamentoID))
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

        private bool AgendamentoExists(int id)
        {
            return _context.Agendamentos.Any(a => a.AgendamentoID == id);
        }

        private async Task CarregarSelectLists()
        {
            var animais = await _context.Animais
                .Include(a => a.Cliente)
                .OrderBy(a => a.Nome)
                .Select(a => new {
                    a.AnimalID,
                    DisplayText = $"{a.Nome} ({a.Especie}) - {(a.Cliente != null ? a.Cliente.NomeCompleto : "Sem cliente")}"
                })
                .ToListAsync();
            
            AnimaisSelectList = new SelectList(animais, "AnimalID", "DisplayText");

            var servicos = await _context.Servicos
                .OrderBy(s => s.NomeServico)
                .Select(s => new {
                    s.ServicoID,
                    DisplayText = $"{s.NomeServico} - {s.Preco:C}"
                })
                .ToListAsync();
            
            ServicosSelectList = new SelectList(servicos, "ServicoID", "DisplayText");

            var tecnicos = await _context.Tecnicos
                .Where(t => t.Ativo)
                .OrderBy(t => t.Nome)
                .Select(t => new {
                    t.TecnicoID,
                    DisplayText = $"{t.NomeCompleto} - {t.Especializacao}"
                })
                .ToListAsync();
            
            TecnicosSelectList = new SelectList(tecnicos, "TecnicoID", "DisplayText");
        }
    }

    /// <summary>
    /// PageModel para eliminar um agendamento.
    /// </summary>
    public class DeleteModel : PageModel
    {
        private readonly PetShopContext _context;

        public DeleteModel(PetShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Agendamento Agendamento { get; set; } = default!;

        /// <summary>
        /// Carrega o agendamento com todos os dados relacionados.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendamento = await _context.Agendamentos
                .Include(a => a.Animal)
                    .ThenInclude(an => an!.Cliente)
                .Include(a => a.Servico)
                .Include(a => a.Tecnico)
                .FirstOrDefaultAsync(a => a.AgendamentoID == id);

            if (agendamento == null)
            {
                return NotFound();
            }

            Agendamento = agendamento;
            return Page();
        }

        /// <summary>
        /// Processa a eliminação do agendamento.
        /// </summary>
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendamento = await _context.Agendamentos.FindAsync(id);
            if (agendamento != null)
            {
                _context.Agendamentos.Remove(agendamento);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }

    /// <summary>
    /// PageModel para mostrar os detalhes de um agendamento.
    /// </summary>
    public class DetailsModel : PageModel
    {
        private readonly PetShopContext _context;

        public DetailsModel(PetShopContext context)
        {
            _context = context;
        }

        public Agendamento Agendamento { get; set; } = default!;

        /// <summary>
        /// Carrega o agendamento com todos os dados relacionados.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendamento = await _context.Agendamentos
                .Include(a => a.Animal)
                    .ThenInclude(an => an!.Cliente)
                .Include(a => a.Servico)
                .Include(a => a.Tecnico)
                .FirstOrDefaultAsync(a => a.AgendamentoID == id);

            if (agendamento == null)
            {
                return NotFound();
            }

            Agendamento = agendamento;
            return Page();
        }
    }
}
