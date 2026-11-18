namespace DESAFIO_ESTAGIO
{
    public class AutentificaHelper
    {
        internal bool CompararUsuario(string usuario, string usuarioArmazenado)
        {
            return usuario == usuarioArmazenado;
        }

        internal bool CompararSenha(string senha, string senhaArmazenada)
        {
            return senha == senhaArmazenada;
        }

    }   
        
    
}

