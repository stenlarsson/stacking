using System;
using System.Runtime.InteropServices;

namespace FakeXna.FFmpeg.AVUtil
{
	internal static class NativeMethods
	{
		public const string Library = "libavutil.dll";
		public const CallingConvention CallConv = CallingConvention.Cdecl;

		[DllImport(Library, CallingConvention = CallConv)]
		public static extern void av_free(IntPtr ptr);

		[DllImport(Library, CallingConvention = CallConv)]
		public static extern IntPtr av_mallocz(int size);
	}
}
