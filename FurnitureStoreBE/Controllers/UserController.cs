using FurnitureStoreBE.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStoreBE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }
        [HttpGet()]
        //[Authorize(Policy = "CreateUserPolicy")]
        public string getVoid()
        {
            _context.Colors.Add(new Models.Color { ColorName = "ABC" });
            _context.SaveChanges();
            return "OK";
        }
    }
}
