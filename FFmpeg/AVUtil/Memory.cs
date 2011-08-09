using System;
using System.Runtime.InteropServices;

namespace FakeXna.FFmpeg.AVUtil
{
	public static class Memory
	{
		public static int IntPtrSize = Marshal.SizeOf(typeof(IntPtr));

		public static IntPtr Free(IntPtr val)
		{
			NativeMethods.av_free(val);
			return IntPtr.Zero;
		}

		public static IntPtr Mallocz(int size)
		{
			IntPtr result = NativeMethods.av_mallocz(size);
			if (result == IntPtr.Zero)
				throw new OutOfMemoryException();
			return result;
		}
	}
}
