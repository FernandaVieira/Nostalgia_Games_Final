using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nostalgia_Games.Data;
using Nostalgia_Games.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nostalgia_Games.Controllers
{
    public class Trivia_MusicalController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private Random _random = new Random();
        

        public List<Musica> MusicaJogo { get; private set; } = new List<Musica>();
        public Musica MusicaReferencia { get; private set; } = new Musica();
        public List<Musica> _listaMusicasTemporaria { get; private set; } = new List<Musica>();
        public DateTime TempoInicio { get; private set; }
        public int PontuacaoJogador { get; private set; }
        public bool JogoFinalizado { get; private set; } = false;
        public bool _musicasCarregadas { get; private set; }

        public Trivia_MusicalController(AppDbContext context)
        {
            if(!_musicasCarregadas) _appDbContext = context;
            
        }

        public ActionResult Index()
        {
            // Recuperar os dados da Session, se disponíveis
            //RecuperarDadosDaSession();

            if (!_musicasCarregadas)
            {
                CarregarMusicas();
            }
            //MusicaReferencia = ObterMusicaAleatoria();
            SalvarNaSession();
                        
            var viewModel = new MusicaJogoViewModel
            {
                MusicaJogo_ = MusicaJogo.OrderBy(m => m.AnoLancamento).ToList(),
                MusicaReferencia_ = MusicaReferencia,
                PontuacaoJogador_ = PontuacaoJogador,
            };

            return View(viewModel);
        }

        private void CarregarMusicas()
        {
            RecuperarDadosDaSession();
            if (_musicasCarregadas) return;

            _listaMusicasTemporaria = _appDbContext.Musicas.ToList();
            _musicasCarregadas = true;

            MusicaJogo.Add(ObterMusicaAleatoria());
            MusicaReferencia = ObterMusicaAleatoria();
        }

        public Musica ObterMusicaAleatoria()
        {
            
            //TempoInicio = DateTime.Now;

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
            //RecuperarDadosDaSession();
            if (comparador(anoreferencia, anoMusica))
            {
                MusicaJogo.Add(MusicaReferencia); // Adiciona sem reiniciar
                AtualizarPontuacao(true);
            }
            else
            {
                AtualizarPontuacao(false);
            }
           
        }

        [HttpPost]
        public ActionResult CompararAntes(int anoreferencia, int anoMusica)
        {
            RecuperarDadosDaSession();
            Comparar(anoreferencia, anoMusica, (refAno, ano) => refAno <= ano);
            MusicaReferencia = ObterMusicaAleatoria();
            SalvarNaSession();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CompararDepois(int anoreferencia, int anoMusica)
        {
            RecuperarDadosDaSession();
            Comparar(anoreferencia, anoMusica, (refAno, ano) => refAno >= ano);
            MusicaReferencia = ObterMusicaAleatoria();
            SalvarNaSession();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CompararEntre(int anoreferencia, int anoMusica1)
        {
            RecuperarDadosDaSession();

            var ultimoAnoReferencia = anoreferencia;

            // Ordenar MusicaJogo por AnoLancamento (caso não esteja ordenado)
            var MusicaJogoOrdenada = MusicaJogo.OrderBy(m => m.AnoLancamento).ToList();

            // Encontrar dois anos consecutivos em MusicaJogoOrdenada que cercam o ultimoAnoReferencia
            for (int i = 0; i < MusicaJogoOrdenada.Count - 1; i++)
            {
                int anoAnterior = MusicaJogoOrdenada[i].AnoLancamento;
                int anoPosterior = MusicaJogoOrdenada[i + 1].AnoLancamento;

                // Verificar se ultimoAnoReferencia está entre anoAnterior e anoPosterior
                if (ultimoAnoReferencia >= anoAnterior && ultimoAnoReferencia <= anoPosterior)
                {
                    // Está entre os dois anos
                    MusicaJogo.Add(MusicaReferencia);
                    
                    AtualizarPontuacao(true); // Atualiza a pontuação com resposta correta

                    MusicaReferencia = ObterMusicaAleatoria();
                    SalvarNaSession();
                    return RedirectToAction("Index");
                }
            }
            // Caso o ultimoAnoReferencia não esteja entre dois anos
            // Resposta incorreta
            AtualizarPontuacao(false); 
            MusicaReferencia = ObterMusicaAleatoria();
            SalvarNaSession();
            return RedirectToAction("Index");
        }

        private void AtualizarPontuacao(bool acertou)
        {
            if (acertou)
            {
                PontuacaoJogador += 100;
            }

            if (PontuacaoJogador >= 500)
            {
                JogoFinalizado = true;
            }
        }

        private void SalvarNaSession()
        {
            HttpContext.Session.SetString("MusicaJogo", JsonConvert.SerializeObject(MusicaJogo));
            HttpContext.Session.SetString("MusicaReferencia", JsonConvert.SerializeObject(MusicaReferencia));
            HttpContext.Session.SetString("PontuacaoJogador", PontuacaoJogador.ToString());
            HttpContext.Session.SetString("ListaMusicasTemporaria", JsonConvert.SerializeObject(_listaMusicasTemporaria));
            HttpContext.Session.SetString("MusicasCarregadas", _musicasCarregadas.ToString());
        }

        private void RecuperarDadosDaSession()
        {
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
        }
    }
}
