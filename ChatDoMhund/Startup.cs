using ChatDoMhund.Data.Repository;
using ChatDoMhund.Hubs;
using ChatDoMhund.Models.Domain;
using ChatDoMhund.Models.Infra;
using ChatDoMhund.Models.Infra.Filter;
using ChatDoMhund.Models.Tratamento;
using HelperMhundCore31;
using HelperMhundCore31.Data.Entity.Partials;
using HelperSaeCore31.Models.Infra.ControllerComponents;
using HelperSaeCore31.Models.Infra.ControllerComponents.Interface;
using HelperSaeCore31.Models.Infra.Cookie;
using HelperSaeCore31.Models.Infra.Cookie.Interface;
using HelperSaeCore31.Models.Infra.Criptography;
using HelperSaeCore31.Models.Infra.Session;
using HelperSaeCore31.Models.Infra.Session.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ChatDoMhund
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = context => false;
				options.MinimumSameSitePolicy = SameSiteMode.Lax;
			});

			services.AddControllersWithViews();
			services.AddSession();
			services.AddHttpContextAccessor();
			services.AddMvc(options =>
			{
				options.Filters.Add<LoginFilterAttribute>();
			}).SetCompatibilityVersion(CompatibilityVersion.Latest);


			services.AddRazorPages()
				.AddRazorRuntimeCompilation();

			services
				.AddSingleton<SaeCriptography>()
				.AddScoped<ISaeHelperCookie, SaeHelperCookie>()
				.AddScoped<ISaeHelperSession, SaeHelperSession>()
				.AddTransient<IHttpContextAccessor, HttpContextAccessor>()
				.AddTransient<IViewRenderService, ViewRenderService>()
				.AddTransient<ConnectionManager>()
				.AddTransient<MhundDbContext>()
				.AddTransient<UsuarioLogado>()
				//Repositórios
				.AddTransient<AlunosRepository>()
				.AddTransient<AppCfgRepository>()
				.AddTransient<CadforpsRepository>()
				.AddTransient<PessoasRepository>()
				.AddTransient<ChatProfessRepository>()
				.AddTransient<ProfHabilitaRepository>()
				.AddTransient<ChatLogRepository>()
				.AddTransient<GroupBuilder>()
				//Domínios
				.AddTransient<ChatDomain>()
				.AddTransient<PesquisarContatosDomain>()
				;

			services.AddSignalR();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			//app.UseExceptionHandler("/Error/HandleError?code=500");
			app.UseExceptionHandler("/Error/HandleError");
			if (!Util.EhDebug())
			{
				//todo: ver um jeito de pegar qual recurso deu 404
				//todo: desabilitei em debug para facilitar minha vida enquanto não corrige
				app.UseStatusCodePagesWithRedirects("/Error/HandleError?code={0}");
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseCookiePolicy();
			app.UseSession();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Chat}/{action=Index}/{id?}");
				endpoints.MapHub<ChatHub>("/chathub");
			});
		}
	}
}
