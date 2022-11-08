using Microsoft.AspNetCore.Mvc;

namespace ServicoExterno.Controllers
{
    [Route("api/[controller]")]
    public class ExternoController : ControllerBase
    {

        [HttpGet("{taxaSucesso:int}")]
        public async Task<IActionResult> Teste(int taxaSucesso) 
        {
            var random = new Random();

            var sucesso = random.Next(1,100) < taxaSucesso;

            var msg = "---> SUCESSO !!!";
            

            if(sucesso)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine(msg);
                return Ok(msg);
            }

                Console.ForegroundColor = ConsoleColor.Red;
                msg = " ---> FALHA !!!";
                System.Console.WriteLine(msg);
                return StatusCode(500,msg);


        }
    }
}