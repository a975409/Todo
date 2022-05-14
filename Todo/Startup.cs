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

            //啟用Session功能
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                //options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                //options.Cookie.HttpOnly = true;
                //options.Cookie.IsEssential = true;
                options.Cookie.Name = "session.user";
            });

            //將Session放置到伺服器的內建記憶體
            services.AddDistributedMemoryCache();

            services.AddOptions<TestAuthenticationOptions>();

            // 新增驗證功能
            services.AddAuthentication(option =>
            {
                // 新增我們自定義的驗證方案名
                option.AddScheme<TestAuthenticationHandler>(TestAuthenticationHandler.TEST_SCHEM_NAME, null);
            });

            // 新增授權功能
            services.AddAuthorization(option =>
            {
                option.AddPolicy("demo2", c =>
                {
                    // 與我們前面定義的驗證方案繫結
                    // 授權過程跟隨該驗證後發生
                    c.AddAuthenticationSchemes(TestAuthenticationHandler.TEST_SCHEM_NAME);

                    // 要求存在已登入使用者的標識
                    c.RequireAuthenticatedUser();
                });
            });

            
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
            //app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
