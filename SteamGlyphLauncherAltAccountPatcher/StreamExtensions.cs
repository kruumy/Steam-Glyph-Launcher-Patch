using System;
using System.IO;
using System.Linq; // Still included for general C# development context, though not strictly used in this specific function.

namespace SteamGlyphLauncherAltAccountPatcher
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Searches for the first occurrence of a byte array within a FileStream,
        /// and sets the stream's position to the start of the pattern if found.
        /// </summary>
        /// <param name="stream">The FileStream to search within. The stream must be readable and seekable.</param>
        /// <param name="pattern">The byte array pattern to find</param>
        /// <returns>True if the pattern was found and the stream position was set; False otherwise.</returns>
        public static bool SeekToByteArray( FileStream stream, byte[] pattern )
        {
            if ( stream == null || pattern == null || pattern.Length == 0 )
            {
                return false;
            }

            int patternLength = pattern.Length;
            const int bufferSize = 4096;
            byte[] buffer = new byte[ bufferSize ];
            int bytesRead;

            long initialPosition = stream.Position;

            stream.Seek(0, SeekOrigin.Begin);

            int overlap = Math.Min(patternLength - 1, bufferSize - patternLength);
            if ( overlap < 0 )
            {
                overlap = 0;
            }

            long streamPosition = 0;
            while ( (bytesRead = stream.Read(buffer, 0, bufferSize)) > 0 )
            {
                int searchLength = bytesRead;

                if ( streamPosition > 0 )
                {
                    searchLength += overlap;
                }

                for ( int i = 0; i < searchLength - patternLength ; i++ )
                {
                    bool found = true;
                    for ( int j = 0; j < patternLength; j++ )
                    {
                        if ( buffer[ i + j ] != pattern[ j ] )
                        {
                            found = false;
                            break;
                        }
                    }

                    if ( found )
                    {
                        long resultPosition = streamPosition + i;
                        stream.Seek(resultPosition, SeekOrigin.Begin);
                        return true;
                    }
                }
                streamPosition += bytesRead - overlap;
                if ( bytesRead >= bufferSize && overlap > 0 )
                {
                    Array.Copy(buffer, bufferSize - overlap, buffer, 0, overlap);
                    stream.Seek(-overlap, SeekOrigin.Current);
                }
                else if ( bytesRead < bufferSize )
                {
                    break;
                }
            }
            stream.Seek(initialPosition, SeekOrigin.Begin);
            return false;
        }
    }
}
