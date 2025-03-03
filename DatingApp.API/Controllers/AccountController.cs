using System.Security.Cryptography;
using System.Text;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) 
        {
            if (await IsUsernameExists(registerDto.Username)) return BadRequest("Username already exists!");

            using var hmac = new HMACSHA512();

            var user = new AppUser{
                UserName = registerDto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            context.Add(user);
            await context.SaveChangesAsync();

            return new UserDto{
                Username = user.UserName,
                Token = tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) 
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null) return Unauthorized("Username or Password not found!");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Username or Password not found!");
            }

            return new UserDto{
                Username = user.UserName,
                Token = tokenService.CreateToken(user)
            };
        }

        public async Task<bool> IsUsernameExists(string userName) 
        {
            return await context.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower());
        }        
    }
}
