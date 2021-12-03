using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QoiSharp.Qoi
{
    public class Qoi
    {
		private const int QOI_INDEX   = 0x00;
		private const int QOI_RUN_8   = 0x40;
		private const int QOI_RUN_16  = 0x60;
		private const int QOI_DIFF_8  = 0x80;
		private const int QOI_DIFF_16 = 0xc0;
		private const int QOI_DIFF_24 = 0xe0;
		private const int QOI_COLOR   = 0xf0;

		private const int QOI_MASK_2  = 0xc0;
		private const int QOI_MASK_3  = 0xe0;
		private const int QOI_MASK_4  = 0xf0;

		private const int QOI_PADDING = 4;
		private const int QOI_HEADER_SIZE = 14;

		private int QOI_COLOR_HASH(byte r, byte g, byte b, byte a)
		{
			return ((r ^ g ^ b ^ a) % 64) * 4;
		}

		public (byte[] pixels, int width, int height, byte channels) Decode(string filename)
		{
			BytesReader reader = new BytesReader(File.Open(filename, FileMode.Open));

			string magic	= reader.ReadCString(4);
			int width		= reader.ReadInt32BE();
			int	height		= reader.ReadInt32BE();
			byte channels	= reader.ReadUChar();
			byte colorspace	= reader.ReadUChar();

			byte[] index = new byte[64 * 4];

			byte r = 0;
			byte g = 0;
			byte b = 0;
			byte a = 255;

			int pixel_size = width * (height * channels);
			byte[] pixels  = new byte[pixel_size];

			int run = 0;
			int p	= QOI_HEADER_SIZE;

			int chunks_size = (int)reader.Length - QOI_PADDING;

			for (int px_pos = 0; px_pos < pixel_size; px_pos += channels)
			{
				if (run > 0)
				{
					run--;
				}
				else if (p < chunks_size)
				{
					int b1 = reader.ReadUChar();
					if ((b1 & QOI_MASK_2) == QOI_INDEX)
					{
						var index_pos_1 = (b1 ^ QOI_INDEX) * 4;

						r = index[index_pos_1];
						g = index[index_pos_1 + 1];
						b = index[index_pos_1 + 2];
						a = index[index_pos_1 + 3];
					}
					else if ((b1 & QOI_MASK_3) == QOI_RUN_8)
					{
						run = (b1 & 0x1f);
					}
					else if ((b1 & QOI_MASK_3) == QOI_RUN_16)
					{
						int b2 = reader.ReadUChar();
						run = (((b1 & 0x1f) << 8) | (b2)) + 32;
					}
					else if ((b1 & QOI_MASK_2) == QOI_DIFF_8)
					{
						r += (byte)(((b1 >> 4) & 0x03) - 2);
						g += (byte)(((b1 >> 2) & 0x03) - 2);
						b += (byte)((b1 & 0x03) - 2);
					}
					else if ((b1 & QOI_MASK_3) == QOI_DIFF_16)
					{
						int b2 = reader.ReadUChar();
						r += (byte)((b1 & 0x1f) - 16);
						g += (byte)((b2 >> 4) - 8);
						b += (byte)((b2 & 0x0f) - 8);
					}
					else if ((b1 & QOI_MASK_4) == QOI_DIFF_24)
					{
						int b2 = reader.ReadUChar();
						int b3 = reader.ReadUChar();
						r += (byte)((((b1 & 0x0f) << 1) | (b2 >> 7)) - 16);
						g += (byte)(((b2 & 0x7c) >> 2) - 16);
						b += (byte)((((b2 & 0x03) << 3) | ((b3 & 0xe0) >> 5)) - 16);
						a += (byte)((b3 & 0x1f) - 16);
					}
					else if ((b1 & QOI_MASK_4) == QOI_COLOR)
					{
						if ((b1 & 8) != 0) { r = reader.ReadUChar(); }
						if ((b1 & 4) != 0) { g = reader.ReadUChar(); }
						if ((b1 & 2) != 0) { b = reader.ReadUChar(); }
						if ((b1 & 1) != 0) { a = reader.ReadUChar(); }
					}
					int index_pos_2 = QOI_COLOR_HASH(r, g, b, a);

					index[index_pos_2] = r;
					index[index_pos_2 + 1] = g;
					index[index_pos_2 + 2] = b;
					index[index_pos_2 + 3] = a;
				}

				pixels[px_pos    ] = r;
				pixels[px_pos + 1] = g;
				pixels[px_pos + 2] = b;

				if (channels == 4)
				{
					pixels[px_pos + 3] = a;
				}
			}
			return (pixels, width, height, channels);
		}
	}
}
