using Joao.API.Controllers;
using Joao.Business.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace Joao.API.V1.Controllers
{
    
    [ApiVersion("1.0", Deprecated = true)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TesteController : MainController
    {
        public TesteController(INotificador notificador, IUser appUser) : base(notificador, appUser)
        {
        }

        [HttpGet]
        public string Valor()
        {
            return "V1";
        }
 
    }
}
