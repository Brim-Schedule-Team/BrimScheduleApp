using System;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using log4net.Config;

namespace BrimSchedule.Application.Logging
{
	public class LoggingManager: ILoggingManager
	{
		private const string DefaultConfigFileName = "log4net.config";
		private readonly ILog _logger = LogManager.GetLogger(typeof(LoggingManager));

		public LoggingManager(): this(DefaultConfigFileName)
		{
		}

		public LoggingManager(string configFileName)
		{
			var log4NetConfig = new XmlDocument();

			using var fs = File.OpenRead(configFileName);
			log4NetConfig.Load(fs);

			var repo = LogManager.CreateRepository(
				Assembly.GetEntryAssembly(),
				typeof(log4net.Repository.Hierarchy.Hierarchy));

			XmlConfigurator.Configure(repo, log4NetConfig["log4net"]);
		}

		public void Debug(string msg)
		{
			_logger.Debug(msg);
		}

		public void Info(string msg)
		{
			_logger.Info(msg);
		}

		public void Warn(string msg)
		{
			_logger.Warn(msg);
		}

		public void Error(string msg, Exception ex)
		{
			_logger.Error(msg, ex);
		}
	}
}
