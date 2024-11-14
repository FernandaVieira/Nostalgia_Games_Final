window.onerror = function () {
    return true; // Retorna true para suprimir todos os erros no console
};

function tocarNovaMusica() {
    if (MusicaReferencia.Any()) {
        let novaIdMusica = @MusicaReferencia.IdMusica;
        document.getElementById('player-musica').src = "https://www.youtube.com/embed/" + novaIdMusica + "?autoplay=1";
    }
    else {
        adicionarNovaCarta();
    }
}

navigator.permissions.query({ name: 'geolocation' })
    .then(result => {
        // Manipule a resposta
        console.log(result.state);
    })
    .catch(error => {
        console.error('Erro ao executar o query:', error);
    });

