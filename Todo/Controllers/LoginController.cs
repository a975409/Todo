using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Todo.Dto;
using Todo.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TodoContext _todoContext;

        public LoginController(TodoContext todoContext, IConfiguration configuration) {
            _todoContext = todoContext;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Login(AccountDto value)
        {
            var result = _todoContext.Employees.Where(m => m.Account == value.Account && m.Password == value.Password).FirstOrDefault();

            if (result == null)
                return StatusCode(302, "登入失敗，帳號密碼錯誤");


            var claims = new List<Claim> {
                             new Claim(ClaimTypes.Name,result.Account),
                             new Claim("FullName",result.Name),
                             new Claim("EmployeeId",result.EmployeeId.ToString())
                            };

            var Roles = _todoContext.Roles.Where(m => m.EmployeeId == result.EmployeeId);

            foreach (var role in Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return Ok("登入成功");
        }

        [HttpDelete]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("已登出");
        }

        [HttpGet("NoLogin")]
        public IActionResult NoLogin()
        {
            return Ok("未登入");
        }

        [HttpGet("NoAccess")]
        public IActionResult NoAccess()
        {
            return Ok("沒有權限");
        }

        [HttpPost("jwtLogin")]
        public IActionResult jwtLogin(AccountDto value)
        {
            var result = _todoContext.Employees.Where(m => m.Account == value.Account && m.Password == value.Password).FirstOrDefault();

            if (result == null)
                return StatusCode(302, "登入失敗，帳號密碼錯誤");


            var claims = new List<Claim> {
                             new Claim(JwtRegisteredClaimNames.Email,result.Account),
                             new Claim("FullName",result.Name),
                             new Claim(JwtRegisteredClaimNames.NameId,result.EmployeeId.ToString())
                            };

            var Roles = _todoContext.Roles.Where(m => m.EmployeeId == result.EmployeeId);

            foreach (var role in Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            //取出appsettings.json裡的KEY處理
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:KEY"]));

            //設定jwt相關資訊
            var jwt = new JwtSecurityToken
            (
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            );

            //產生JWT Token
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            //回傳JWT Token給認證通過的使用者
            return Ok(token);
        }
    }
}
