using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace ServicoInterno.Controllers
{

    [Route("api/[controller]")]
    public class InternoController : ControllerBase
    {

        public AsyncRetryPolicy<HttpResponseMessage> RetryImediato { get;  }

        public AsyncRetryPolicy<HttpResponseMessage> WaitAndRetry { get;  }

        public AsyncRetryPolicy<HttpResponseMessage> RetryExponencial { get;  }

        public AsyncRetryPolicy<HttpResponseMessage> RetryByException { get;  }
        

        public InternoController()
        {
            RetryImediato = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
            .RetryAsync(5);

            WaitAndRetry = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
            .WaitAndRetryAsync(10, (tentativa) => TimeSpan.FromSeconds(2));

            RetryExponencial = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
            .WaitAndRetryAsync(10, (tentativa) => TimeSpan.FromSeconds(Math.Pow(2,tentativa)));

            RetryByException = Policy.Handle<HttpRequestException>()
                               .OrResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
            .WaitAndRetryAsync(10, (tentativa) => TimeSpan.FromSeconds(3));

        }

        [HttpPost]
        public async Task<IActionResult> ExecutarProcessoInterno()
        {

            var client = new HttpClient();
            
            var resposta = await RetryByException.ExecuteAsync(() => {return client.GetAsync("https://localhost:7282/api/externo/30");});

            var msg = "---> SUCESSO !!!";

            if(resposta.IsSuccessStatusCode)
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