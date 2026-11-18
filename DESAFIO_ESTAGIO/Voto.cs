namespace DESAFIO_ESTAGIO
{
    public class Voto
    {
        public int FilmeId { get; set; }
        public int UsuarioId { get; set; }
        public int Nota { get; set; } 

        public bool ValidarNota()
        {
            if (Nota >= 0 && Nota <= 4)
            {
                return true;
            }
            return false;
        }
    }
}
