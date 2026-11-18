namespace DESAFIO_ESTAGIO.Contracts
{
    public class CadastrarUsuarioRequest
    {  
        public string Nome { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
