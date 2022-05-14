using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Dto;
using Todo.Helpers;
using Todo.Helpers.Authentication;
using Todo.Models;
using Todo.Service;

namespace Todo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo", Version = "v1" });
            });

            services.AddDbContext<TodoContext>(options => options.UseSqlServer(Configuration.GetConnectionString("TodoDatabase")));
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped(typeof(TodoListService));

            //取得使用者資訊會用到
            services.AddHttpContextAccessor();

            #region 啟用Session功能
            services.AddSession(options =>
            {
                //修改合理的 Session 到期時間
                options.IdleTimeout = TimeSpan.FromMinutes(5);

                //限制只有在 HTTPS 連線的情況下，才允許使用 Session。如此一來變成加密連線，就不容易被攔截。
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;

                //如果 cookie 必須無法由用戶端腳本存取，則為 true;否則為 false。
                options.Cookie.HttpOnly = true;

                //指出此 cookie 是否為應用程式正確運作的必要項。 若為 true，則可能略過同意原則檢查
                options.Cookie.IsEssential = true;

                //更改Session 名稱
                options.Cookie.Name = "session.user";
            });

            //將Session放置到伺服器的內建記憶體
            services.AddDistributedMemoryCache();
            #endregion

            #region 自訂驗證方案
            services.AddOptions<TestAuthenticationOptions>();

            // 新增驗證功能
            services.AddAuthentication(option =>
            {
                // 新增我們自定義的驗證方案名
                option.AddScheme<TestAuthenticationHandler>(TestAuthenticationHandler.TEST_SCHEM_NAME, null);
            });
            #endregion

            #region 改寫內建授權功能，並指定自訂的驗證方案
            
            // 新增授權功能
            services.AddAuthorization(option =>
            {
                // 註冊授權策略，名為“demo2”
                option.AddPolicy("demo2", c =>
                {
                    // 與我們前面定義的驗證方案繫結
                    // 授權過程跟隨該驗證後發生
                    // 要注意指定驗證方案
                    c.AddAuthenticationSchemes(TestAuthenticationHandler.TEST_SCHEM_NAME);

                    // 要求存在已登入使用者的標識
                    c.RequireAuthenticatedUser();
                });
            });
            #endregion

            services.AddMvc(options =>
            {
                //所有controller都要驗證
                options.Filters.Add(new AuthorizeFilter());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseStaticFiles();

            #region Session驗證功能要加以下3項
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
