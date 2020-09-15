namespace Tallan.QuickStart.Config.Config
{
	public class AppConfig
	{
		#region Properties
		public AzureConfig Azure { get; set; }
		public DatabaseConfig Database { get; set; }
		public LoggingConfig Logging { get; set; }
		public CacheConfig Cache { get; set; }
		#endregion

		public AppConfig()
		{
			Azure = new AzureConfig();
			Database = new DatabaseConfig();
			Logging = new LoggingConfig();
			Cache = new CacheConfig();
		}
	}
}
