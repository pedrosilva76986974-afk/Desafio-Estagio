namespace DESAFIO_ESTAGIO
{
    public class Usuario : IAutentificavel
    {
        private AutentificaHelper _autentificaHelper = new AutentificaHelper();
        public bool Autenticar(string usuario, string senha)
        {
            if (Excluido == true)
            {
                return false;
            }
            return _autentificaHelper.CompararUsuario(usuario, _usuario) && _autentificaHelper.CompararSenha(senha, _senha);
        }
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public bool Excluido { get; set; } 
        private string _usuario{ get; }
        private string _senha{ get; }
        public Usuario(string usuario, string senha)
        {
            _usuario = usuario;
            _senha = senha;
        }
        public string GetUsuario()
        {
            return _usuario;
        }
        
    }
}
