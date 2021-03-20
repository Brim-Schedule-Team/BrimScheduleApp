using System;

namespace BrimSchedule.Application.Logging
{
	public interface ILoggingManager
	{
		void Debug(string msg);
		void Info(string msg);
		void Warn(string msg);
		void Error(string msg, Exception ex);
	}
}
