using System;

namespace BrimSchedule.Application
{
	public class ServiceResult
	{
		public bool Success { get; protected init; }
		public string ErrorMessage { get; protected init; }

		public static ServiceResult SuccessResult => new()
		{
			Success = true,
			ErrorMessage = string.Empty
		};

		public static ServiceResult FailureResult(string errorMessage) => new()
		{
			Success = false,
			ErrorMessage = errorMessage
		};
	}

	public class ServiceResult<T> : ServiceResult
	{
		public T Content { get; private init; }

		public static ServiceResult<T> SuccessResult(T content) => new()
		{
			Success = true,
			ErrorMessage = string.Empty,
			Content = content
		};
	}
}
