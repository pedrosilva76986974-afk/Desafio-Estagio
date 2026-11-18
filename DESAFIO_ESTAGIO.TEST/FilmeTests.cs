namespace DESAFIO_ESTAGIO.TEST
{
    [TestClass]
    public sealed class FilmeTests
    {
        [TestMethod]
        public void CalcularMediaVotos_ComUmVoto_DeveRetornarSuaNota()
        {
            Filme filme = new Filme();
            filme.Id = 1;
            filme.Titulo = "O Poderoso Chefão";
            filme.Diretor = "Francis Ford Coppola";
            filme.Genero = "Crime";
            filme.Atores = new List<string> { "Marlon Brando", "Al Pacino", "James Caan" };
            filme.AdicionarVoto(new Voto { FilmeId = 1, UsuarioId = 1, Nota = 4});

            Assert.AreEqual(4, filme.CalcularMediaVotos());
          


        }
        [TestMethod]
        public void AdicionarVoto_ComNotaInvalida()
        {
            Filme filme = new Filme();
            filme.Id = 1;
            var votoAdd = filme.AdicionarVoto(new Voto { UsuarioId = 1, FilmeId = 1, Nota = 5 });
            Assert.IsFalse(votoAdd);
            Assert.AreEqual(0,filme.Votos.Count);

        }     
        [TestMethod]
        public void AdicionarVoto_ComNotaZero()
        {
            Filme filme = new Filme();
            filme.Id = 1;
            filme.AdicionarVoto(new Voto { FilmeId = 1, Nota = 0, UsuarioId = 1});
            Assert.AreEqual(filme.Votos.Count, 1);
        }
        [TestMethod]
        public void AdicionarVoto_ComUsuarioRepetido()
        {
            Filme filme = new Filme();  
            filme.Id = 1;
            filme.AdicionarVoto(new Voto { FilmeId = 1, Nota = 2, UsuarioId = 1});
            var votoAdd = filme.AdicionarVoto(new Voto { FilmeId = 1, UsuarioId = 1, Nota = 4 });
            Assert.IsFalse(votoAdd);
            Assert.AreEqual(filme.Votos.Count, 1);
        }
        [TestMethod]
        public void AdicionarVoto_ComUsuarioDiferentes()
        {
            Filme filme = new Filme();
            filme.Id = 1;
            var votoAdd = filme.AdicionarVoto(new Voto { FilmeId = 1, Nota = 2, UsuarioId = 1});
            var votoAdd2 = filme.AdicionarVoto(new Voto { FilmeId = 1, Nota = 3, UsuarioId = 2 });
            Assert.IsTrue(votoAdd2);
            Assert.IsTrue(votoAdd);
            Assert.AreEqual(2, filme.Votos.Count);
            Assert.AreEqual (2.5, filme.CalcularMediaVotos());

        }

    }
}
