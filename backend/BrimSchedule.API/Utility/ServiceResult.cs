namespace BrimSchedule.API.Utility
{
	public class ServiceResult<T>
	{
		public bool Success { get; set; }
		public T Content { get; set; }
		public string ErrorMessage { get; set; }
	}
}
