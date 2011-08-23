using System;
using System.Runtime.InteropServices;
using FakeXna.FFmpeg.AVFormat;

namespace FakeXna.FFmpeg.AVCodec
{
	public class Codec
	{
		internal IntPtr native;

		internal Codec(FormatContext owner, IntPtr native)
		{
			this.native = native;
		}

		public static uint Version
		{
			get { return NativeMethods.avcodec_version(); }
		}

		public static void Init()
		{
			if (NativeMethods.avcodec_version() < 0x340000)
				throw new NotSupportedException();

			NativeMethods.avcodec_register_all();
		}
	}
}
