namespace Tallan.QuickStart.Config.Config
{
	public class LoggingConfig
	{
		#region Properties
		public bool VerboseLogging { get; set; }
		public bool LogEntityFrameworkCalls { get; set; }
		public bool EnableSensitiveDataLogging { get; set; }
		#endregion
	}
}
