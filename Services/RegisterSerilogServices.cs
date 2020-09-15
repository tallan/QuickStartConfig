using System;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;

namespace Tallan.QuickStart.Config.Services
{
	public static class RegisterSerilogServices
	{
		/// <summary>
		/// Register the Serilog service with a custom configuration.
		/// </summary>
		public static IServiceCollection AddSerilogServices(this IServiceCollection services, LoggerConfiguration configuration)
		{
			Log.Logger = configuration.CreateLogger();
			AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
			return services.AddSingleton(Log.Logger);
		}

		/// <summary>
		/// Register the Serilog service for console logging only.
		/// </summary>
		public static IServiceCollection AddSerilogServicesConsole(this IServiceCollection services)
		{
			return services.AddSerilogServices(
				new LoggerConfiguration()
					.Enrich.WithExceptionDetails()
					.MinimumLevel.Verbose()
					.WriteTo.Console());
		}
	}
}
