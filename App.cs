using System;
using Serilog;
using Tallan.QuickStart.Config.Config;

namespace Tallan.QuickStart.Config
{
	public class App
	{
		private readonly AppConfig _config;
		private readonly ILogger _logger;

		public App(AppConfig config, ILogger logger)
		{
			_config = config;
			_logger = logger;
		}

		public void Run()
		{
			try
			{
				_logger.Information("Contents of AppConfig: ");
				_logger.Information("AppConfig:Azure:ClientId: {ClientId}", _config.Azure.ClientId);
				_logger.Information("AppConfig:Azure:Secret: {Secret}", _config.Azure.Secret);
				_logger.Information("AppConfig:Azure:TenantId: {TenantId}", _config.Azure.TenantId);
				_logger.Information("");
				_logger.Information("AppConfig:Cache:CacheType: {CacheType}", _config.Cache.CacheType);
				_logger.Information("AppConfig:Cache:RedisClientId: {RedisClientId}", _config.Cache.RedisClientId);
				_logger.Information("AppConfig:Cache:RedisEndpoint: {RedisEndpoint}", _config.Cache.RedisEndpoint);
				_logger.Information("AppConfig:Cache:RedisSecret: {RedisSecret}", _config.Cache.RedisSecret);
				_logger.Information("AppConfig:Cache:RedisServiceName: {RedisServiceName}", _config.Cache.RedisServiceName);
				_logger.Information("");
				_logger.Information("AppConfig:Database:ConnectionString: {ConnectionString}", _config.Database.ConnectionString);
				_logger.Information("");
				_logger.Information("AppConfig:Logging:EnableSensitiveDataLogging: {EnableSensitiveDataLogging}", _config.Logging.EnableSensitiveDataLogging);
				_logger.Information("AppConfig:Logging:LogEntityFrameworkCalls: {LogEntityFrameworkCalls}", _config.Logging.LogEntityFrameworkCalls);
				_logger.Information("AppConfig:Logging:VerboseLogging: {VerboseLogging}", _config.Logging.VerboseLogging);
				_logger.Information("");
				_logger.Information("Done");
				Console.ReadLine();
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "error");
				Console.ReadLine();
			}
		}
	}
}
