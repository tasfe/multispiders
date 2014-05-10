using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;

namespace Crawler.Engine.Utils
{
	public enum LogType
	{
		Console,
		Database,
		Error
	}

	public static class Log
	{
		private static object SyncRootConsole = new object();
		private static object SyncRootDataBase = new object();
		private static object SyncRootError = new object();

	    static Log()
	    {
	        if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Logs"))
	        {
	            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Logs");
	        }
	    }

		public static void Record(bool display = true, LogType type = LogType.Console)
		{
			switch (type)
			{
				case LogType.Console:
					lock (SyncRootConsole)
						File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "Logs" + Path.DirectorySeparatorChar
							+ "console" + DateTime.Now.ToString("yyyyMMdd") + ".log",
							string.Format("[{0}]" + Environment.NewLine, DateTime.Now.ToString()));
					break;
				case LogType.Database:
					lock (SyncRootConsole)
						File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "Logs" + Path.DirectorySeparatorChar
							+ "database" + DateTime.Now.ToString("yyyyMMdd") + ".log",
							string.Format("[{0}]" + Environment.NewLine, DateTime.Now.ToString()));
					break;
				case LogType.Error:
					lock (SyncRootConsole)
						File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "Logs" + Path.DirectorySeparatorChar
							+ "error" + DateTime.Now.ToString("yyyyMMdd") + ".log",
							string.Format("[{0}]" + Environment.NewLine, DateTime.Now.ToString()));
					break;
			}

			if (display)
			{
				ConsoleExt.WriteLine("");
			}
		}

		public static void Record(string value, bool display = true, LogType type = LogType.Console)
		{
			switch (type)
			{
				case LogType.Console:
					lock (SyncRootConsole)
						File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "Logs" + Path.DirectorySeparatorChar
							+ "console" + DateTime.Now.ToString("yyyyMMdd") + ".log",
							string.Format("[{0}] {1}" + Environment.NewLine, DateTime.Now.ToString(), value));
					break;
				case LogType.Database:
					lock (SyncRootDataBase)
						File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "Logs" + Path.DirectorySeparatorChar
							+ "database" + DateTime.Now.ToString("yyyyMMdd") + ".log",
							string.Format("[{0}] {1}" + Environment.NewLine, DateTime.Now.ToString(), value));
					break;
				case LogType.Error:
					lock (SyncRootError)
						File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "Logs" + Path.DirectorySeparatorChar
								+ "error" + DateTime.Now.ToString("yyyyMMdd") + ".log",
								string.Format("[{0}] {1}" + Environment.NewLine, DateTime.Now.ToString(), value));
					break;
			}

			if (display)
			{
				ConsoleExt.WriteLine("[{0}] {1}", DateTime.Now.ToString(), value);
			}
		}
        
		public static void Record(string format, params object[] arg)
		{
			//lock (SyncRoot)
			//{
			//    string s = string.Format("[{0}] ", DateTime.Now.ToString()) + string.Format(format, arg);
			//    int lines = (s.Length / Console.BufferWidth) + 1;
			//    int left = Console.CursorLeft;
			//    int top = Console.CursorTop;
			//    Console.MoveBufferArea(0, lines, Console.BufferWidth, top - lines, 0, 0);
			//    Console.CursorTop = top - lines;
			//    Console.CursorLeft = 0;
			//    Console.Write("[{0}] ", DateTime.Now.ToString());
			//    Console.WriteLine(format, arg);
			//    Console.CursorTop = top;
			//    Console.CursorLeft = left;
			//}
			lock (SyncRootConsole)
				File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "Logs" + Path.DirectorySeparatorChar
					+ "console" + DateTime.Now.ToString("yyyyMMdd") + ".log",
					string.Format("[{0}] {1}" + Environment.NewLine, DateTime.Now.ToString(), string.Format(format, arg)));
			ConsoleExt.WriteLine("[{0}] {1}", DateTime.Now.ToString(), string.Format(format, arg));
		}
	}
}
