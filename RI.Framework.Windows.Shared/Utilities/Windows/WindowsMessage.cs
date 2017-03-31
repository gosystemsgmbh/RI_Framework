using System;
using System.Drawing;
using System.Runtime.InteropServices;




namespace RI.Framework.Utilities.Windows
{
	//TODO: Win32Exceptions based on PInvoke results
	//TODO: Attribute parameters for PInvoke functions
	//TODO: Handle Quit message
	//TODO: SendMessageTimeout
	//TODO: SendMessageCallback
	//TODO: BroadcastSystemMessage
	public struct WindowsMessage
	{
		public static WindowsMessage GetMessage ()
		{
			return WindowsMessage.GetMessage(IntPtr.Zero);
		}

		public static WindowsMessage GetMessage (IntPtr window)
		{
			return WindowsMessage.GetMessage(window, 0, 0);
		}

		public static WindowsMessage GetMessage (IntPtr window, uint filterMin, uint filterMax)
		{
			MSG nativeMessage;
			WindowsMessage.GetMessage(out nativeMessage, window, filterMin, filterMax);
			return new WindowsMessage(nativeMessage);
		}

		public static WindowsMessage? PeekMessage (bool remove, bool noYield)
		{
			return WindowsMessage.PeekMessage(remove, noYield, IntPtr.Zero);
		}

		public static WindowsMessage? PeekMessage (bool remove, bool noYield, IntPtr window)
		{
			return WindowsMessage.PeekMessage(remove, noYield, window, 0, 0);
		}

		public static WindowsMessage? PeekMessage (bool remove, bool noYield, IntPtr window, uint filterMin, uint filterMax)
		{
			uint flags = (remove ? WindowsMessage.PM_REMOVE : WindowsMessage.PM_NOREMOVE) & (noYield ? WindowsMessage.PM_NOYIELD : 0);
			MSG nativeMessage;
			if (!WindowsMessage.PeekMessage(out nativeMessage, window, filterMin, filterMax, flags))
			{
				return null;
			}
			return new WindowsMessage(nativeMessage);
		}

		internal static uint GetCurrentThreadId ()
		{
			return WindowsMessage.GetCurrentThreadIdNative();
		}

		private const uint PM_NOREMOVE = 0x0000;
		private const uint PM_REMOVE = 0x0001;
		private const uint PM_NOYIELD = 0x0002;

		private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xFFFF);

		internal const uint WM_QUIT = 0x0012;

		[DllImport("user32.dll")]
		private static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

		[DllImport("user32.dll")]
		private static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

		[DllImport("user32.dll")]
		private static extern bool TranslateMessage(ref MSG lpMsg);

		[DllImport("user32.dll")]
		private static extern IntPtr DispatchMessage(ref MSG lpMsg);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern bool PostMessage(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool PostThreadMessage(uint threadId, uint msg, UIntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", EntryPoint = "GetCurrentThreadId")]
		private static extern uint GetCurrentThreadIdNative();

		[DllImport("user32.dll")]
		private static extern void PostQuitMessage(int nExitCode);

		[StructLayout(LayoutKind.Sequential)]
		private struct MSG
		{
			public IntPtr hWnd;
			public uint message;
			public UIntPtr wParam;
			public IntPtr lParam;
			public uint time;
			public POINT pt;
		}

		[StructLayout (LayoutKind.Sequential)]
		private struct POINT
		{
			public int X;
			public int Y;
		}

		private MSG NativeMessage { get; set; }

		private WindowsMessage (MSG nativeMessage)
		{
			this.NativeMessage = nativeMessage;
		}

		public uint Message => this.NativeMessage.message;

		public UIntPtr WParam => this.NativeMessage.wParam;

		public IntPtr LParam => this.NativeMessage.lParam;

		public IntPtr Window => this.NativeMessage.hWnd;

		public Point Point => new Point(this.NativeMessage.pt.X, this.NativeMessage.pt.Y);

		public DateTime Time => new DateTime(this.NativeMessage.time);

		public WindowsMessage(uint message)
		{
			MSG nativeMessage = new MSG();
			nativeMessage.message = message;
			nativeMessage.wParam = UIntPtr.Zero;
			nativeMessage.lParam = IntPtr.Zero;
			nativeMessage.hWnd = IntPtr.Zero;
			this.NativeMessage = nativeMessage;
		}

		public WindowsMessage (uint message, UIntPtr wParam, IntPtr lParam)
		{
			MSG nativeMessage = new MSG();
			nativeMessage.message = message;
			nativeMessage.wParam = wParam;
			nativeMessage.lParam = lParam;
			nativeMessage.hWnd = IntPtr.Zero;
			this.NativeMessage = nativeMessage;
		}

		public WindowsMessage(uint message, UIntPtr wParam, IntPtr lParam, IntPtr window)
		{
			MSG nativeMessage = new MSG();
			nativeMessage.message = message;
			nativeMessage.wParam = wParam;
			nativeMessage.lParam = lParam;
			nativeMessage.hWnd = window;
			this.NativeMessage = nativeMessage;
		}

		public WindowsMessage Translate ()
		{
			MSG message = this.NativeMessage;
			WindowsMessage.TranslateMessage(ref message);
			return new WindowsMessage(message);
		}

		public IntPtr Dispatch ()
		{
			MSG message = this.NativeMessage;
			IntPtr result = WindowsMessage.DispatchMessage(ref message);
			return result;
		}

		public void Broadcast ()
		{
			this.Post(WindowsMessage.HWND_BROADCAST);
		}

		public void PostQuit ()
		{
			this.PostQuit(Environment.ExitCode);
		}

		public void PostQuit (int exitCode)
		{
			WindowsMessage.PostQuitMessage(exitCode);
		}

		public void Post ()
		{
			this.Post(IntPtr.Zero);
		}

		public void Post (WindowsMessageLoop loop)
		{
			if (loop == null)
			{
				throw new ArgumentNullException(nameof(loop));
			}

			this.Post(loop.NativeThreadId);
		}

		public void Post (IntPtr window)
		{
			WindowsMessage.PostMessage(window, this.NativeMessage.message, this.NativeMessage.wParam, this.NativeMessage.lParam);
		}

		internal void Post (uint nativeThreadId)
		{
			WindowsMessage.PostThreadMessage(nativeThreadId, this.NativeMessage.message, this.NativeMessage.wParam, this.NativeMessage.lParam);
		}

		public IntPtr Send (IntPtr window)
		{
			IntPtr result = WindowsMessage.SendMessage(window, this.NativeMessage.message, this.NativeMessage.wParam, this.NativeMessage.lParam);
			return result;
		}
	}
}