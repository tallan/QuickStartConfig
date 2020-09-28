namespace Tallan.QuickStart.Config.Config
{
	/// <summary>
	/// This class is used to house all variables that pertain to an Azure Connection.  If you are not connecting into Azure
	/// then this class may not be necessary.
	/// </summary>
	public class AzureConfig
	{
		#region Properties
		/// <summary>
		/// This ClientID is meant to be used to retrieve Azure Credentials
		/// </summary>
		public string ClientId { get; set; }
		/// <summary>
		/// This Secret is meant to be used to retrieve Azure Credentials
		/// </summary>
		public string Secret { get; set; }
		/// <summary>
		/// This TenantId is meant to signify the Azure Tenant we are connecting into
		/// </summary>
		public string TenantId { get; set; }
		#endregion
	}
}
