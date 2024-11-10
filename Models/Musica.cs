namespace Nostalgia_Games.Models
{
    public class Musica
    {
        public int Id { get; set; }
        public string NomeDoCantor { get; set; }
        public int AnoLancamento { get; set; }
        public string NomeDaMusica { get; set; }
        public string IdMusica { get; set; }

    }

    public class MusicaJogoViewModel
    {
        public int PontuacaoJogador { get; set; }
        public Musica MusicaReferencia { get; set; }
        public List<Musica> MusicaJogo { get; set; }        
        


    }
}
