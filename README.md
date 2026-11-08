#  Plataforma de Jogos API

## Sobre o projeto

Este projeto foi desenvolvido como parte de uma atividade acadêmica do curso de Engenharia de Software.

O objetivo é desenvolver uma API para uma plataforma de jogos, permitindo o gerenciamento de jogos, categorias, usuários e compras.

A aplicação foi desenvolvida utilizando C# e .NET, com banco de dados SQLite e Entity Framework Core para realizar o armazenamento e a manipulação das informações.

## Tecnologias utilizadas

- **C#** – Linguagem de programação utilizada no desenvolvimento.
- **.NET 10** – Framework utilizado para construir a aplicação.
- **ASP.NET Core** – Utilizado para criação dos endpoints da API.
- **Entity Framework Core** – Responsável pela comunicação com o banco de dados.
- **SQLite** – Banco de dados utilizado para armazenar as informações.
- **Postman** – Ferramenta utilizada para testar as requisições.
- **Visual Studio** – Ambiente de desenvolvimento utilizado.

## Funcionalidades

A API possui operações de cadastro, consulta, atualização e exclusão de dados, além de algumas regras de negócio.

###  Jogos

- Cadastrar novos jogos.
- Listar todos os jogos cadastrados.
- Buscar um jogo pelo ID.
- Atualizar as informações de um jogo.
- Excluir um jogo.
- Associar cada jogo a uma categoria.
- Impedir a exclusão de jogos que já foram comprados.

###  Categorias

O projeto possui três categorias cadastradas automaticamente no banco de dados:

- Fantasia Sombria.
- Pós-apocalíptico.
- Aventura.

Também é possível consultar todas as categorias ou buscar uma categoria específica pelo ID.

###  Usuários

- Cadastrar novos usuários.
- Listar todos os usuários.
- Buscar um usuário pelo ID.
- Atualizar os dados de um usuário.
- Excluir usuários.
- Impedir a exclusão de usuários que já realizaram compras.

###  Compras

- Realizar a compra de um jogo.
- Verificar se o usuário está cadastrado.
- Verificar se o jogo existe.
- Impedir que um usuário compre o mesmo jogo mais de uma vez.
- Aplicar desconto de 10% em jogos com valor acima de R$ 100,00.
- Listar todas as compras realizadas.

## Endpoints da API

Os endpoints foram organizados de acordo com cada funcionalidade do projeto.

### Jogos

| Método | Rota | Descrição |
|---|---|---|
| GET | `/jogos` | Lista todos os jogos |
| GET | `/jogos/{id}` | Busca um jogo pelo ID |
| POST | `/jogos` | Cadastra um jogo |
| PUT | `/jogos/{id}` | Atualiza um jogo |
| DELETE | `/jogos/{id}` | Exclui um jogo |

### Categorias

| Método | Rota | Descrição |
|---|---|---|
| GET | `/categorias` | Lista todas as categorias |
| GET | `/categorias/{id}` | Busca uma categoria pelo ID |

### Usuários

| Método | Rota | Descrição |
|---|---|---|
| GET | `/usuarios` | Lista todos os usuários |
| GET | `/usuarios/{id}` | Busca um usuário pelo ID |
| POST | `/usuarios` | Cadastra um usuário |
| PUT | `/usuarios/{id}` | Atualiza um usuário |
| DELETE | `/usuarios/{id}` | Exclui um usuário |

### Compras

| Método | Rota | Descrição |
|---|---|---|
| GET | `/compras` | Lista todas as compras |
| POST | `/compras` | Realiza uma compra |

## Exemplos de requisições

As requisições podem ser realizadas utilizando o Postman. Para isso, basta selecionar o método HTTP correspondente, informar a rota e enviar os dados no formato JSON quando necessário.

### Cadastrar um jogo

**POST** `/jogos`

```json
{
    "nome": "Resident Evil 4",
    "preco": 120,
    "categoriaId": 1
}
```

### Cadastrar um usuário

**POST** `/usuarios`

```json
{
    "nome": "Violet",
    "email": "Violet@email.com",
    "senha": "123456"
}
```

### Realizar uma compra

**POST** `/compras`

```json
{
    "usuarioId": 1,
    "jogoId": 1
}
```

## Banco de dados

O projeto utiliza o SQLite como banco de dados e o Entity Framework Core para realizar as operações.

As tabelas são criadas e atualizadas por meio das migrations do Entity Framework Core.

Além disso, o projeto possui um cadastro inicial de categorias e jogos, realizado automaticamente quando o banco ainda não possui esses registros.

## Como executar o projeto

Para executar a API localmente, é necessário ter o .NET SDK instalado.

1. Clone o repositório:

```bash
git clone URL_DO_REPOSITORIO
```

2. Entre na pasta do projeto:

```bash
cd PlataformaDeJogosApi
```

3. Restaure as dependências:

```bash
dotnet restore
```

4. Execute a aplicação:

```bash
dotnet run
```

5. Após iniciar, a API estará disponível no endereço local indicado pelo terminal.

Para testar as funcionalidades, utilize o Postman e informe o endereço da API junto com a rota desejada.

## Considerações finais

O desenvolvimento deste projeto foi uma oportunidade de colocar em prática os conhecimentos adquiridos durante o curso, principalmente na utilização de C#, desenvolvimento de APIs, banco de dados e Entity Framework Core.

Durante a implementação, foram trabalhadas operações CRUD, relacionamentos entre tabelas e regras de negócio, contribuindo para uma melhor compreensão do funcionamento de uma API e da comunicação entre a aplicação e o banco de dados.
