using System;
using System.Runtime.InteropServices;

namespace FakeXna.FFmpeg.AVFormat
{
	internal static class NativeMethods
	{
		// TODO: Change these to the proper dll names...
		// TODO: Can we support multiple dll names easily?
		// TODO: Can we detect which procedures are available easily?
		public const string Library = "libavformat.dll";
		public const CallingConvention CallConv = CallingConvention.Cdecl;

		[DllImport(Library, CallingConvention = CallConv)]
		public static extern uint avformat_version();

		[DllImport(Library, CallingConvention = CallConv)]
		public static extern void av_register_all();

		[DllImport(Library, CallingConvention = CallConv)]
		public static extern IntPtr avio_alloc_context(
			IntPtr buffer,
			int buffer_size,
			int write_flag,
			IntPtr opaque,
			Delegates.ReadPacketCallback read_packet,
			Delegates.WritePacketCallback write_packet,
			Delegates.SeekCallback seek);

		[DllImport(Library, CallingConvention = CallConv)]
		public static extern IntPtr avformat_alloc_context();

		[DllImport(Library, CallingConvention = CallConv)]
		public static extern int avformat_open_input(
			ref IntPtr formatContext,
			[MarshalAs(UnmanagedType.LPStr)] string filename,
			IntPtr fmt,
			IntPtr options);

		[DllImport(Library, CallingConvention = CallConv)]
		public static extern int av_find_stream_info(IntPtr formatContext);

		[DllImport(Library, CallingConvention = CallConv)]
		public static extern int av_find_best_stream(
			IntPtr formatContext,
			MediaType type,
			int wanted_stream_nb,
			int related_stream,
			out IntPtr decoder_ret,
			int flags);

		[DllImport(Library, CallingConvention = CallConv)]
		public static extern int av_read_frame(IntPtr formatContext, IntPtr avpkt);

		[DllImport(Library, CallingConvention = CallConv)]
		public static extern void avformat_free_context(IntPtr formatContext);
	}
}
