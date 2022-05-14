using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Todo.Models;

namespace Todo.Helpers.Authentication
{
    public class TestAuthenticationHandler : IAuthenticationSignInHandler
    {
        //驗證方案名稱
        public const string TEST_SCHEM_NAME = "some_authen";

        public TestAuthenticationOptions Options { get; private set; }

        public TestAuthenticationHandler(IOptions<TestAuthenticationOptions> opt)
        {
            Options = opt.Value;
        }

        public HttpContext HttpContext { get; private set; }
        public AuthenticationScheme Scheme { get; private set; }

        /// <summary>
        /// 驗證過程
        /// </summary>
        /// <returns>驗證結果</returns>
        public Task<AuthenticateResult> AuthenticateAsync()
        {
            if (Scheme.Name != TEST_SCHEM_NAME)
            {
                return Task.FromResult(AuthenticateResult.Fail("驗證方案不匹配"));
            }

            if (!HttpContext.Session.Keys.Contains(Options.SessionKeyName))
            {
                return Task.FromResult(AuthenticateResult.Fail("Session無效"));
            }

            #region 驗證通過

            //獲取使用者名稱
            string un = HttpContext.Session.GetString(Options.SessionKeyName) ?? string.Empty;

            //建立使用者資訊
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, un) };
            ClaimsIdentity id = new ClaimsIdentity(claims, TEST_SCHEM_NAME);
            ClaimsPrincipal prcp = new ClaimsPrincipal(id);
            AuthenticationTicket ticket = new AuthenticationTicket(prcp, TEST_SCHEM_NAME);
            return Task.FromResult(AuthenticateResult.Success(ticket));

            #endregion
        }

        /// <summary>
        /// 驗證失敗，使用者登入驗證失敗會跑這
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            HttpContext.Response.Redirect(Options.LoginPath);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 禁止訪問，沒有權限(Roles)的會跑這裡
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Task ForbidAsync(AuthenticationProperties properties)
        {
            //跳轉到沒有權限的頁面
            HttpContext.Response.Redirect(Options.ReturnUrlKey);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 初始化，獲取一些必備物件的引用
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            HttpContext = context;
            Scheme = scheme;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="user"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            //獲取使用者名稱
            string uname = user.Identity?.Name ?? string.Empty;

            if (!string.IsNullOrEmpty(uname))
            {
                HttpContext.Session.SetString(Options.SessionKeyName, uname);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Task SignOutAsync(AuthenticationProperties properties)
        {
            if (HttpContext.Session.Keys.Contains(Options.SessionKeyName))
            {
                HttpContext.Session.Remove(Options.SessionKeyName);
            }
            return Task.CompletedTask;
        }
    }
}
