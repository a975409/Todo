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

            //���o�ϥΪ̸�T�|�Ψ�
            services.AddHttpContextAccessor();

            #region �ҥ�Session�\��
            services.AddSession(options =>
            {
                //�ק�X�z�� Session ����ɶ�
                options.IdleTimeout = TimeSpan.FromMinutes(5);

                //����u���b HTTPS �s�u�����p�U�A�~���\�ϥ� Session�C�p���@���ܦ��[�K�s�u�A�N���e���Q�d�I�C
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;

                //�p�G cookie �����L�k�ѥΤ�ݸ}���s���A�h�� true;�_�h�� false�C
                options.Cookie.HttpOnly = true;

                //���X�� cookie �O�_�����ε{�����T�B�@�����n���C �Y�� true�A�h�i�ತ�L�P�N��h�ˬd
                options.Cookie.IsEssential = true;

                //���Session �W��
                options.Cookie.Name = "session.user";
            });

            //�NSession��m����A�������ذO����
            services.AddDistributedMemoryCache();
            #endregion

            #region �ۭq���Ҥ��
            services.AddOptions<TestAuthenticationOptions>();

            // �s�W���ҥ\��
            services.AddAuthentication(option =>
            {
                // �s�W�ڭ̦۩w�q�����Ҥ�צW
                option.AddScheme<TestAuthenticationHandler>(TestAuthenticationHandler.TEST_SCHEM_NAME, null);
            });
            #endregion

            #region ��g���ر��v�\��A�ë��w�ۭq�����Ҥ��
            
            // �s�W���v�\��
            services.AddAuthorization(option =>
            {
                // ���U���v�����A�W����demo2��
                option.AddPolicy("demo2", c =>
                {
                    // �P�ڭ̫e���w�q�����Ҥ��ô��
                    // ���v�L�{���H�����ҫ�o��
                    // �n�`�N���w���Ҥ��
                    c.AddAuthenticationSchemes(TestAuthenticationHandler.TEST_SCHEM_NAME);

                    // �n�D�s�b�w�n�J�ϥΪ̪�����
                    c.RequireAuthenticatedUser();
                });
            });
            #endregion

            services.AddMvc(options =>
            {
                //�Ҧ�controller���n����
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

            #region Session���ҥ\��n�[�H�U3��
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
