using Microsoft.AspNetCore.Mvc;

namespace DESAFIO_ESTAGIO.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosController : ControllerBase
    {
        [HttpGet]
        public IActionResult Listar() 
        { 
           List<Usuario> usuarios = new();
           return Ok(usuarios);
           
        }
    }
}
