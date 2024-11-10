using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nostalgia_Games.Data;
using Nostalgia_Games.Models;
using Newtonsoft.Json;

namespace Nostalgia_Games.Controllers
{
    public class Trivia_MusicalController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private Random _random = new Random();
        private bool _musicasCarregadas = false;

        public List<Musica> MusicaJogo { get; private set; } = new List<Musica>();
        public Musica MusicaReferencia { get; private set; } = new Musica();
        public List<Musica> _listaMusicasTemporaria { get; private set; } = new List<Musica>();
        public DateTime TempoInicio { get; private set; }
        public int PontuacaoJogador { get; private set; }
        public bool JogoFinalizado { get; private set; } = false;

        public Trivia_MusicalController(AppDbContext context)
        {
            _appDbContext = context;
        }

        public ActionResult Index()
        {
            // Recuperar os dados da Session, se disponíveis
            if (HttpContext.Session.GetString("MusicasCarregadas") != null)
            {
                _musicasCarregadas = Convert.ToBoolean(HttpContext.Session.GetString("MusicasCarregadas"));
            }

            if (HttpContext.Session.GetString("MusicaJogo") != null)
            {
                MusicaJogo = JsonConvert.DeserializeObject<List<Musica>>(HttpContext.Session.GetString("MusicaJogo"));
            }

            if (HttpContext.Session.GetString("MusicaReferencia") != null)
            {
                MusicaReferencia = JsonConvert.DeserializeObject<Musica>(HttpContext.Session.GetString("MusicaReferencia"));
            }

            if (HttpContext.Session.GetString("PontuacaoJogador") != null)
            {
                PontuacaoJogador = Convert.ToInt32(HttpContext.Session.GetString("PontuacaoJogador"));
            }

            if (HttpContext.Session.GetString("ListaMusicasTemporaria") != null)
            {
                _listaMusicasTemporaria = JsonConvert.DeserializeObject<List<Musica>>(HttpContext.Session.GetString("ListaMusicasTemporaria"));
            }

            if (!_musicasCarregadas)
            {
                CarregarMusicas();
            }
            SalvarNaSession();

            var musicaReferencia = MusicaReferencia;
            var musicaJogo = MusicaJogo.OrderBy(m => m.AnoLancamento).ToList();
            var placar = PontuacaoJogador;

            var viewModel = new MusicaJogoViewModel
            {
                MusicaJogo = musicaJogo,
                MusicaReferencia = musicaReferencia,
                PontuacaoJogador = placar,
            };

            return View(viewModel);
        }

        private void CarregarMusicas()
        {
            if (_musicasCarregadas) return; // Verifica se as músicas já foram carregadas

            _listaMusicasTemporaria = _appDbContext.Musicas.ToList();
            _musicasCarregadas = true;

            MusicaJogo.Add(ObterMusicaAleatoria());
            MusicaReferencia = ObterMusicaAleatoria();
        }

        public Musica ObterMusicaAleatoria()
        {
            TempoInicio = DateTime.Now;

            if (_listaMusicasTemporaria.Count == 0)
            {
                CarregarMusicas();
            }

            int indexAleatorio = _random.Next(_listaMusicasTemporaria.Count);
            Musica musicaSelecionada = _listaMusicasTemporaria[indexAleatorio];
            _listaMusicasTemporaria.RemoveAt(indexAleatorio);

            return musicaSelecionada;
        }

        public void Comparar(int anoreferencia, int anoMusica, Func<int, int, bool> comparador)
        {
            if (comparador(anoreferencia, anoMusica))
            {
                MusicaJogo.Add(MusicaReferencia); // Adiciona sem reiniciar
                AtualizarPontuacao(true);
            }
            else
            {
                AtualizarPontuacao(false);
            }
            MusicaReferencia = ObterMusicaAleatoria();
        }

        [HttpPost]
        public ActionResult CompararAntes(int anoreferencia, int anoMusica)
        {
            Comparar(anoreferencia, anoMusica, (refAno, ano) => refAno <= ano);

            // Armazena os dados na Session antes de redirecionar
            SalvarNaSession();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CompararDepois(int anoreferencia, int anoMusica)
        {
            Comparar(anoreferencia, anoMusica, (refAno, ano) => refAno >= ano);

            // Armazena os dados na Session antes de redirecionar
            SalvarNaSession();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CompararEntre(int anoreferencia, int anoMusica1, int anoMusica2)
        {
            Comparar(anoreferencia, anoMusica1, (refAno, ano1) => refAno >= ano1 && refAno <= anoMusica2);

            // Armazena os dados na Session antes de redirecionar
            SalvarNaSession();

            return RedirectToAction("Index");
        }

        private void AtualizarPontuacao(bool acertou)
        {
            if (acertou)
            {
                PontuacaoJogador++;
            }

            if (PontuacaoJogador >= 500)
            {
                JogoFinalizado = true;
            }
        }

        // Método para salvar os dados na session
        private void SalvarNaSession()
        {
            HttpContext.Session.SetString("MusicaJogo", JsonConvert.SerializeObject(MusicaJogo));
            HttpContext.Session.SetString("MusicaReferencia", JsonConvert.SerializeObject(MusicaReferencia));
            HttpContext.Session.SetString("PontuacaoJogador", PontuacaoJogador.ToString());
            HttpContext.Session.SetString("ListaMusicasTemporaria", JsonConvert.SerializeObject(_listaMusicasTemporaria));
            HttpContext.Session.SetString("MusicasCarregadas", _musicasCarregadas.ToString());
        }
    }
}
