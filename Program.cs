using Azure.Identity;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using Tallan.QuickStart.Config.Config;
using Tallan.QuickStart.Config.Services;

namespace Tallan.QuickStart.Config
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// Creates the new instance of IServiceCollection which is the main DI component
			var services = ConfigureServices();

			// Initializes a service provider
			var serviceProvider = services.BuildServiceProvider();

			// Calls the "run" method on the App class to start the program
			serviceProvider.GetService<App>().Run();
		}

		/// <summary>
		/// This helps to configure all of the services for the application
		/// 
		/// Some Helpful Links:
		///		Logging in EF Core - https://www.entityframeworktutorial.net/efcore/logging-in-entityframework-core.aspx
		///		Adding Other Project Services - https://stackoverflow.com/questions/40306928/implement-dependency-injection-outside-of-startup-cs
		/// </summary>
		/// <returns></returns>
		private static IServiceCollection ConfigureServices()
		{
			IServiceCollection services = new ServiceCollection();

			// Add Configuration
			var config = LoadConfiguration();
			var appConfig = new AppConfig();
			services.AddOptions();
			config.Bind("AppConfig", appConfig);
			services.AddSingleton(appConfig);

			// Add Serilog
			services.AddSerilogServicesConsole();

			// Add a LoggerFactory for Entity Framework if our App Config says to do so
			// Note that you will also need to make a change to your DBContext class see **Logging in EF Core**
			if (appConfig.Logging.LogEntityFrameworkCalls)
				services.AddSingleton<ILoggerFactory>(svs => new SerilogLoggerFactory());

			// Configure the cache
			ConfigureCache(services, appConfig);

			// Add other services - see **Adding Other Project Services**
			// You can extend the IServiceCollection, by simply creating a method that returns IServiceCollection and takes in (this IServicesCollection)
			// Once that is done it's as simple as calling the new method in your Startup.cs.  In addition you can register all of your dependencies
			// either by manually entering them or by Assembly Scanning
			// The below is an example of a say to call it where "OtherServiceName" is the name of the method in another project
			// ToDo: Put in Assembly Scanning Example? (Pros and Cons of both)
			// services.OtherServiceName();

			// Call the App
			services.AddTransient<App>();

			return services;
		}

		/// <summary>
		/// This is the main method for loading in configuration
		/// 
		/// Some Helpful Links:
		///		Secret Manager - https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&amp;tabs=windows#secret-manager
		///		Environment Variables - https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-3.1#environments
		/// </summary>
		/// <returns></returns>
		public static IConfiguration LoadConfiguration()
		{
			// We need to know if we are debugging in a local environment so the #if DEBUG statement will tell us
			var debug = false;
			#if DEBUG
				debug = true;
			#endif

			// This is the official configuration builder object
			var builder = new ConfigurationBuilder();

			// We need to load an initial config before we can build out the full config
			var initialConfigBuilder = new ConfigurationBuilder();

			// We will store our initial config in environment variables
			initialConfigBuilder.AddEnvironmentVariables();

			// Add any User Secrets
			// These are local secrets that are created in your secrets.json
			// See the link above on **Secret Manager**

			// This example is here for a web app
			//var env = hostingContext.HostingEnvironment;
			//var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));

			// Use this example if a console app
			var appAssembly = Assembly.GetExecutingAssembly();
			initialConfigBuilder.AddUserSecrets(appAssembly, optional: true);

			// Build our initial config
			// We do this so we can have access to configuration variables that we need to
			// access other configs later on.
			var initialConfig = initialConfigBuilder.Build();

			// Set Base Path, this is for loading of file based resources
			builder.SetBasePath(Directory.GetCurrentDirectory());

			// We will store our initial config in environment variables
			builder.AddEnvironmentVariables();

			// Add any User Secrets
			// These are local secrets that are created in your secrets.json
			builder.AddUserSecrets(appAssembly, optional: true);

			// Add any JSON files we have
			// Do not forget to set "Copy to Output Directory" on this file in your solution
			builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

			// In ASP.NET you could use different appsettings for different environments using the 
			// IWebHostEnvironment variable - See **Environment Variables** above.
			//builder.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);

			// If you are not using Azure Key Vault you can remove this method
			if (initialConfig["AzureKeyVaultUri"] != null)
				AddAzureKeyVault(debug, builder, initialConfig);

			// If you are not using Azure App Config you can remove this method
			if ((debug && !string.IsNullOrEmpty(initialConfig["AzureAppConfigConnectionString"])) || 
			    (!debug && !string.IsNullOrEmpty(initialConfig["AzureKeyVaultUri"]) && !string.IsNullOrEmpty(initialConfig["AppId"])))
				AddAzureAppConfig(debug, builder, initialConfig);

			return builder.Build();
		}

		/// <summary>
		/// Add Azure App Config
		///
		/// You must have a secrets.json defined!
		///
		/// Some Helpful Links:
		///		Add a Key Vault Reference to App Configuration - https://docs.microsoft.com/en-us/azure/azure-app-configuration/use-key-vault-references-dotnet-core?tabs=cmd%2Ccore2x#add-a-key-vault-reference-to-app-configuration
		///		App Config Best Practices - https://docs.microsoft.com/en-us/azure/azure-app-configuration/howto-best-practices
		///		Feature Flags - https://docs.microsoft.com/en-us/azure/azure-app-configuration/concept-feature-management
		///		Feature Flogs How To - https://docs.microsoft.com/en-us/azure/azure-app-configuration/howto-feature-filters-aspnet-core
		///		Assign a Azure Config Access Policy - https://docs.microsoft.com/en-us/azure/azure-app-configuration/howto-integrate-azure-managed-service-identity?tabs=core2x
		/// </summary>
		/// <param name="debug">Check if debug for local settings vs. production</param>
		/// <param name="builder">The configuration builder object</param>
		/// <param name="initialConfig">The initial config that holds the app config objects</param>
		private static void AddAzureAppConfig(bool debug, ConfigurationBuilder builder, IConfigurationRoot initialConfig)
		{
			// You can call these variables anything you like, this is just what I defaulted them to
			// Please note that when you define a key in Azure App config it should be done with the following structure
			// Example: AppConfig:Database:ConnectionString
			// If you structure it like so it will automatically map to the AppConfig.cs object.  For this example it would map to AppConfig.Database.ConnectionString.
			try
			{
				Action<AzureAppConfigurationOptions> azureAppConfigOptions;
				if (debug)
				{
					//ToDo: Look into Developer Credential Version here.
					azureAppConfigOptions = (options =>
					{
						// If you are using Azure Key Vault, you can connect it into App Config, see **Add a Key Vault Reference to App Configuration**
						if (!string.IsNullOrEmpty(initialConfig["TenantId"]) && !string.IsNullOrEmpty(initialConfig["AppId"]) && !string.IsNullOrEmpty(initialConfig["AppSecret"]))
							options.ConfigureKeyVault(kv => { kv.SetCredential(new ClientSecretCredential(initialConfig["TenantId"], initialConfig["AppId"], initialConfig["AppSecret"])); });

						options.Connect(initialConfig["AzureAppConfigConnectionString"]);
					});
				}
				else
				{
					// This assumes you have setup a service principle to access the App Config on behalf of this app, see **Assign a Key Vault Access Policy** above
					azureAppConfigOptions = (options =>
					{
						var credentials = new ManagedIdentityCredential(initialConfig["AppId"]);

						// If you are using Azure Key Vault, you can connect it into App Config, see **Assign a Azure Config Access Policy**
						options.ConfigureKeyVault(kv => { kv.SetCredential(credentials); });

						options.Connect(new Uri(initialConfig["AzureKeyVaultUri"]), credentials);
					});
				}

				builder.AddAzureAppConfiguration(azureAppConfigOptions);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		/// <summary>
		/// Add Key Vault
		///
		/// You must have a secrets.json defined!
		///
		/// Some Helpful Links:
		///		Creating Key Vault in Azure - https://docs.microsoft.com/en-us/azure/key-vault/general/developers-guide
		///		Key Vault Best Practices - https://docs.microsoft.com/en-us/azure/key-vault/general/best-practices
		///		Key Vault Access Management - https://docs.microsoft.com/en-us/azure/key-vault/general/overview-security#identity-and-access-management
		///		Assign a Key Vault Access Policy - https://docs.microsoft.com/en-us/azure/key-vault/general/assign-access-policy-cli
		/// </summary>
		/// <param name="debug">Check if debug for local settings vs. production</param>
		/// <param name="builder">The configuration builder object</param>
		/// <param name="initialConfig">The initial config that holds the app config objects</param>
		private static void AddAzureKeyVault(bool debug, ConfigurationBuilder builder, IConfigurationRoot initialConfig)
		{
			// You can call these variables anything you like, this is just what I defaulted them to
			// Please note that when you define a key in Azure Key Vault it should be done with the following structure
			// Example: AppConfig:Database:ConnectionString
			// If you structure it like so it will automatically map to the AppConfig.cs object.  For this example it would map to AppConfig.Database.ConnectionString.
			try
			{
				var akvUri = initialConfig["AzureKeyVaultUri"] ?? throw new ArgumentNullException("AzureKeyVaultUri");
				if (debug)
				{
					// ToDo: Put in example using dev credentials - ## Secrets Manager
					var appId = initialConfig["AppId"] ?? throw new ArgumentNullException("AppId");
					var appSecret = initialConfig["AppSecret"] ?? throw new ArgumentNullException("AppSecret");
					builder.AddAzureKeyVault(akvUri, appId, appSecret);
				}
				else
				{
					// This assumes you have setup a service principle to access the key vault on behalf of this app, see **Assign a Key Vault Access Policy** above
					var azureKeyVaultConfigOptions = new AzureKeyVaultConfigurationOptions()
					{
						Vault = akvUri,
						Client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback)),
						Manager = new DefaultKeyVaultSecretManager()
					};

					builder.AddAzureKeyVault(azureKeyVaultConfigOptions);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		/// <summary>
		/// Configure the Cache
		/// 
		/// Some Helpful Links:
		///		https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-3.1#caching-basics
		///		https://stackexchange.github.io/StackExchange.Redis/Configuration
		/// </summary>
		/// <param name="services"></param>
		/// <param name="appConfig"></param>
		private static void ConfigureCache(IServiceCollection services, AppConfig appConfig)
		{
			switch (appConfig.Cache.CacheType)
			{
				case ConfigEnums.CacheType.Memory:
				{
					// Here you can set your memory cache options, for now just default
					var memoryCacheOptions = new MemoryCacheOptions();

					// Add Memory Cache to our Services
					services.AddMemoryCache(o => {
						o = memoryCacheOptions;
					});

					break;
				}
				case ConfigEnums.CacheType.DistributedMemory:
				{
					// Here you can set your distributed memory cache options, for now just default
					var distributedMemoryCacheOptions = new MemoryDistributedCacheOptions();

					// Add Distributed Memory Cache to our Services
					services.AddDistributedMemoryCache(o => {
						o = distributedMemoryCacheOptions;
					});

					break;
				}
				case ConfigEnums.CacheType.RedisCache:
				{
					// Here you can set your redis cache options, for now just default
					var redisCacheOptions = new RedisCacheOptions();

					redisCacheOptions.ConfigurationOptions.ClientName = appConfig.Cache.RedisClientId;
					redisCacheOptions.ConfigurationOptions.Password = appConfig.Cache.RedisSecret;
					redisCacheOptions.ConfigurationOptions.ServiceName = appConfig.Cache.RedisServiceName;
					redisCacheOptions.ConfigurationOptions.EndPoints.Add(appConfig.Cache.RedisEndpoint);

					// Add Redis Cache to our Services
					services.AddDistributedRedisCache(o => {
						o = redisCacheOptions;
					});

					break;
				}
				default:
				{
					// Here you can set your memory cache options, for now just default
					var memoryCacheOptions = new MemoryCacheOptions();

					// Add Memory Cache to our Services
					services.AddMemoryCache(o => {
						o = memoryCacheOptions;
					});

					break;
				}
			}
		}

	}
}
