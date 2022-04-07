using Joao.API.Controllers;
using Joao.Business.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace Joao.API.V2.Controllers
{
    
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TesteController : MainController
    {
        public TesteController(INotificador notificador, IUser appUser) : base(notificador, appUser)
        {
        }

        [HttpGet]
        public string Valor()
        {
            return "V2";
        }
 
    }
}
