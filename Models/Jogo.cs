namespace PlataformaDeJogosApi.Models;

public class Jogo
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public double Preco { get; set; }

    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; } //O ? indica que a categoria pode estar temporariamente sem valor, por exemplo, quando um jogo ainda não foi carregado com seu relacionamento.
}