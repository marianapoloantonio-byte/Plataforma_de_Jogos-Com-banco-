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

// Atualizar uma categoria
app.MapPut("/categorias/{id}", (int id, Categoria dados, AppDbContext context) =>
{
    var categoria = context.Categorias.Find(id);

    if (categoria == null)
        return Results.NotFound();

    categoria.Nome = dados.Nome;

    context.SaveChanges();

    return Results.Ok(categoria);
});

// Excluir uma categoria
app.MapDelete("/categorias/{id}", (int id, AppDbContext context) =>
{
    var categoria = context.Categorias.Find(id);

    if (categoria == null)
        return Results.NotFound();

    context.Categorias.Remove(categoria);
    context.SaveChanges();

    return Results.Ok();
});

app.Run();
