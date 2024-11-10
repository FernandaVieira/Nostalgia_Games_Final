using Microsoft.EntityFrameworkCore;
using Nostalgia_Games.Data;

var builder = WebApplication.CreateBuilder(args);

// Adicionar o DbContext ao cont�iner de DI (Inje��o de Depend�ncia)
// Aqui voc� est� configurando a string de conex�o com o banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Adicionar os servi�os de controlador e visualiza��es
builder.Services.AddControllersWithViews();

// Configura��o do middleware de sess�o
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Defina o tempo de expira��o da sess�o conforme necess�rio
    options.Cookie.HttpOnly = true;  // Para maior seguran�a, torna o cookie acess�vel apenas pelo servidor
    options.Cookie.IsEssential = true;  // Necess�rio para usar a sess�o mesmo em sites sem consentimento de cookies
});

var app = builder.Build();

// Configurar o pipeline de requisi��es HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // O valor padr�o de HSTS � 30 dias. Voc� pode querer mudar isso para cen�rios de produ��o.
    app.UseHsts();
}

// Usar o middleware de sess�o
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Definir a rota padr�o para os controladores
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
