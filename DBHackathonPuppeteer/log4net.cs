using log4net;

namespace PuppeteerSharpPdfDemo
{
	static class Logger
	{
		static ILog logger = null;
		
		public static void SetConfiguration()
		{
			logger = LogManager.GetLogger(typeof(Logger));
			var l = (log4net.Repository.Hierarchy.Logger)logger.Logger;
			l.Level = log4net.Core.Level.Trace;
			AddConsoleAppender();
		}
		
		public static void Debug(string message)
		{
			logger.Debug(message);
		}
		
		public static void Info(string message)
		{
			logger.Info(message);
		}
		
		// Others here
		
		static private void AddConsoleAppender()
		{
			var appender = new log4net.Appender.AnsiColorTerminalAppender
			{
				Threshold = log4net.Core.Level.Info
			};
			var l = (log4net.Repository.Hierarchy.Logger)logger.Logger;
			log4net.Layout.PatternLayout layout = new log4net.Layout.PatternLayout
			{
				ConversionPattern = "%d [%t] %-5p %c [%x] - %m%n"
			};
			layout.ActivateOptions();
			appender.Name = "Console";
			appender.Layout = layout;
			appender.ActivateOptions();
			l.AddAppender(appender);
			l.Repository.Configured = true;
		}
	}
	
}