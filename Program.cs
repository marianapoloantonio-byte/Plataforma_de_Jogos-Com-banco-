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

// ================================= CATEGORIAS ===============================================

// Listar todas as categorias
app.MapGet("/categorias", (AppDbContext context) =>
{
    return context.Categorias.ToList();
});

// Buscar uma categoria pelo ID
app.MapGet("/categorias/{id}", (int id, AppDbContext context) =>
{
    var categoria = context.Categorias.Find(id);

    if (categoria == null)
        return Results.NotFound();

    return Results.Ok(categoria);
});


// ==================================== JOGOS ==============================================

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
    var jogo = context.Jogos
        .Where(j => j.Id == id)
        .Select(j => new
        {
            j.Id,
            j.Nome,
            j.Preco,
            Categoria = j.Categoria!.Nome
        })
        .FirstOrDefault();

    if (jogo == null)
        return Results.NotFound();

    return Results.Ok(jogo);
});

// Cadastrar jogo
app.MapPost("/jogos", (Jogo jogo, AppDbContext context) =>
{
    var categoria = context.Categorias.Find(jogo.CategoriaId);

    if(categoria == null)
    {
        return Results.BadRequest("Categoria não encontrada!");
    }

    jogo.Categoria = categoria;

    context.Jogos.Add(jogo);
    context.SaveChanges();

    return Results.Created($"/jogos/{jogo.Id}", jogo);
});

//Atualizar dados do jogo
app.MapPut("/jogos/{id}", (int id, Jogo jogo, AppDbContext context) =>
{
    //procura o jogo pelo Id
    var jogoExistente = context.Jogos.Find(id);

    if (jogoExistente == null)
    {
        return Results.NotFound("Jogo não encontrado!");
    }

    //procura categoria pelo id
    var categoria = context.Categorias.Find(jogo.CategoriaId);

    if (categoria == null)
    {
        return Results.BadRequest("Categoria não encontrada!");
    }

    //Atualiza as informações do jogo
    jogoExistente.Nome = jogo.Nome;
    jogoExistente.Preco = jogo.Preco;
    jogoExistente.CategoriaId = jogo.CategoriaId;
    jogoExistente.Categoria = categoria;

    //Salva as alterações no banco
    context.SaveChanges();

    return Results.Ok(jogoExistente);
});

// Remover jogo
app.MapDelete("/jogos/{id}", (int id, AppDbContext context) =>
{
    var jogo = context.Jogos.Find(id);

    if (jogo == null)
    {
        return Results.NotFound("Jogo não encontrado!");
    }

    context.Jogos.Remove(jogo);
    context.SaveChanges();

    return Results.Ok("Jogo removido com sucesso!");
});


// ================================== USUARIO ===============================================

// Listar todos os usuários
app.MapGet("/usuarios", (AppDbContext context) =>
{
    var usuarios = context.Usuarios
        .Select(u => new
        {
            u.Id,
            u.Nome,
            u.Email
        })
        .ToList();

    return Results.Ok(usuarios);
});

// Buscar usuário pelo ID
app.MapGet("/usuarios/{id}", (int id, AppDbContext context) =>
{
    var usuario = context.Usuarios
        .Where(u => u.Id == id) //procura o usuario correspondente ao id informado
        .Select(u => new
        {
            u.Id,
            u.Nome,
            u.Email
        })
        .FirstOrDefault(); //retorna o usuario encontrado ou null se nao existir

    if (usuario == null) //se nao for encontrado, retorna um erro
    {
        return Results.NotFound("Usuário não encontrado!");
    }

    return Results.Ok(usuario);
});

// Cadastrar usuário
app.MapPost("/usuarios", (Usuario usuario, AppDbContext context) =>
{
    context.Usuarios.Add(usuario);
    context.SaveChanges();

    return Results.Created($"/usuarios/{usuario.Id}", new
    {
        usuario.Id,
        usuario.Nome,
        usuario.Email
    });
});

// Atualizar usuário
app.MapPut("/usuarios/{id}", (int id, Usuario usuario, AppDbContext context) =>
{
    var usuarioExistente = context.Usuarios.Find(id); //Procura usuario pelo id da URL

    if (usuarioExistente == null) //Se nao encontrar retorna erro
    {
        return Results.NotFound("Usuário não encontrado!");
    }

    //Atualiza os dados
    usuarioExistente.Nome = usuario.Nome;
    usuarioExistente.Email = usuario.Email;
    usuarioExistente.Senha = usuario.Senha;

    context.SaveChanges();

    return Results.Ok(new
    {
        usuarioExistente.Id,
        usuarioExistente.Nome,
        usuarioExistente.Email
    });
});

// Remover usuário
app.MapDelete("/usuarios/{id}", (int id, AppDbContext context) =>
{
    var usuario = context.Usuarios.Find(id);

    if (usuario == null)
    {
        return Results.NotFound("Usuário não encontrado!");
    }

    context.Usuarios.Remove(usuario);
    context.SaveChanges();

    return Results.Ok("Usuário removido com sucesso!");
});


// ====================================== COMPRAS =========================================


app.Run();
