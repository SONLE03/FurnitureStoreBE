using Azure.Core;
using Elfie.Serialization;
using FurnitureStoreBE.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStoreBE.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet()]
        public string getVoid()
        {
            throw new IOException("abcd");
        }
    }
}
