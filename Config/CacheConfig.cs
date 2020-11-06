using static Tallan.QuickStart.Config.Config.ConfigEnums;

namespace Tallan.QuickStart.Config.Config
{
	public class CacheConfig
	{
		#region Properties
		public CacheType CacheType { get; set; }
		public string RedisClientId { get; set; }
		public string RedisSecret { get; set; }
		public string RedisServiceName { get; set; }
		public string RedisEndpoint { get; set; }
		#endregion
	}
}
