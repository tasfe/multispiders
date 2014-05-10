using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;

namespace Crawler.Engine.Utils
{
	/// <summary>
	/// 增强型控制台操作类，在多线程环境下可保护当前输入不被输出打断。
	/// </summary>
	public static class ConsoleExt
	{
		private static readonly int PageSize = 80;
		private static readonly object SyncRoot = new object();
		private static int _InputTop;
		public static int InputTop
		{
			get { return _InputTop; }
			set
			{
				if (value > Console.BufferHeight - 2)
					throw new Exception();

				//System.Diagnostics.Debug.WriteLine(_InputTop.ToString() + " -> " + value.ToString());
				_InputTop = value;
			}
		}
		private static int InputLeft;
		private static bool InRead;
		private static Thread AsyncThread;

		private static bool _IsMono;
		private static bool IsMono
		{
			get
			{
				return _IsMono;
			}
			//set
			//{
			//    lock (SyncRoot)
			//    {
			//        _IsMono = value;
			//    }
			//}
		}


		private static readonly ConcurrentQueue<Action> Actions = new ConcurrentQueue<Action>();

		public static void StartAsync()
		{
			_IsMono = Type.GetType("Mono.Runtime") != null;
			AsyncThread = new Thread(Flush);
			AsyncThread.Name = "TheKing2013控制台线程";
			AsyncThread.Start();
		}

		public static void StopAsync()
		{
			AsyncThread = null;
		}

		private static void Flush()
		{
			Action action;
			while (AsyncThread != null)
			{
				while (Actions.TryDequeue(out action))
				{
					try
					{
						action.Invoke();
					}
					catch (Exception ex)
					{
						//Log.Record("[" + Thread.CurrentThread.Name + "] " + ex.Message + Environment.NewLine + ex.StackTrace, false, LogType.Error);
					}
				}

				Thread.Sleep(100);
			}
		}

		public static string ReadLine()
		{
			if (IsMono)
			{
				return Console.ReadLine();
			}
			if (!InRead)
			{
				lock (SyncRoot)
				{
					InputTop = Console.CursorTop;
					InputLeft = Console.CursorLeft;
					InRead = true;
				}
				string ret = Console.ReadLine();
				lock (SyncRoot)
				{
					InRead = false;
				}

				return ret;
			}
			else
				throw new Exception("控制台输入锁定状态。");
		}

		public static string ReadLine(string format, params object[] arg)
		{
			if (IsMono)
			{
				Console.Write(format, arg);
				return Console.ReadLine();
			}
			if (!InRead)
			{
				lock (SyncRoot)
				{
					if (Console.CursorTop >= Console.BufferHeight - 1)
					{
						MoveBufferArea(0, 1, Console.BufferWidth, Console.BufferHeight - 1, 0, 0);
						Console.CursorTop -= 1;
						Console.SetWindowPosition(0, Console.WindowTop - 1);
					}
					InputTop = Console.CursorTop;
					InputLeft = Console.CursorLeft;
					//if (InputLeft != 0)
					//    try
					//    {
					//        throw new Exception("");
					//    }
					//    catch (Exception ex)
					//    {
					//        Debug.WriteLine("");
					//    }
					InRead = true;

					Console.Write(format, arg);
				}
				string ret = Console.ReadLine();
				lock (SyncRoot)
				{
					InRead = false;
				}

				return ret;
			}
			else
				throw new Exception("控制台输入锁定状态。");
		}

		public static void WriteLine(string format, params object[] arg)
		{
			if (IsMono)
			{
				Console.WriteLine(format, arg);
				return;
			}
			if (AsyncThread == null)
				ExcuteWriteLine(format, arg);
			else
				Actions.Enqueue(() => ExcuteWriteLine(format, arg));
		}

		public static void ExcuteWriteLine(string format, params object[] arg)
		{
			lock (SyncRoot)
			{
				if (!InRead)
				{
					Console.WriteLine(format, arg);
				}
				else
				{
					int Left = Console.CursorLeft;
					int Top = Console.CursorTop;

					int lines_input = Console.CursorTop - InputTop + 1;
					int lines = (string.Format(format, arg).Length / Console.BufferWidth) + 1;
					int lines_remain = Console.BufferHeight - Console.CursorTop - 1;
					int lines_up = lines - lines_remain + 2 - lines_input;
					int lines_down;

					if (lines_up <= 0)
					{
						//下移输入行
						lines_down = lines;
						Console.MoveBufferArea(0, InputTop, Console.BufferWidth, lines_input, 0, InputTop + lines_down);
						Console.CursorTop = InputTop;
						Console.CursorLeft = 0;
						Console.WriteLine(format, arg);
						Console.CursorTop = Top + lines_down;
						Console.CursorLeft = Left;
						InputTop += lines_down;
					}
					else if (lines_up == lines)
					{
						//上移所有行
						int new_top = Console.WindowTop - PageSize;
						MoveBufferArea(0, lines_up + PageSize, Console.BufferWidth, Console.BufferHeight - lines_input - lines_up - lines_remain - PageSize, 0, 0);
						Console.CursorTop = Top - lines_input - lines + 1 - PageSize;
						Console.CursorLeft = 0;
						Console.Write(format, arg);
						Console.MoveBufferArea(0, InputTop, Console.BufferWidth, lines_input, 0, InputTop - PageSize);
						Console.CursorTop = Top - PageSize;
						Console.CursorLeft = Left;
						Console.SetWindowPosition(0, new_top);
						_InputTop -= PageSize;
					}
					else
					{
						//一次输出多行，同时产生上移和下移
						int new_top = Console.WindowTop - PageSize + lines_remain;
						MoveBufferArea(0, lines_up + PageSize, Console.BufferWidth, Console.BufferHeight - lines - lines_input - PageSize, 0, 0);
						MoveBufferArea(0, InputTop, Console.BufferWidth, lines_input, 0, InputTop + lines_remain - PageSize);

						Console.CursorTop = InputTop + lines_remain - lines - PageSize;
						Console.CursorLeft = 0;
						Console.Write(format, arg);
						Console.CursorTop = Top + lines_remain - PageSize;
						Console.CursorLeft = Left;
						Console.SetWindowPosition(0, new_top);
						InputTop += lines_remain - PageSize;
					}
				}
			}
		}

		public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
		{
			if (sourceHeight > PageSize)
			{
				int lines_tomove = sourceHeight;
				int lines_moved = 0;
				do
				{
					int l = lines_tomove < PageSize ? lines_tomove : PageSize;

					Console.MoveBufferArea(sourceLeft, sourceTop + lines_moved, Console.BufferWidth, l, targetLeft, lines_moved);
					lines_tomove -= l;
					lines_moved += l;
				}
				while (lines_tomove > 0);
			}
			else
				Console.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop);
		}
	}
}
