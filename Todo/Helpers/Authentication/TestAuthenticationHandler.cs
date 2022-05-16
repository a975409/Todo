using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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
        /// 驗證過程，主要是檢查Session是否有存放任何Key
        /// </summary>
        /// <returns>驗證結果</returns>
        public Task<AuthenticateResult> AuthenticateAsync()
        {
            if (Scheme.Name != TEST_SCHEM_NAME)
            {
                return Task.FromResult(AuthenticateResult.Fail("驗證方案不匹配"));
            }

            if (!HttpContext.Session.Keys.Any())
            {
                return Task.FromResult(AuthenticateResult.Fail("Session無效"));
            }

            #region 驗證通過

            //要注意指定驗證方案
            ClaimsIdentity id = new ClaimsIdentity(TEST_SCHEM_NAME);

            //從Session獲取使用者資訊
            foreach (var key in HttpContext.Session.Keys)
            {
                id.AddClaim(new Claim(key, HttpContext.Session.GetString(key)));
            }

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
            //將在LoginController的Login設定的使用者資訊(Claims)，都存到session裡面，之後每次的Request只要從Session取出使用者資訊就行
            foreach (var claim in user.Claims)
            {
                HttpContext.Session.SetString(claim.Type, claim.Value);
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
            //移除存在Session裡面所有使用者資訊
            foreach (var key in HttpContext.Session.Keys)
            {
                HttpContext.Session.Remove(key);
            }

            return Task.CompletedTask;
        }
    }
}
