using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.Models;

namespace PetShop.Pages.Animais
{
    /// <summary>
    /// PageModel para listar todos os animais com suporte a paginação, filtros e ordenação.
    /// Esta classe implementa o padrão de listagem usado em todas as entidades do sistema.
    /// </summary>
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly PetShopContext _context;

        // Construtor com injeção de dependência do DbContext
        public IndexModel(PetShopContext context)
        {
            _context = context;
        }

        // Propriedade para armazenar a lista paginada de animais
        public PaginaList<Animal> Animais { get; set; } = default!;

        // Parâmetros de filtro e ordenação vindos da query string
        [BindProperty(SupportsGet = true)]
        public string? FiltroAtual { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? OrdenacaoAtual { get; set; }

        /// <summary>
        /// Método executado ao carregar a página de listagem.
        /// Implementa filtros por nome/espécie/raça, ordenação e paginação.
        /// </summary>
        public async Task OnGetAsync(int? indexPagina)
        {
            // Criamos uma query inicial que inclui os dados do cliente (proprietário)
            IQueryable<Animal> animaisQuery = _context.Animais
                .Include(a => a.Cliente); // Include carrega a relação Cliente automaticamente

            // Aplicar filtro se existir (procura em Nome, Espécie ou Raça)
            if (!string.IsNullOrEmpty(FiltroAtual))
            {
                animaisQuery = animaisQuery.Where(a =>
                    a.Nome.Contains(FiltroAtual) ||
                    a.Especie.Contains(FiltroAtual) ||
                    (a.Raca != null && a.Raca.Contains(FiltroAtual))
                );
            }

            // Aplicar ordenação baseada no parâmetro da query string
            animaisQuery = OrdenacaoAtual switch
            {
                "nome_desc" => animaisQuery.OrderByDescending(a => a.Nome),
                "especie" => animaisQuery.OrderBy(a => a.Especie),
                "especie_desc" => animaisQuery.OrderByDescending(a => a.Especie),
                "cliente" => animaisQuery.OrderBy(a => a.Cliente!.Nome),
                "cliente_desc" => animaisQuery.OrderByDescending(a => a.Cliente!.Nome),
                "data" => animaisQuery.OrderBy(a => a.DataNascimento),
                "data_desc" => animaisQuery.OrderByDescending(a => a.DataNascimento),
                _ => animaisQuery.OrderBy(a => a.Nome), // Ordenação padrão
            };

            // Criar lista paginada (5 animais por página)
            int numeroPagina = indexPagina ?? 1;
            Animais = await PaginaList<Animal>.CreateAsync(animaisQuery.AsNoTracking(), numeroPagina, 5);
        }
    }

    /// <summary>
    /// PageModel para criar um novo animal.
    /// Carrega a lista de clientes para o dropdown.
    /// </summary>
    public class CreateModel : PageModel
    {
        private readonly PetShopContext _context;

        public CreateModel(PetShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Animal Animal { get; set; } = default!;

        // Lista de clientes para preencher o dropdown
        public List<Cliente> Clientes { get; set; } = new();

        /// <summary>
        /// Carrega a lista de clientes para o dropdown ao abrir o formulário.
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
            Clientes = await _context.Clientes.OrderBy(c => c.Nome).ToListAsync();
            return Page();
        }

        /// <summary>
        /// Processa a submissão do formulário de criação.
        /// Valida os dados e adiciona o animal à base de dados.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            // Verifica se os dados do formulário são válidos
            if (!ModelState.IsValid)
            {
                // Se inválido, recarrega a lista de clientes e mostra erros
                Clientes = await _context.Clientes.OrderBy(c => c.Nome).ToListAsync();
                return Page();
            }

            // Adiciona o animal ao contexto
            _context.Animais.Add(Animal);
            
            // Salva as alterações na base de dados
            await _context.SaveChangesAsync();

            // Redireciona para a página de listagem
            return RedirectToPage("./Index");
        }
    }

    /// <summary>
    /// PageModel para editar um animal existente.
    /// </summary>
    public class EditModel : PageModel
    {
        private readonly PetShopContext _context;

        public EditModel(PetShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Animal Animal { get; set; } = default!;

        public List<Cliente> Clientes { get; set; } = new();

        /// <summary>
        /// Carrega os dados do animal a editar e a lista de clientes.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Busca o animal pelo ID
            var animal = await _context.Animais.FirstOrDefaultAsync(a => a.AnimalID == id);
            if (animal == null)
            {
                return NotFound();
            }

            Animal = animal;
            Clientes = await _context.Clientes.OrderBy(c => c.Nome).ToListAsync();
            return Page();
        }

        /// <summary>
        /// Processa a atualização do animal.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Clientes = await _context.Clientes.OrderBy(c => c.Nome).ToListAsync();
                return Page();
            }

            // Marca a entidade como modificada
            _context.Attach(Animal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Verifica se o animal ainda existe
                if (!AnimalExists(Animal.AnimalID))
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

        private bool AnimalExists(int id)
        {
            return _context.Animais.Any(a => a.AnimalID == id);
        }
    }

    /// <summary>
    /// PageModel para eliminar um animal.
    /// Mostra confirmação antes de eliminar.
    /// </summary>
    public class DeleteModel : PageModel
    {
        private readonly PetShopContext _context;

        public DeleteModel(PetShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Animal Animal { get; set; } = default!;

        /// <summary>
        /// Carrega os dados do animal a eliminar, incluindo o cliente.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Include para mostrar dados do cliente
            var animal = await _context.Animais
                .Include(a => a.Cliente)
                .FirstOrDefaultAsync(a => a.AnimalID == id);

            if (animal == null)
            {
                return NotFound();
            }

            Animal = animal;
            return Page();
        }

        /// <summary>
        /// Processa a eliminação do animal.
        /// Nota: Agendamentos relacionados também serão eliminados (cascade).
        /// </summary>
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animais.FindAsync(id);
            if (animal != null)
            {
                _context.Animais.Remove(animal);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }

    /// <summary>
    /// PageModel para mostrar os detalhes de um animal.
    /// Inclui a lista de agendamentos do animal.
    /// </summary>
    public class DetailsModel : PageModel
    {
        private readonly PetShopContext _context;

        public DetailsModel(PetShopContext context)
        {
            _context = context;
        }

        public Animal Animal { get; set; } = default!;

        /// <summary>
        /// Carrega todos os dados do animal incluindo cliente e agendamentos.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Include para carregar Cliente e Agendamentos com Servicos
            var animal = await _context.Animais
                .Include(a => a.Cliente)
                .Include(a => a.Agendamentos)
                    .ThenInclude(ag => ag.Servico)
                .FirstOrDefaultAsync(a => a.AnimalID == id);

            if (animal == null)
            {
                return NotFound();
            }

            Animal = animal;
            return Page();
        }
    }
}
