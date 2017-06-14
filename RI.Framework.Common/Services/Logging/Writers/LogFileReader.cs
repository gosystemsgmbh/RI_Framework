using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using RI.Framework.Utilities.Exceptions;

namespace RI.Framework.Services.Logging.Writers
{
	internal sealed class LogFileReader
	{
		internal static int MoveToNextEntryInStream (Stream stream, int minMove)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			if (!stream.CanRead)
			{
				throw new NotReadableStreamArgumentException(nameof(stream));
			}

			if (!stream.CanSeek)
			{
				throw new NotSeekableStreamArgumentException(nameof(stream));
			}

			if (!stream.CanWrite)
			{
				throw new NotWriteableStreamArgumentException(nameof(stream));
			}

			if (minMove < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(minMove));
			}

			int read = 0;
			int stage = 0;
			while (true)
			{
				int current = stream.ReadByte();
				if (current == -1)
				{
					if (read >= minMove)
					{
						return read;
					}
					else
					{
						stream.SetLength(stream.Length + (minMove - read));
						return minMove;
					}
				}

				read++;

				switch (stage)
				{
					case 0:
						if ((char)current == '\n')
						{
							stage = 1;
						}
						break;

					case 1:
						if ((char)current == '#')
						{
							if (read >= minMove)
							{
								stream.Seek(-1, SeekOrigin.Current);
								return read - 1;
							}
							else
							{
								stage = 0;
							}
						}
						else
						{
							stage = 0;
						}
						break;
				}
			}
		}
	}
}
