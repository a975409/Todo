using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Todo.Helpers.Authentication
{
    public class TestAuthenticationOptions
    {
        /// <summary>
        /// 未登入時會自動導到這個網址
        /// </summary>
        public string LoginPath { get; set; } = "/api/Login/NoLogin";

        /// <summary>
        /// 沒有權限時會自動導到這個網址
        /// </summary>
        public string ReturnUrlKey { set; get; } = "/api/Login/NoAccess";
    }
}
