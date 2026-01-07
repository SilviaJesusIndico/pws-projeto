/// ============================================================================
/// PROGRAM.CS - Ficheiro de Inicialização da Aplicação PetShop
/// ============================================================================
/// 
/// Este ficheiro é o ponto de entrada principal da aplicação.
/// Responsabilidades:
/// - Configurar serviços (Dependency Injection)
/// - Configurar o pipeline HTTP
/// - Registar DbContext e base de dados
/// - Configurar autenticação e autorização
/// - Iniciar a aplicação
/// 
/// Padrão: Minimal Hosting Model do ASP.NET Core 9.0
/// ============================================================================

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;

// ===== Criar o builder da aplicação =====
// O builder é responsável pela configuração de serviços e pipeline
var builder = WebApplication.CreateBuilder(args);

// ===== SECÇÃO DE SERVIÇOS (Dependency Injection) =====
// Adiciona os serviços que serão injetados em toda a aplicação

// Adiciona suporte para Razor Pages
// Isto permite que a aplicação use o padrão de Razor Pages
builder.Services.AddRazorPages();

// Configurar Autenticação com Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
        options.AccessDeniedPath = "/Admin/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "PetShop.Auth";
        options.Cookie.HttpOnly = true;
    });

builder.Services.AddAuthorization();

// Adiciona o DbContext ao contentor de serviços
// O DbContext será injetado automaticamente em páginas e controladores
// Configurado para usar SQLite com a string de conexão do appsettings.json
builder.Services.AddDbContext<PetShopContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("PetShopDatabase")
        ?? throw new InvalidOperationException("Connection string 'PetShopDatabase' not found.")
    )
);

// ===== CONSTRUIR A APLICAÇÃO =====
// Após configurar todos os serviços, construímos a aplicação
var app = builder.Build();

// ===== SEED DE DADOS INICIAIS =====
// Adiciona serviços de exemplo se a tabela estiver vazia
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PetShopContext>();
    context.Database.EnsureCreated();
    
    if (!context.Servicos.Any())
    {
        context.Servicos.AddRange(
            new PetShop.Models.Servico { NomeServico = "Banho Simples", Descricao = "Banho completo com shampoo adequado ao tipo de pelo do animal", Preco = 15.00m, DuracaoMinutos = 30 },
            new PetShop.Models.Servico { NomeServico = "Banho e Tosa", Descricao = "Banho completo seguido de tosa higiénica ou à escolha do cliente", Preco = 25.00m, DuracaoMinutos = 60 },
            new PetShop.Models.Servico { NomeServico = "Tosa Completa", Descricao = "Tosa completa com acabamento profissional", Preco = 20.00m, DuracaoMinutos = 45 },
            new PetShop.Models.Servico { NomeServico = "Corte de Unhas", Descricao = "Corte e lima das unhas do animal", Preco = 8.00m, DuracaoMinutos = 15 },
            new PetShop.Models.Servico { NomeServico = "Limpeza de Ouvidos", Descricao = "Limpeza profunda dos ouvidos com produtos adequados", Preco = 10.00m, DuracaoMinutos = 20 },
            new PetShop.Models.Servico { NomeServico = "Escovagem de Dentes", Descricao = "Higiene oral completa com pasta e escova apropriadas", Preco = 12.00m, DuracaoMinutos = 20 },
            new PetShop.Models.Servico { NomeServico = "Consulta Veterinária", Descricao = "Consulta geral com veterinário especializado", Preco = 35.00m, DuracaoMinutos = 30 },
            new PetShop.Models.Servico { NomeServico = "Vacinação", Descricao = "Aplicação de vacinas com acompanhamento veterinário", Preco = 25.00m, DuracaoMinutos = 20 },
            new PetShop.Models.Servico { NomeServico = "Desparasitação", Descricao = "Tratamento antiparasitário interno e externo", Preco = 15.00m, DuracaoMinutos = 15 },
            new PetShop.Models.Servico { NomeServico = "Spa Completo", Descricao = "Pacote premium: banho, tosa, unhas, ouvidos e hidratação", Preco = 45.00m, DuracaoMinutos = 90 }
        );
        context.SaveChanges();
    }
}

// ===== SECÇÃO DO PIPELINE HTTP =====
// Define como a aplicação processa os pedidos HTTP

// Configurar tratamento de exceções e segurança HTTPS
if (!app.Environment.IsDevelopment())
{
    // Em produção: mostrar página de erro genérica
    app.UseExceptionHandler("/Error");
    
    // HSTS: Reforça segurança com HTTPS (30 dias por padrão)
    app.UseHsts();
}

// Redireciona HTTP para HTTPS automaticamente
app.UseHttpsRedirection();

// Configura o roteamento (routing)
app.UseRouting();

// Adiciona suporte a autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Mapeia activos estáticos (CSS, JavaScript, imagens, etc.)
app.MapStaticAssets();

// Mapeia Razor Pages - torna todas as páginas .cshtml disponíveis
app.MapRazorPages()
   .WithStaticAssets();

// ===== Inicia a aplicação =====
app.Run();
