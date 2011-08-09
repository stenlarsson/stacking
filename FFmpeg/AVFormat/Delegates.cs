using System;
using System.Runtime.InteropServices;

namespace FakeXna.FFmpeg.AVFormat
{
	public static class Delegates
	{
		public delegate int ReadPacketCallback(IntPtr opaque, IntPtr buf, int buf_size);
		public delegate int WritePacketCallback(IntPtr opaque, IntPtr buf, int buf_size);
		public delegate long SeekCallback(IntPtr opaque, long offset, int whence);
	}
}
