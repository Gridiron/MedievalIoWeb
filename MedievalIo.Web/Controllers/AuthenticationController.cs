﻿using System;
using System.Threading.Tasks;
using MedievalIo.Services.Interfaces;
using MedievalIo.Services.Models.Authentication;
using MedievalIoWeb.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedievalIoWeb.Controllers
{
	public class AuthenticationController : ApiController
	{
		private readonly IUserService _userService;

        public AuthenticationController(IUserService userService)
		{
			_userService = userService;
        }

        [HttpGet("User")]
        public AuthenticatedUserModel GetUser()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            return new AuthenticatedUserModel
            {
                Token = UserToken,
                UserId = Id,
                IsAdmin = !string.IsNullOrEmpty(UserRole)
            };
        }

		[HttpPost("Login")]
		public async Task<IActionResult> Login(LoginModel model)
		{
			try
			{
                var result = await _userService.AuthenticateUserAsync(model, AppSettings.UserServiceEndPoint);
				if (result != null)
				{
                    await AuthenticationHelper.CreateSessionCookieAsync(HttpContext, result.Token, result.IsAdmin, result.UserId);

                    var user = new AuthenticatedUserModel
                    {
                        Token = result.Token,
                        UserId = result.UserId,
                        IsAdmin = result.IsAdmin
                    };

					return Ok(user);
                }

				return Unauthorized();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}

		}

        [HttpGet("CookieExpiration")]
        public int GetCookieExpirationTime()
        {
            return AppSettings.CookieExpirationInDays;
        }

		[Authorize]
		[HttpGet("Logout")]
		public async Task<IActionResult> Logout()
		{
			await AuthenticationHelper.RemoveSessionCookieAsync(HttpContext);

			return Ok(true);
		}

        [HttpPost("Register")]
		public async Task<IActionResult> Register(RegistrationModel model)
        {
            var result = await _userService.RegisterUserAsync(model, AppSettings.UserServiceEndPoint);

			return Ok(result.Item1);
		}
    }
}
