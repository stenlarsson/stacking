using System;
using System.Runtime.InteropServices;

namespace FakeXna.FFmpeg.AVCodec
{
	internal static class NativeMethods
	{
		public const string Library = "libavcodec.dll";
		public const CallingConvention CallConv = CallingConvention.Cdecl;

		[DllImport(Library, CallingConvention = CallConv)]
		public static extern uint avcodec_version();

		[DllImport(Library, CallingConvention = CallConv)]
		public static extern void avcodec_register_all();

		[DllImport(Library, CallingConvention = CallConv)]
		public static extern void av_init_packet(IntPtr avpkt);

		[DllImport(Library, CallingConvention = CallConv)]
		public static extern void av_free_packet(IntPtr avpkt);

		[DllImport(Library, CallingConvention = CallConv)]
		public static extern int avcodec_open(IntPtr avctx, IntPtr codec);

		[DllImport(Library, CallingConvention = CallConv)]
		public static extern int avcodec_decode_audio3(
			IntPtr avctx,
			IntPtr samples,
			ref int frame_size_ptr,
			IntPtr avpkt);
	}
}
