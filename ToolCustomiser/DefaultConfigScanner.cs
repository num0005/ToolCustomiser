﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ToolCustomiser
{
    static class DefaultConfigScanner
	{
		private static readonly byte[] _pattern = {
			0xA1, 0xA0, 0x20, 0x3D, 0x02,
			0x00, 0x00, 0x00, 0x00, 0x00,
			0x80, 0x3F, 0x00, 0x00, 0x00,
			0x3F, 0x00, 0x00, 0x00, 0x40,
			0x00, 0x00, 0x80, 0x40, 0x00,
			0x00, 0x00, 0x40, 0x00, 0x00,
			0x80, 0x3F, 0x00, 0x00, 0x80,
			0x40, 0x00, 0x00, 0x00, 0x41,
			0x00, 0x00, 0x40, 0x40, 0x00,
			0x00, 0x00, 0x40, 0x00, 0x00,
			0x00, 0x41, 0x00, 0x00, 0x80,
			0x41, 0xFF, 0xFF, 0x7F, 0x7F,
			0x00, 0x00, 0xA0, 0x41, 0x00,
			0x00, 0x20, 0x42, 0x00, 0x00,
			0xA0, 0x42, 0x81, 0x80, 0x80,
			0x3B, 0x04, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x3F, 0x00,
			0x00, 0x00, 0x3E, 0x00, 0x00,
			0x00, 0x3F, 0x66, 0x66, 0x66,
			0x3F, 0x33, 0x33, 0x33, 0x3F,
			0x9A, 0x99, 0x99, 0x3E, 0x9A,
			0x99, 0x99, 0x3F, 0x9A, 0x99,
			0x19, 0x40, 0xCD, 0xCC, 0x4C,
			0x3F, 0x00, 0x00, 0x00, 0x3F,
			0x00, 0x00, 0x00, 0x40, 0x00,
			0x00, 0x80, 0x40, 0xFF, 0xFF,
			0x7F, 0x7F, 0x00, 0x00, 0xA0,
			0x41, 0x00, 0x00, 0x20, 0x42,
			0x00, 0x00, 0xA0, 0x42,
		};

		public static long? Find(Stream stream)
        {
			return StreamSearch.Find(stream, _pattern);
		}
    }
}
