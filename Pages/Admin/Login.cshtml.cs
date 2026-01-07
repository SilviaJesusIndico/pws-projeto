// ============================================================================
// ADMIN/LOGIN.CSHTML.CS - Código por trás da Página de Login
// ============================================================================
//
// Este ficheiro contém a lógica de autenticação para administradores.
//
// Responsabilidades:
// - Validar credenciais de login
// - Criar cookie de autenticação com Claims
// - Gerir sessão do utilizador (persistência opcional)
// - Redirecionar para área de administração após login
//
// Segurança:
// - Usa Cookie Authentication do ASP.NET Core
// - Claims incluem nome e role (Admin)
// - Expiração configrável (8 horas ou 7 dias se "Lembrar-me")
//
// NOTA: Em produção, as credenciais devem vir de uma base de dados
// com passwords encriptadas (ex: BCrypt, Argon2)
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PetShop.Pages.Admin
{
    /// <summary>
    /// PageModel para a página de Login de Administrador
    /// Gere a autenticação via Cookie Authentication
    /// </summary>
    public class LoginModel : PageModel
    {
        // ===== CREDENCIAIS DE DEMONSTRAÇÃO =====
        // ATENÇÃO: Em produção, usar base de dados com passwords encriptadas!
        private const string AdminUsername = "admin";     // Nome de utilizador
        private const string AdminPassword = "admin123"; // Palavra-passe

        // ===== PROPRIEDADES =====

        /// <summary>
        /// Dados do formulário de login vinculados via [BindProperty]
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = new();

        /// <summary>
        /// Mensagem de erro a mostrar (ex: "Credenciais inválidas")
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// URL para redirecionar após login bem-sucedido
        /// </summary>
        public string? ReturnUrl { get; set; }

        // ===== CLASSE DE INPUT =====

        /// <summary>
        /// Modelo de dados do formulário de login
        /// Contém validações para campos obrigatórios
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// Nome de utilizador para login
            /// </summary>
            [Required(ErrorMessage = "O utilizador é obrigatório")]
            [Display(Name = "Utilizador")]
            public string Username { get; set; } = string.Empty;

            /// <summary>
            /// Palavra-passe do utilizador
            /// </summary>
            [Required(ErrorMessage = "A palavra-passe é obrigatória")]
            [DataType(DataType.Password)]
            [Display(Name = "Palavra-passe")]
            public string Password { get; set; } = string.Empty;

            /// <summary>
            /// Se marcado, mantém a sessão por 7 dias
            /// Se não marcado, sessão expira em 8 horas
            /// </summary>
            [Display(Name = "Lembrar-me")]
            public bool RememberMe { get; set; }
        }

        // ===== MÉTODOS HTTP =====

        /// <summary>
        /// Método GET - Carrega a página de login
        /// Se já autenticado, redireciona para a administração
        /// </summary>
        /// <param name="returnUrl">URL de retorno após login (opcional)</param>
        public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
        {
            // Verificar se o utilizador já está autenticado
            // Se sim, não precisa de fazer login novamente
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Agendamentos/Index");
            }

            ReturnUrl = returnUrl;

            // Limpar qualquer cookie de autenticação existente
            // Garante um estado limpo antes de novo login
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Page();
        }

        /// <summary>
        /// Método POST - Processa o formulário de login
        /// Valida credenciais e cria cookie de autenticação
        /// </summary>
        /// <param name="returnUrl">URL de retorno após login (opcional)</param>
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            // Definir URL de retorno (default: página de agendamentos)
            ReturnUrl = returnUrl ?? Url.Content("~/Agendamentos/Index");

            // Validar modelo - se inválido, retorna com erros
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // ===== VALIDAR CREDENCIAIS =====
            // Compara com as credenciais definidas nas constantes
            if (Input.Username == AdminUsername && Input.Password == AdminPassword)
            {
                // ===== CRIAR CLAIMS DO UTILIZADOR =====
                // Claims são informações sobre o utilizador autenticado
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Input.Username),     // Nome do utilizador
                    new Claim(ClaimTypes.Role, "Admin"),            // Role para autorização
                    new Claim("FullName", "Administrador"),         // Nome completo para UI
                };

                // Criar identidade com os claims
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // ===== CONFIGURAR PROPRIEDADES DO COOKIE =====
                var authProperties = new AuthenticationProperties
                {
                    // IsPersistent: se true, cookie sobrevive ao fechar browser
                    IsPersistent = Input.RememberMe,
                    // Expiração: 7 dias se "Lembrar-me", 8 horas se não
                    ExpiresUtc = Input.RememberMe 
                        ? DateTimeOffset.UtcNow.AddDays(7) 
                        : DateTimeOffset.UtcNow.AddHours(8),
                    // Permitir refresh do cookie quando próximo de expirar
                    AllowRefresh = true,
                };

                // ===== CRIAR COOKIE DE AUTENTICAÇÃO =====
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Redirecionar para a página pretendida
                return LocalRedirect(ReturnUrl);
            }

            // Credenciais inválidas - mostrar erro
            ErrorMessage = "Utilizador ou palavra-passe incorretos";
            return Page();
        }
    }
}
