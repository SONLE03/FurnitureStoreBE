using Azure.Core;
using Elfie.Serialization;
using FurnitureStoreBE.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStoreBE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet()]
        [Authorize(Policy = "CreateUserPolicy")]
        public string getVoid()
        {
            return "10";
        }
    }
}
