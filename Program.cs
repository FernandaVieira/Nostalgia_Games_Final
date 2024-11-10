using Microsoft.EntityFrameworkCore;
using Nostalgia_Games.Data;

var builder = WebApplication.CreateBuilder(args);

// Adicionar o DbContext ao contêiner de DI (Injeção de Dependência)
// Aqui você está configurando a string de conexão com o banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Adicionar os serviços de controlador e visualizações
builder.Services.AddControllersWithViews();

// Configuração do middleware de sessão
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Defina o tempo de expiração da sessão conforme necessário
    options.Cookie.HttpOnly = true;  // Para maior segurança, torna o cookie acessível apenas pelo servidor
    options.Cookie.IsEssential = true;  // Necessário para usar a sessão mesmo em sites sem consentimento de cookies
});

var app = builder.Build();

// Configurar o pipeline de requisições HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // O valor padrão de HSTS é 30 dias. Você pode querer mudar isso para cenários de produção.
    app.UseHsts();
}

// Usar o middleware de sessão
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Definir a rota padrão para os controladores
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
