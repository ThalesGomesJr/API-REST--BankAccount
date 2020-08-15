using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WeBank.API.DTOs;
using WeBank.Domain.Models;
using WeBank.Repository;

namespace WeBank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IWeBankRepository _repo;
        private readonly IConfiguration _config;
        public readonly UserManager<User> _userManager;
        public readonly SignInManager<User> _signInManager;
        public readonly IMapper _mapper;
        public UserController(IWeBankRepository repo, IConfiguration config, UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper)
        {
            this._repo = repo;
            this._mapper = mapper;
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._config = config;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserRegisterDTO userRegister)
        {
            try
            {
                var user = this._mapper.Map<User>(userRegister);
                user.NumAccount = await this._repo.VerifyNumAccount();
                var result = await this._userManager.CreateAsync(user, userRegister.password);
                
                if (result.Succeeded)
                {
                    return Created($"/api/user/{userRegister.Id}", this._mapper.Map<UserDTO>(user));
                }    

                return BadRequest(result.Errors);
            }

            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDTO userLogin)
        {
            try
            {
                var user = await this._userManager.FindByNameAsync(userLogin.UserName);
                var result = await this._signInManager.CheckPasswordSignInAsync(user, userLogin.Password, false);

                if (result.Succeeded)
                {
                    var bankUser = await this._userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == userLogin.UserName.ToUpper());
                    var userToReturn = this._mapper.Map<UserLoginDTO>(bankUser);

                    return Ok(new{
                        token = GenerateJWToken(bankUser).Result,
                        user = userToReturn
                    });
                }

                return Unauthorized();
            }

            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }
        }

        //Gera o token para login
        private async Task<String> GenerateJWToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await this._userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));    
            }

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this._config.GetSection("AppSettings:Token").Value)); 

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha384Signature); 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        [HttpDelete("{Id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                var evento = await this._repo.GetUserAsyncById(Id);
                if (evento == null) return this.StatusCode(StatusCodes.Status404NotFound, "Usuário não encontrado");

                this._repo.Delete(evento);

                if (await _repo.SaveChangesAsync())
                {
                    return Ok();
                }

            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }

            return BadRequest();
        }

        [HttpGet("get")]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            try
            {
                var users = await this._repo.GetAllUserAsync();
                var results = this._mapper.Map<UserDTO[]>(users);
        
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetUserbyId(int id)
        {
            try
            {
                var user = await this._repo.GetUserAsyncById(id);
                
                if (user == null) return this.StatusCode(StatusCodes.Status404NotFound, "Usuário não encontrado");

                var results = this._mapper.Map<UserDTO>(user);
                
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }

        }
    }
}