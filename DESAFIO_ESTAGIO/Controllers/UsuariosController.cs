using Microsoft.AspNetCore.Mvc;
using DESAFIO_ESTAGIO.Contracts;

namespace DESAFIO_ESTAGIO.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosController : ControllerBase
    {
        [HttpGet]
        public IActionResult Listar()
        {
            List<UsuarioResponse> usuarios = new();
            return Ok(usuarios);
        }
        [HttpGet("{id:int}")]
        public IActionResult BuscaId(int id)
        {

            return NotFound();
        }
        [HttpPost]
        public IActionResult Cadastrar(CadastrarUsuarioRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nome))
            {
                return BadRequest("Nome não pode ser nulo ou vazio.");
            }
            if (string.IsNullOrWhiteSpace(request.Login))
            {
                return BadRequest("Login não pode ser nulo ou vazio.");
            }
            if (string.IsNullOrWhiteSpace(request.Senha))
            {
                return BadRequest("Senha não pode ser nulo ou vazio.");
            }
            UsuarioResponse resposta = new UsuarioResponse
            {
                Nome = request.Nome,
                Login = request.Login
            };
            return Ok(resposta);
        }
        [HttpPut("{id:int}")]
        public IActionResult Atualizar(int id, AtualizarUsuarioRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nome))
            {
                return BadRequest("Nome não pode ser nulo ou vazio.");
            }
            if (string.IsNullOrWhiteSpace(request.Login))
            {
                return BadRequest("Login não pode ser nulo ou vazio.");
            }

            return NotFound();
        }
        [HttpDelete("{id:int}")]
        public IActionResult Deletar(int id)
        {
            return NotFound();
        }
    }
}
     