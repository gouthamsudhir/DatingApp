using DatingApp.API.Data;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(DataContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers() 
        {
            var users = await context.Users.ToListAsync();

            return Ok(users);
        }

        [HttpGet("user/{userName}")]
        public async Task<ActionResult<AppUser>> GetUserByUserName(string userName)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == userName);

            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            var user = await context.Users.FindAsync(id);

            if (user == null) return NotFound();

            return Ok(user);
        }
    }
}
