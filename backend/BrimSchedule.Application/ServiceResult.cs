namespace BrimSchedule.Application
{
	public class ServiceResult
	{
		public bool Success { get; init; }
		public string ErrorMessage { get; init; }

		private static ServiceResult StaticSuccessResult => new()
		{
			Success = true,
			ErrorMessage = string.Empty
		};

		public static ServiceResult SuccessResult() => StaticSuccessResult;

		public static ServiceResult<T> SuccessResult<T>(T content) => new ()
		{
			Success = true,
			ErrorMessage = string.Empty,
			Content = content
		};

		public static ServiceResult FailureResult(string errorMessage) => new()
		{
			Success = false,
			ErrorMessage = errorMessage
		};

		public static ServiceResult<T> FailureResult<T>(string errorMessage, T content = default) => new()
		{
			Success = false,
			ErrorMessage = errorMessage,
			Content = content
		};
	}

	public class ServiceResult<T> : ServiceResult
	{
		public T Content { get; init; }
	}
}
