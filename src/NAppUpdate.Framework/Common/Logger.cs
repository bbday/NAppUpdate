﻿using System;
using System.Collections.Generic;

namespace NAppUpdate.Framework.Common
{
    public class Logger
    {
		[Serializable]
		public enum SeverityLevel
		{
			Debug,
			Warning,
			Error
		}

    	[Serializable]
    	public class LogItem
    	{
    		public DateTime Timestamp { get; set; }
    		public string Message { get; set; }
    		public Exception Exception { get; set; }
    		public SeverityLevel Severity { get; set; }

    		public override string ToString()
    		{
    			if (Exception == null)
    				return string.Format("{0,-25}\t{1}\t{2}",
    				                     Timestamp.ToShortDateString() + " " + Timestamp.ToString("HH:mm:ss.fff"),
    				                     Severity,
    				                     Message);

    			return string.Format("{0,-25}\t{1}\t{2}{3}{4}",
    			                     Timestamp.ToShortDateString() + " " + Timestamp.ToString("HH:mm:ss.fff"),
    			                     Severity,
    			                     Message, Environment.NewLine, Exception);
    		}
    	}

    	public List<LogItem> LogItems { get; private set; }

		public Logger()
		{
			LogItems = new List<LogItem>();
		}

		public Logger(List<LogItem> logItems)
		{
			LogItems = logItems;
		}

        public void Log(SeverityLevel severity, string message, params object[] args)
        {
            LogItems.Add(new LogItem
                         	{
								Message = string.Format(message, args),
								Severity = severity,
								Timestamp = DateTime.Now,
                         	});
        }

		public void Log(Exception exception)
		{
			Log(exception, string.Empty);
		}

    	public void Log(Exception exception, string message)
    	{
    		LogItems.Add(new LogItem
    		             	{
								Message = message,
    		             		Severity = SeverityLevel.Error,
								Timestamp = DateTime.Now,
								Exception = exception,
    		             	});
    	}
    }
}