using Microsoft.EntityFrameworkCore;
using PlataformaDeJogosApi.Data;
using PlataformaDeJogosApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------- Banco de dados do projeto -------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("ConexaoPadrao")
    )
);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// ---------- CATEGORIAS FIXAS ----------

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

     // Atualiza o banco de dados
    context.Database.Migrate();

    if (!context.Categorias.Any())
    {
        context.Categorias.AddRange(
            new Categoria { Nome = "Fantasia Sombria" },
            new Categoria { Nome = "Pós-apocalíptico" },
            new Categoria { Nome = "Aventura" }
        );

        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
// ---------- CATEGORIAS ----------

// Listar todas as categorias
app.MapGet("/categorias", (AppDbContext context) =>
{
    return context.Categorias.ToList();
});

// Cadastrar uma categoria
app.MapPost("/categorias", (Categoria categoria, AppDbContext context) =>
{
    context.Categorias.Add(categoria);
    context.SaveChanges();

    return Results.Created($"/categorias/{categoria.Id}", categoria);
});

// Buscar uma categoria pelo ID
app.MapGet("/categorias/{id}", (int id, AppDbContext context) =>
{
    var categoria = context.Categorias.Find(id);

    if (categoria == null)
        return Results.NotFound();

    return Results.Ok(categoria);
});



app.Run();
