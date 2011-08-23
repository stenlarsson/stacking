using System;
using System.Runtime.InteropServices;
using FakeXna.FFmpeg.AVUtil;
using FakeXna.FFmpeg.AVFormat;

namespace FakeXna.FFmpeg.AVCodec
{
	public class CodecContext
	{
		FormatContext owner;
		IntPtr native;

		internal CodecContext(FormatContext owner, IntPtr native)
		{
			this.owner = owner;
			this.native = native;
		}

		public void Open(Codec codec)
		{
			// TODO: Implement lookup of codec id...
			if (codec == null)
				throw new ArgumentException();

			if (owner.Disposed)
				throw new InvalidOperationException();

			int r = NativeMethods.avcodec_open(native, codec.native);
			if (r != 0)
				throw new InvalidOperationException();
		}

		// Compute the rather complicated offset for sample_rate and whatnot.
		int offset
		{
			get {
				if (owner.Disposed)
					throw new InvalidOperationException();

				// Offset: 1 ptr, 5 int, 1 ptr, 7 int, maybe 1 int, 1 ptr
				// NOTE: This will break for 128-bit processors, but perhaps we don't need to be that future proof. :)
				if (Memory.IntPtrSize == 8)
					return 3*8 /*ptrs*/ + 12*4 /*ints*/ + 1*4 /*pad*/ + 1*4 /*pad/extra*/;
				else if (Codec.Version < 53)
					return 3*4 /*ptrs*/ + 12*4 /*ints*/ + 1*4 /*extra*/;
				else
					return 3*4 /*ptrs*/ + 12*4 /*ints*/;
			}
		}
		// Followed by: int sample_rate, int channels, and enum sample_fmt
		public int SampleRate { get { return Marshal.ReadInt32(native, offset); } }
		public int Channels { get { return Marshal.ReadInt32(native, offset + sizeof(int)); } }
		public SampleFormat SampleFormat { get { return (SampleFormat)Marshal.ReadInt32(native, offset + 2*sizeof(int)); } }

		public const int MinimumOutputBufferSize = 192015; // 15 Extra for alignment

		public int DecodeAudio(byte[] outputBuffer, Packet inputPacket)
		{
			if (owner.Disposed)
				throw new InvalidOperationException();

			int outputSize = outputBuffer.Length;
			if (outputSize < MinimumOutputBufferSize)
				throw new ArgumentException();
			
			GCHandle handle = GCHandle.Alloc(outputBuffer, GCHandleType.Pinned);
			try {
				int consumed =
					NativeMethods.avcodec_decode_audio3(
						native, handle.AddrOfPinnedObject(), ref outputSize, inputPacket.native);
				if (consumed < 0)
					throw new ArgumentException();
				if (consumed != inputPacket.Size)
					throw new NotImplementedException();
				return outputSize;
			} finally {
				handle.Free();
			}
		}
	}
}
