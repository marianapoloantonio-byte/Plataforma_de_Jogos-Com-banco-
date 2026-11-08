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

    //Se o jogo ja foi comprado por algum usuario, nao permite excluir
    var compraExistente = context.Compras.Any(c => c.JogoId == id);
    if (compraExistente)
    {
        return Results.BadRequest("Não é possível remover um jogo que já foi comprado!");
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

    //Nao permite excluir usuario que ja comprou algum jogo
    var compraExistente = context.Compras.Any(c => c.UsuarioId == id);
    if (compraExistente)
    {
        return Results.BadRequest("Não é possível remover um usuário que já realizou compras!");
    }

    context.Usuarios.Remove(usuario);
    context.SaveChanges();

    return Results.Ok("Usuário removido com sucesso!");
});


// ====================================== COMPRAS =========================================

// Realizar uma compra
app.MapPost("/compras", (Compra compra, AppDbContext context) =>
{
    var usuario = context.Usuarios.Find(compra.UsuarioId); //Procura usuario
 
    if (usuario == null) //se o usuario nao existir, retorna um erro
    {
        return Results.NotFound("Usuário não encontrado!");
    }

    var jogo = context.Jogos.Find(compra.JogoId); //Procura o jogo

    if (jogo == null) //se o jogo nao existir, retorna erro
    {
        return Results.NotFound("Jogo não encontrado!");
    }

    //Verifica se o usuario ja comprou aquele jogo
    var compraExistente = context.Compras.Any(c =>
        c.UsuarioId == compra.UsuarioId && c.JogoId == compra.JogoId);

    if (compraExistente)
    {
        return Results.BadRequest("Usuário já possui esse jogo!");
    }


    double valorPago = jogo.Preco;

    if (jogo.Preco > 100) //Aplica o desconto
    {
        valorPago = jogo.Preco * 0.90;
    }

    //Cria a nova compra
    var novaCompra = new Compra
    {
        UsuarioId = usuario.Id,
        JogoId = jogo.Id,
        ValorPago = valorPago
    };

    //Adiciona a compra no banco
    context.Compras.Add(novaCompra);
    context.SaveChanges();

    return Results.Created($"/compras/{novaCompra.Id}", new
    {
        novaCompra.Id,
        Usuario = usuario.Nome,
        Jogo = jogo.Nome,
        ValorOriginal = jogo.Preco,
        ValorPago = novaCompra.ValorPago
    });
});

// Listar todas as compras
app.MapGet("/compras", (AppDbContext context) =>
{
    var compras = context.Compras
        .Select(c => new
        {
            c.Id,
            Usuario = c.Usuario!.Nome, //retorna o nome do usuario que realizou a compra
            Jogo = c.Jogo!.Nome, //retorna o nome do jogo que realizou a compra
            c.ValorPago
        })
        .ToList();

    return Results.Ok(compras);
});

// Consultar biblioteca do usuário
app.MapGet("/usuarios/{id}/biblioteca", (int id, AppDbContext context) =>
{
    var usuario = context.Usuarios.Find(id); //procura usuario pelo Id

    if (usuario == null)
    {
        return Results.NotFound("Usuário não encontrado!");
    }

    var biblioteca = context.Compras //busca as compras feitas por esse usuario
        .Where(c => c.UsuarioId == id)
        .Select(c => new
        {
            c.Jogo!.Id,
            c.Jogo.Nome,
            c.Jogo.Preco,
            c.ValorPago
        })
        .ToList();

    return Results.Ok(biblioteca); //retorna a biblioteca
});


app.Run();
