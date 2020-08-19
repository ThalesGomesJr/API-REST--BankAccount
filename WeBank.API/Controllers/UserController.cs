using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
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

        [HttpPost("registration")]
        [AllowAnonymous]
        public async Task<IActionResult> Registration(UserRegisterDTO userRegister)
        {
            try
            {
                var user = this._mapper.Map<User>(userRegister);
                user.NumAccount = await this._repo.VerifyNumAccount();
                user.ImageURL = "default.png";
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
        
        [HttpPut("{Id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(int Id, UserUpdateDTO userUpdate)
        {
            try
            {
                var user = await this._userManager.FindByIdAsync(Id.ToString());
                if (user == null) return this.StatusCode(StatusCodes.Status404NotFound, "Usuário não encontrado");

                this._mapper.Map(userUpdate, user);
                
                var result = await this._userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Created($"/api/user/{userUpdate.Id}", this._mapper.Map<UserDTO>(user));
                }    

                return BadRequest(result.Errors);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }
        }

        [HttpPut("updatepassword/{Id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdatePassword(int Id, UserUpdatePasswordDTO userUpdatePassword)
        {
            try
            {
                var user = await this._userManager.FindByIdAsync(Id.ToString());
                if (user == null) return this.StatusCode(StatusCodes.Status404NotFound, "Usuário não encontrado");
                
                await this._userManager.RemovePasswordAsync(user);
                var result = await this._userManager.AddPasswordAsync(user, userUpdatePassword.Password);

                if (result.Succeeded)
                {
                    return Ok();
                }    

                return BadRequest(result.Errors);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources","Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if(file.Length > 0)
                {
                    var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
                    var userName = filename.Replace("\"","").Split("-")[0];
                    var fullPath = Path.Combine(pathToSave, filename.Replace("\""," ").Trim());
                    
                    var user = await this._userManager.FindByNameAsync(userName.ToUpper());
                    user.ImageURL = filename.Replace("\""," ").Trim();

                    await this._userManager.UpdateAsync(user);

                    using(var stream = new FileStream(fullPath, FileMode.Create))
                    {
                       await file.CopyToAsync(stream);
                    }
                }

                return Ok();
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }
            
        }

        [HttpDelete("{Id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                var user = await this._userManager.FindByIdAsync(Id.ToString());
                if (user == null) return this.StatusCode(StatusCodes.Status404NotFound, "Usuário não encontrado");
                var result = await this._userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return Ok();
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

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); 

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

        [HttpGet("getAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
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

        [HttpGet("name/{userName}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserbyName(string userName)
        {
            try
            {
                var user = await this._userManager.FindByNameAsync(userName);
                
                if (user == null) return this.StatusCode(StatusCodes.Status404NotFound, "Usuário não encontrado");

                var results = this._mapper.Map<UserProfileDTO>(user);
        
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }
        }

        [HttpGet("{Id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserbyId(int Id)
        {
            try
            {
                var user = await this._userManager.FindByIdAsync(Id.ToString());
                
                if (user == null) return this.StatusCode(StatusCodes.Status404NotFound, "Usuário não encontrado");

                var results = this._mapper.Map<UserProfileDTO>(user);
                
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }
        }

        [HttpGet("numAccount/{numAccount}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserbyNumAccount(string numAccount)
        {
            try
            {
                var user = await this._repo.GetUserAsyncByNumAccount(numAccount);
                
                if (user == null) return this.StatusCode(StatusCodes.Status404NotFound, "Numero da conta não encontrado");

                var results = this._mapper.Map<UserTransferDTO>(user);
                
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }
        }

    }
}