using System.Text;
using Org.BouncyCastle.Crypto;

namespace Boundless.Swapper.Swappers;

public static class Researcher
{
    public static void FindAndReplace(Stream stream, string search, string replace)
    {
        if (search.Length != replace.Length)
            throw new ArgumentException(
                "Search and replace strings must be the same length.");

        if (!stream.CanRead || !stream.CanWrite || !stream.CanSeek)
            throw new ArgumentException(
                "Stream must support reading, writing, and seeking.");

        if (search.Length == 0)
            throw new ArgumentException("Search string cannot be empty.");

        var searchBytes = Encoding.ASCII.GetBytes(search);
        var replaceBytes = Encoding.ASCII.GetBytes(replace);
        var buffer = new byte[searchBytes.Length];

        stream.Position = 0;

        while (stream.Position <= stream.Length - searchBytes.Length)
        {
            var bytesRead = 0;

            while (bytesRead < buffer.Length)
            {
                var read = stream.Read(buffer, bytesRead, buffer.Length - bytesRead);
                if (read == 0)
                {
                    throw new DataLengthException("Read 0 bytes");
                }

                bytesRead += read;
            }

            if (buffer.SequenceEqual(searchBytes))
            {
                stream.Position -= buffer.Length;
                stream.Write(replaceBytes);
                break;
            }

            stream.Position -= buffer.Length - 1;
        }
    }
}