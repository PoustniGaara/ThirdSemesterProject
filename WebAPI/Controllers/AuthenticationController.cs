﻿using AutoMapper;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.ActionFilters;
using WebApi.DTOs;

namespace WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        #region Properties and constructor
        IUserDataAccess _userDataAccess;
        private readonly IMapper _mapper;


        public AuthenticationController(IUserDataAccess userDataAccess, IMapper mapper)
        {
            _userDataAccess = userDataAccess;
            _mapper = mapper;
        }
        #endregion

        [ServiceFilter(typeof(ValidationFilter))]
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel loginData)
        {
            User? user = await _userDataAccess.LoginAsync(loginData.Email, loginData.Password);
            
            if (user != null)
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: "https://localhost:5001",
                    audience: "https://localhost:5001",
                    claims: new List<Claim>{
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("address",user.Address),
                        new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                        new Claim(ClaimTypes.Name, user. Name),
                        new Claim(ClaimTypes.Surname, user.Surname),
                        new Claim(ClaimTypes.Role, user.IsAdmin.ToString()),
                    },
                    expires: DateTime.Now.AddMinutes(20),
                    signingCredentials: signinCredentials
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new AuthenticatedResponse { Token = tokenString });
            }
            return Unauthorized();
        }
    }
}