namespace BrimSchedule.API.Utility
{
	public class ServiceResult
	{
		public bool IsSuccess { get; set; }
		public object Content { get; set; }
		public string ErrorMessage { get; set; }
	}
}
