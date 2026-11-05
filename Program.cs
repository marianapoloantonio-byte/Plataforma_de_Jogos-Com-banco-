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
    // ---------- JOGOS FIXOS ----------

if (!context.Jogos.Any())
{
    var fantasia = context.Categorias.First(c => c.Nome == "Fantasia Sombria");
    var posApocaliptico = context.Categorias.First(c => c.Nome == "Pós-apocalíptico");
    var aventura = context.Categorias.First(c => c.Nome == "Aventura");

    context.Jogos.AddRange(
        new Jogo { Nome = "A Plague Tale", Preco = 139.90, CategoriaId = fantasia.Id },
        new Jogo { Nome = "The Last of Us", Preco = 199.90, CategoriaId = posApocaliptico.Id },
        new Jogo { Nome = "Tomb Raider", Preco = 124.90, CategoriaId = aventura.Id }
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
// ---------- JOGOS ----------

// Listar todos os jogos
app.MapGet("/jogos", (AppDbContext context) =>
{
    var jogos = context.Jogos
        .Select(j => new
        {
            j.Id,
            j.Nome,
            j.Preco,
            Categoria = j.Categoria!.Nome
        })
        .ToList();

    return Results.Ok(jogos);
});
// Buscar jogo pelo ID
app.MapGet("/jogos/{id}", (int id, AppDbContext context) =>
{
    var jogo = context.Jogos.Find(id);

    if (jogo == null)
        return Results.NotFound();

    return Results.Ok(jogo);
});


app.Run();
