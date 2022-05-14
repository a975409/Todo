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
using Todo.Helpers;
using Todo.Helpers.Authentication;
using Todo.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly TodoContext _todoContext;

        public LoginController(TodoContext todoContext) {
            _todoContext = todoContext;
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccountDto value)
        {
            var result = _todoContext.Employees.Where(m => m.Account == value.Account && m.Password == value.Password).FirstOrDefault();

            if (result == null)
                return StatusCode(302, "登入失敗，帳號密碼錯誤");

            //塞入使用者資訊
            var claims = new List<Claim> {
                             new Claim(ClaimTypes.Name,result.Account),//使用者名稱
                             new Claim("FullName",result.Name),
                             new Claim("EmployeeId",result.EmployeeId.ToString())
                            };

            //取得權限(Roles)
            var Roles = _todoContext.Roles.Where(m => m.EmployeeId == result.EmployeeId);

            //新增權限(Roles)
            foreach (var role in Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            // 要注意指定驗證方案
            var claimsIdentity = new ClaimsIdentity(claims, TestAuthenticationHandler.TEST_SCHEM_NAME);
            await HttpContext.SignInAsync(TestAuthenticationHandler.TEST_SCHEM_NAME, new ClaimsPrincipal(claimsIdentity));
            return Ok("登入成功");
        }

        [HttpDelete]
        public async Task<IActionResult> Logout()
        {
            // 要注意指定驗證方案
            await HttpContext.SignOutAsync(TestAuthenticationHandler.TEST_SCHEM_NAME);
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
    }
}
