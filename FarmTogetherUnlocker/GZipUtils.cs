using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmTogetherUnlocker
{
    internal class GZipUtils
    {
        public static void DecompressGZip(string CompressedFileName, string OutputFileName)
        {
            using (FileStream compressedFileStream = File.Open(CompressedFileName, FileMode.Open))
            {
                using (FileStream outputFileStream = File.Create(OutputFileName))
                {
                    using (var decompressor = new GZipStream(compressedFileStream, CompressionMode.Decompress))
                    {
                        decompressor.CopyTo(outputFileStream);
                    }
                }
            }

        }

        public static void CompressGZip(string OriginalFileName, String CompressedFileName)
        {
            using (FileStream originalFileStream = File.Open(OriginalFileName, FileMode.Open))
            {
                using (FileStream compressedFileStream = File.Create(CompressedFileName))
                {
                    using (var compressor = new GZipStream(compressedFileStream, CompressionMode.Compress))
                    {
                        originalFileStream.CopyTo(compressor);

                    }
                }
            }
        }
    }
}
