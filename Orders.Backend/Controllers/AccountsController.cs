﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Orders.Backend.Helpers;
using Orders.Backend.Helpers.Orders.Backend.Helpers;
using Orders.Backend.Repositories;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Backend.Controllers
{
    [ApiController]
    [Route("/api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly IFileStorage _fileStorage;
        private readonly IMailHelper _mailHelper;
        private readonly IUsersRepository _usersRepository;
        private readonly string _container;

        public AccountsController(IUserHelper userHelper, IConfiguration configuration, IFileStorage fileStorage, IMailHelper mailHelper, IUsersRepository usersRepository)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _fileStorage = fileStorage;
            _mailHelper = mailHelper;
            _usersRepository = usersRepository;
            _container = "users";
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _usersRepository.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalPages")]
        public async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var action = await _usersRepository.GetTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO model)
        {
            User user = model;
            if (!string.IsNullOrEmpty(model.Photo))
            {
                var photoUser = Convert.FromBase64String(model.Photo);
                model.Photo = await _fileStorage.SaveFileAsync(photoUser, ".jpg", _container);
            }

            var result = await _userHelper.AddUserAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userHelper.AddUserToRoleAsync(user, user.UserType.ToString());
                var response = await SendConfirmationEmailAsync(user);
                if (response.WasSuccess)
                {
                    return NoContent();
                }

                return BadRequest(response.Message);
            }

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
        {
            token = token.Replace(" ", "+");
            var user = await _userHelper.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault());
            }

            return NoContent();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var result = await _userHelper.LoginAsync(model);
            if (result.Succeeded)
            {
                var user = await _userHelper.GetUserAsync(model.Email);
                return Ok(BuildToken(user));
            }

            if (result.IsLockedOut)
            {
                return BadRequest("Ha superado el máximo número de intentos, su cuenta está bloqueada, intente de nuevo en 5 minutos.");
            }

            if (result.IsNotAllowed)
            {
                return BadRequest("El usuario no ha sido habilitado, debes de seguir las instrucciones del correo enviado para poder habilitar el usuario.");
            }

            return BadRequest("Email o contraseña incorrectos.");
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Put(User user)
        {
            try
            {
                var currentUser = await _userHelper.GetUserAsync(User.Identity!.Name!);
                if (currentUser == null)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(user.Photo))
                {
                    var photoUser = Convert.FromBase64String(user.Photo);
                    user.Photo = await _fileStorage.SaveFileAsync(photoUser, ".jpg", _container);
                }

                currentUser.Document = user.Document;
                currentUser.FirstName = user.FirstName;
                currentUser.LastName = user.LastName;
                currentUser.Address = user.Address;
                currentUser.PhoneNumber = user.PhoneNumber;
                currentUser.Photo = !string.IsNullOrEmpty(user.Photo) && user.Photo != currentUser.Photo ? user.Photo : currentUser.Photo;
                currentUser.CityId = user.CityId;

                var result = await _userHelper.UpdateUserAsync(currentUser);
                if (result.Succeeded)
                {
                    return Ok(BuildToken(currentUser));
                }

                return BadRequest(result.Errors.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RecoverPassword")]
        public async Task<IActionResult> RecoverPassword([FromBody] EmailDTO model)
        {
            var user = await _userHelper.GetUserAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
            var tokenLink = Url.Action("ResetPassword", "accounts", new
            {
                userid = user.Id,
                token = myToken
            }, HttpContext.Request.Scheme, _configuration["Url Frontend"]);

            var response = _mailHelper.SendMail(user.FullName, user.Email!,
                $"Orders - Recuperación de contraseña",
                $"<h1>Orders - Recuperación de contraseña</h1>" +
                $"<p>Para recuperar su contraseña, por favor hacer clic 'Recuperar Contraseña':</p>" +
                $"<b><a href ={tokenLink}>Recuperar Contraseña</a></b>");

            if (response.WasSuccess)
            {
                return NoContent();
            }

            return BadRequest(response.Message);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            var user = await _userHelper.GetUserAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(result.Errors.FirstOrDefault()!.Description);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Get()
        {
            return Ok(await _userHelper.GetUserAsync(User.Identity!.Name!));
        }

        [HttpPost("changePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()!.Description);
            }

            return NoContent();
        }

        [HttpPost("ResedToken")]
        public async Task<IActionResult> ResedToken([FromBody] EmailDTO model)
        {
            User user = await _userHelper.GetUserAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            var response = await SendConfirmationEmailAsync(user);
            if (response.WasSuccess)
            {
                return NoContent();
            }

            return BadRequest(response.Message);
        }

        private TokenDTO BuildToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim(ClaimTypes.Role, user.UserType.ToString()),
                new Claim("Document", user.Document),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("Address", user.Address),
                new Claim("Photo", user.Photo ?? string.Empty),
                new Claim("CityId", user.CityId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddDays(30);
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new TokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }

        private async Task<Response<string>> SendConfirmationEmailAsync(User user)
        {
            var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            var tokenLink = Url.Action("ConfirmEmail", "accounts", new
            {
                userid = user.Id,
                token = myToken
            }, HttpContext.Request.Scheme, _configuration["Url Frontend"]);

            return _mailHelper.SendMail(user.FullName, user.Email!,
                $"Orders - Confirmación de cuenta",
                $"<h1>Orders - Confirmación de cuenta</h1>" +
                $"<p>Para habilitar el usuario, por favor hacer clic 'Confirmar Email':</p>" +
                $"<b><a href ={tokenLink}>Confirmar Email</a></b>");
        }
    }
}