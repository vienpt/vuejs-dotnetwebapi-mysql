using System;
using System.Collections.Generic;
using System.IO;
using AccountOwnerServer.Extensions;
using Contracts;
using LoggerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;

namespace AccountOwnerServer
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			// var a = Directory.GetCurrentDirectory();
			LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(),"/nlog.config"));

			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.ConfigureCors();
			services.ConfigureIISIntegration();
			services.ConfigureLoggerService();
			services.ConfigureMySqlContext(Configuration);

			services.AddControllers();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			// add Cors

			// app.UseStaticFiles() enables using static files for the request.
			// If we don’t set a path to the static files, it will use a wwwroot folder in our solution explorer by default.
			app.UseStaticFiles();

			app.UseCors("CorsPolicy");
			// app.UseForwardedHeaders will forward proxy headers to the current request.
			// This will help us during the "Linux deployment".
			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.All
			});
			// end Cors

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
