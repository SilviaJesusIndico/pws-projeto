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

/// <summary>
/// PageModel: Clientes - Create
/// 
/// Responsabilidades:
/// - Exibir formulário para criar novo cliente
/// - Validar dados do cliente
/// - Guardar novo cliente na base de dados
/// 
/// Métodos:
/// - OnGet(): Exibe formulário em branco
/// - OnPostAsync(): Processa submissão do formulário
/// </summary>
[Authorize]
public class CreateModel : PageModel
{
    private readonly PetShop.Data.PetShopContext _context;

    public CreateModel(PetShop.Data.PetShopContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Inicializa a página de criação
    /// </summary>
    public IActionResult OnGet()
    {
        return Page();
    }

    /// <summary>
    /// Cliente vinculado automaticamente ao modelo
    /// [BindProperty] permite binding automático do formulário
    /// </summary>
    [BindProperty]
    public Cliente Cliente { get; set; } = default!;

    /// <summary>
    /// Processa o POST do formulário de criação
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        // Validar modelo
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Guardar data de registo como hoje
        Cliente.DataRegisto = DateTime.Now;

        // Adicionar à base de dados
        _context.Clientes.Add(Cliente);
        await _context.SaveChangesAsync();

        // Redirecionar para a lista de clientes
        return RedirectToPage("./Index");
    }
}

/// <summary>
/// PageModel: Clientes - Edit
/// 
/// Responsabilidades:
/// - Exibir formulário com dados do cliente existente
/// - Permitir editar os dados
/// - Guardar alterações na base de dados
/// 
/// Métodos:
/// - OnGetAsync(id): Carrega dados do cliente
/// - OnPostAsync(): Processa submissão do formulário editado
/// </summary>
[Authorize]
public class EditModel : PageModel
{
    private readonly PetShop.Data.PetShopContext _context;

    public EditModel(PetShop.Data.PetShopContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Cliente Cliente { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var cliente = await _context.Clientes.FirstOrDefaultAsync(m => m.ClienteID == id);
        if (cliente == null)
        {
            return NotFound();
        }
        Cliente = cliente;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Cliente).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClienteExists(Cliente.ClienteID))
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

    private bool ClienteExists(int id)
    {
        return _context.Clientes.Any(e => e.ClienteID == id);
    }
}

/// <summary>
/// PageModel: Clientes - Delete
/// 
/// Responsabilidades:
/// - Exibir página de confirmação de eliminação
/// - Permitir confirmar ou cancelar a eliminação
/// - Guardar eliminação na base de dados
/// 
/// Métodos:
/// - OnGetAsync(id): Carrega dados do cliente a eliminar
/// - OnPostAsync(id): Processa confirmação da eliminação
/// </summary>
[Authorize]
public class DeleteModel : PageModel
{
    private readonly PetShop.Data.PetShopContext _context;

    public DeleteModel(PetShop.Data.PetShopContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Cliente Cliente { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var cliente = await _context.Clientes.FirstOrDefaultAsync(m => m.ClienteID == id);
        if (cliente == null)
        {
            return NotFound();
        }
        else
        {
            Cliente = cliente;
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente != null)
        {
            Cliente = cliente;
            _context.Clientes.Remove(Cliente);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}

/// <summary>
/// PageModel: Clientes - Details
/// 
/// Responsabilidades:
/// - Exibir detalhes completos de um cliente
/// - Mostrar lista de animais do cliente
/// - Mostrar histórico de agendamentos
/// 
/// Métodos:
/// - OnGetAsync(id): Carrega dados do cliente com relacionamentos
/// </summary>
[Authorize]
public class DetailsModel : PageModel
{
    private readonly PetShop.Data.PetShopContext _context;

    public DetailsModel(PetShop.Data.PetShopContext context)
    {
        _context = context;
    }

    public Cliente Cliente { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // Carrega cliente com os seus animais
        var cliente = await _context.Clientes
            .Include(c => c.Animais)
            .FirstOrDefaultAsync(m => m.ClienteID == id);

        if (cliente == null)
        {
            return NotFound();
        }
        else
        {
            Cliente = cliente;
        }
        return Page();
    }
}
