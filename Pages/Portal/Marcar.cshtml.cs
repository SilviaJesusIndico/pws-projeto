// ============================================================================
// PORTAL/MARCAR.CSHTML.CS - Código por trás da Página de Marcação
// ============================================================================
//
// Este ficheiro contém toda a lógica C# para o processo de marcação de serviços.
//
// Responsabilidades:
// - Carregar lista de serviços disponíveis para o dropdown
// - Validar dados do formulário
// - Criar ou reutilizar cliente existente (baseado no email)
// - Criar ou reutilizar animal existente (baseado no nome + cliente)
// - Criar novo agendamento na base de dados
// - Mostrar mensagem de sucesso após marcação
//
// Fluxo de Dados:
// 1. OnGetAsync: Carrega serviços disponíveis e pré-seleciona se servicoId foi passado
// 2. OnPostAsync: Valida, cria registos necessários e guarda agendamento
//
// Padrão utilizado: PageModel (Razor Pages) com [BindProperty]
// Base de dados: SQLite via Entity Framework Core
// ============================================================================

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.Models;

namespace PetShop.Pages.Portal
{
    /// <summary>
    /// PageModel para a página de marcação de serviços
    /// Permite que clientes públicos agendem serviços para os seus pets
    /// </summary>
    public class MarcarModel : PageModel
    {
        // ===== DEPENDÊNCIAS =====
        // Contexto da base de dados para operações CRUD
        private readonly PetShopContext _context;

        /// <summary>
        /// Construtor - Recebe o contexto da BD via Dependency Injection
        /// </summary>
        public MarcarModel(PetShopContext context)
        {
            _context = context;
        }

        // ===== PROPRIEDADES VINCULADAS AO FORMULÁRIO =====

        /// <summary>
        /// Dados do formulário vinculados via [BindProperty]
        /// Permite que os dados sejam automaticamente preenchidos no POST
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = new();

        /// <summary>
        /// Lista de serviços disponíveis para o dropdown
        /// Carregada em cada request (GET e POST)
        /// </summary>
        public SelectList ServicosDisponiveis { get; set; } = null!;
        
        /// <summary>
        /// Flag que indica se a marcação foi bem-sucedida
        /// Usada para mostrar mensagem de confirmação na view
        /// </summary>
        public bool Sucesso { get; set; }

        // ===== CLASSE DE INPUT =====

        /// <summary>
        /// Modelo de dados do formulário com validações
        /// Contém todos os campos necessários para criar uma marcação
        /// </summary>
        public class InputModel
        {
            // ----- DADOS DO CLIENTE -----
            
            /// <summary>
            /// Nome completo do cliente/proprietário
            /// </summary>
            [Required(ErrorMessage = "O nome é obrigatório")]
            [Display(Name = "Nome Completo")]
            public string NomeCliente { get; set; } = string.Empty;

            /// <summary>
            /// Email do cliente - usado para identificar clientes existentes
            /// </summary>
            [Required(ErrorMessage = "O email é obrigatório")]
            [EmailAddress(ErrorMessage = "Email inválido")]
            public string Email { get; set; } = string.Empty;

            /// <summary>
            /// Telefone de contacto do cliente
            /// </summary>
            [Required(ErrorMessage = "O telefone é obrigatório")]
            [Phone(ErrorMessage = "Telefone inválido")]
            public string Telefone { get; set; } = string.Empty;

            // ----- DADOS DO ANIMAL -----

            /// <summary>
            /// Nome do animal de estimação
            /// </summary>
            [Required(ErrorMessage = "O nome do animal é obrigatório")]
            [Display(Name = "Nome do Animal")]
            public string NomeAnimal { get; set; } = string.Empty;

            /// <summary>
            /// Espécie do animal (Cão, Gato, Ave, etc.)
            /// </summary>
            [Required(ErrorMessage = "A espécie é obrigatória")]
            [Display(Name = "Espécie")]
            public string Especie { get; set; } = string.Empty;

            /// <summary>
            /// Raça do animal (opcional)
            /// </summary>
            [Display(Name = "Raça")]
            public string? Raca { get; set; }

            /// <summary>
            /// Idade aproximada do animal em anos (opcional)
            /// </summary>
            [Display(Name = "Idade")]
            public int? Idade { get; set; }

            // ----- SERVIÇO E DATA -----

            /// <summary>
            /// ID do serviço selecionado
            /// Referencia a tabela Servico
            /// </summary>
            [Required(ErrorMessage = "Selecione um serviço")]
            [Display(Name = "Serviço")]
            public int ServicoId { get; set; }

            /// <summary>
            /// Data e hora preferidas para o agendamento
            /// Por defeito: amanhã à mesma hora
            /// </summary>
            [Required(ErrorMessage = "A data é obrigatória")]
            [Display(Name = "Data Preferida")]
            public DateTime DataPreferida { get; set; } = DateTime.Now.AddDays(1);

            /// <summary>
            /// Observações ou notas especiais (opcional)
            /// Ex: "Animal nervoso", "Alergia a produtos X"
            /// </summary>
            [Display(Name = "Observações")]
            public string? Observacoes { get; set; }
        }

        // ===== MÉTODOS HTTP =====

        /// <summary>
        /// Método GET - Carrega a página de marcação
        /// Pode receber servicoId como parâmetro para pré-selecionar um serviço
        /// </summary>
        /// <param name="servicoId">ID do serviço a pré-selecionar (opcional)</param>
        public async Task OnGetAsync(int? servicoId)
        {
            // Carregar lista de serviços para o dropdown
            await CarregarServicos();
            
            // Se um servicoId foi passado na URL, pré-selecionar
            // Útil quando o utilizador clica "Marcar Este Serviço" na lista
            if (servicoId.HasValue)
            {
                Input.ServicoId = servicoId.Value;
            }
        }

        /// <summary>
        /// Método POST - Processa o formulário de marcação
        /// Cria cliente, animal e agendamento na base de dados
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            // Recarregar serviços (necessário para repopular dropdown em caso de erro)
            await CarregarServicos();

            // Validar modelo - se inválido, retorna à página com erros
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // ===== PASSO 1: VERIFICAR OU CRIAR CLIENTE =====
            // Procura cliente existente pelo email
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Email == Input.Email);

            // Se não existe, criar novo cliente
            if (cliente == null)
            {
                cliente = new Cliente
                {
                    Nome = Input.NomeCliente,
                    // Apelido: usa a última palavra do nome ou o nome todo
                    Apelido = Input.NomeCliente.Split(' ').LastOrDefault() ?? Input.NomeCliente,
                    Email = Input.Email,
                    Telefone = Input.Telefone
                };
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync(); // Guardar para obter o ID
            }

            // ===== PASSO 2: VERIFICAR OU CRIAR ANIMAL =====
            // Procura animal existente com mesmo nome e dono
            var animal = await _context.Animais
                .FirstOrDefaultAsync(a => a.Nome == Input.NomeAnimal && a.ClienteID == cliente.ClienteID);

            // Se não existe, criar novo animal
            if (animal == null)
            {
                animal = new Animal
                {
                    Nome = Input.NomeAnimal,
                    Especie = Input.Especie,
                    Raca = Input.Raca ?? string.Empty, // Evitar null
                    ClienteID = cliente.ClienteID
                };
                _context.Animais.Add(animal);
                await _context.SaveChangesAsync(); // Guardar para obter o ID
            }

            // ===== PASSO 3: CRIAR AGENDAMENTO =====
            var agendamento = new Agendamento
            {
                AnimalID = animal.AnimalID,           // Referência ao animal
                ServicoID = Input.ServicoId,          // Referência ao serviço
                DataHora = Input.DataPreferida,       // Data/hora pretendida
                Status = "Pendente",                   // Status inicial
                Observacoes = Input.Observacoes ?? string.Empty  // Evitar null
            };

            _context.Agendamentos.Add(agendamento);
            await _context.SaveChangesAsync();

            // ===== PASSO 4: MOSTRAR SUCESSO =====
            Sucesso = true;
            return Page();
        }

        // ===== MÉTODOS AUXILIARES =====

        /// <summary>
        /// Carrega a lista de serviços para o dropdown
        /// Formata como "Nome do Serviço - Preço"
        /// </summary>
        private async Task CarregarServicos()
        {
            // Buscar todos os serviços ordenados por nome
            var servicos = await _context.Servicos
                .OrderBy(s => s.NomeServico)
                .ToListAsync();

            // Criar SelectList para o dropdown
            // Formato: "Banho e Tosa - 25,00 €"
            ServicosDisponiveis = new SelectList(
                servicos.Select(s => new {
                    s.ServicoID,
                    Display = $"{s.NomeServico} - {s.Preco:C}"
                }),
                "ServicoID",  // Valor do option
                "Display"     // Texto visível do option
            );
        }
    }
}
