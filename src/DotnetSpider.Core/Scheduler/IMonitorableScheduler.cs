namespace DotnetSpider.Core.Scheduler
{
	/// <summary>
	/// The scheduler whose requests can be counted for monitor.
	/// </summary>
	public interface IMonitorable
	{
		long GetLeftRequestsCount();

		long GetTotalRequestsCount();

		long GetSuccessRequestsCount();

		long GetErrorRequestsCount();

		void IncreaseSuccessCounter();

		void IncreaseErrorCounter();
	}
}