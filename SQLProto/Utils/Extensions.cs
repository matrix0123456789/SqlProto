using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLProto.Utils
{
    public static class Extensions
    {
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> obj)
        {
            return obj ?? new T[0];
        }

        public static IEnumerable<string> AndAlso(this IEnumerable<string> obj, params string[] toAdd)
        {
            if (obj == null)
                return toAdd;
            else
                return obj.Concat(toAdd);
        }

        public static string ReadUtf8Line(this Stream stream)
        {
            var bytes = new List<byte>();
            while (true)
            {
                var readed = stream.ReadByte();
                if (readed == -1 || readed == '\n')
                {
                    break;
                }

                bytes.Add((byte) readed);
            }

            if (bytes.Any() && bytes.Last() == '\r')
            {
                return Encoding.UTF8.GetString(bytes.Take(bytes.Count - 1).ToArray());
            }
            else
            {
                return Encoding.UTF8.GetString(bytes.ToArray());
            }
        }

        public static async Task<string> ReadUtf8LineAsync(this Stream stream)
        {
            var bytes = new List<byte>();
            var readedByte = new byte[1];
            while (true)
            {
                var readed = await stream.ReadAsync(readedByte, 0, 1);
                if (readed == 0 || readedByte[0] == '\n')
                {
                    break;
                }

                bytes.Add(readedByte[0]);
            }

            if (bytes.LastOrDefault() == '\r')
            {
                return Encoding.UTF8.GetString(bytes.Take(bytes.Count - 1).ToArray());
            }
            else
            {
                return Encoding.UTF8.GetString(bytes.ToArray());
            }
        }
    }
}