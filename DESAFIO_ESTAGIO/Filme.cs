


namespace DESAFIO_ESTAGIO
{
    public class Filme
    {
        public int Id { get; set; }
        public string Titulo { get;set; } = string.Empty;
        public string Diretor { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public List<string> Atores { get;set; } = new List<string>();
        public List<Voto> Votos { get; set; } = new List<Voto>();
        public double MediaVotos { get { return CalcularMediaVotos(); } } 

        public double CalcularMediaVotos()
        {
            if (Votos.Count == 0)
            {
                return 0;
            }
            double soma = 0;
            foreach (var voto in Votos)
            {
                soma += voto.Nota;
            }
            return soma / Votos.Count;
        }
        public bool AdicionarVoto(Voto voto)
        {
            if (voto.FilmeId != Id)
            {
                return false;
            }
            foreach (var votoExistente in Votos)
            {
                if (votoExistente.UsuarioId == voto.UsuarioId)
                {
                    return false;
                }
            }
            if (voto.ValidarNota() == true)
            {
             Votos.Add(voto);
             return true;
            }
            
            return false;
        }
    }
}
