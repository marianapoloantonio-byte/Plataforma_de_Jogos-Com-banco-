namespace PlataformaDeJogosApi.Models;

public class Compra
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public int JogoId { get; set; }
    public Jogo? Jogo { get; set; }

    public double ValorPago { get; set; }
}