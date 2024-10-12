using System;
using System.Collections.Generic;
using System.IO;

namespace ImageUtility
{
    public struct Dimensions
    {
        public int Width;
        public int Height;

        public static Dimensions ErrorValue
        {
            get
            {
                Dimensions returnValue;
                returnValue.Width  = 0;
                returnValue.Height = 0;
                return returnValue;
            }
        }
    }

    /// <summary>
    /// <para>Retrieves width and height of an image, without loading the entire image in memory.</para>
    ///
    /// <para>Based on https://stackoverflow.com/a/112711/1377948
    /// (with edits from https://stackoverflow.com/a/60667939/1377948
    /// for recognizing progressive jpeg and webp).</para>
    ///
    /// <para>Code will return (-1, -1) as an error value, instead of any exception throwing.</para>
    /// </summary>
    public static class Dimension
    {
        private static readonly Dictionary<byte[], Func<BinaryReader, Dimensions>> ImageFormatDecoders = new()
        {
            { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, DecodePng },
            { new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }, DecodeGif },
            { new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }, DecodeGif },
            { new byte[] { 0x52, 0x49, 0x46, 0x46 }, DecodeWebP },
            { new byte[] { 0xFF, 0xD8 }, DecodeJfif },
            { new byte[] { 0x42, 0x4D }, DecodeBitmap },
        };

        /// <summary>
        /// Currently the longest we have in <see cref="ImageFormatDecoders"/> is the png one, with 8 bytes.
        /// </summary>
        private const int LONGEST_MAGIC_BYTES = 8;

        private static byte[] _magicBytes;

        /// <summary>
        /// <para>Retrieves width and height of an image, without loading the entire image in memory.</para>
        /// </summary>
        /// <param name="path">Full path to the image to get the dimensions of.</param>
        /// <returns>(width, height) tuple, or (-1, -1) if not found.</returns>
        public static Dimensions Get(string path)
        {
            using (var binaryReader = new BinaryReader(File.OpenRead(path))) return Get(binaryReader);
        }

        /// <summary>
        /// <para>Retrieves width and height of an image opened in a BinaryReader.
        /// It will only read through the BinaryReader as least as it possibly
        /// can to get to the width and height.</para>
        /// </summary>
        /// <param name="binaryReader">A BinaryReader with the image file opened in it.</param>
        /// <returns>(width, height) tuple, or (-1, -1) if not found.</returns>
        public static Dimensions Get(BinaryReader binaryReader)
        {
            if (_magicBytes == null)
                _magicBytes = new byte[LONGEST_MAGIC_BYTES];
            else
                // reset the byte array checker to 0 value
                for (var i = 0; i < LONGEST_MAGIC_BYTES; i += 1)
                    _magicBytes[i] = 0;

            // Detect the image type not using the file type,
            // but via the bytes it has at the start of the file.
            //
            // We read up to n number of bytes at the start of the file (where n is LONGEST_MAGIC_BYTES),
            // and each time we've appended to the _magicBytes we're reading,
            // check if what we have so far matches any of the image types we know.
            for (var i = 0; i < LONGEST_MAGIC_BYTES; i += 1)
            {
                _magicBytes[i] = binaryReader.ReadByte();

                foreach (var kvPair in ImageFormatDecoders)
                {
                    if (_magicBytes.StartsWith(kvPair.Key))
                        // The bytes have been recognized as one of our known image types,
                        // now we use the Decode method assigned for that image type.
                        return kvPair.Value(binaryReader);
                }
            }

            return Dimensions.ErrorValue;
        }

        // =================================================================================

        private static bool StartsWith(this byte[] thisBytes, byte[] thatBytes)
        {
            for (var i = 0; i < thatBytes.Length; i += 1)
                if (thisBytes[i] != thatBytes[i])
                    return false;

            return true;
        }

        private const           int    SIZE_OF_SHORT = sizeof(short);
        private static readonly byte[] ShortBytes    = new byte[SIZE_OF_SHORT];

        private static short ReadLittleEndianInt16(this BinaryReader binaryReader)
        {
            for (var i = 0; i < SIZE_OF_SHORT; i += 1) ShortBytes[SIZE_OF_SHORT - 1 - i] = binaryReader.ReadByte();

            return BitConverter.ToInt16(ShortBytes, 0);
        }

        private const           int    SIZE_OF_INT = sizeof(int);
        private static readonly byte[] IntBytes    = new byte[SIZE_OF_INT];

        private static int ReadLittleEndianInt32(this BinaryReader binaryReader)
        {
            for (var i = 0; i < SIZE_OF_INT; i += 1) IntBytes[SIZE_OF_INT - 1 - i] = binaryReader.ReadByte();

            return BitConverter.ToInt32(IntBytes, 0);
        }

        // =================================================================================

        private static Dimensions DecodeBitmap(BinaryReader binaryReader)
        {
            binaryReader.ReadBytes(16);
            Dimensions returnValue;
            returnValue.Width  = binaryReader.ReadInt32();
            returnValue.Height = binaryReader.ReadInt32();
            return returnValue;
        }

        private static Dimensions DecodeGif(BinaryReader binaryReader)
        {
            Dimensions returnValue;
            returnValue.Width  = binaryReader.ReadInt16();
            returnValue.Height = binaryReader.ReadInt16();
            return returnValue;
        }

        private static Dimensions DecodePng(BinaryReader binaryReader)
        {
            binaryReader.ReadBytes(8);
            Dimensions returnValue;
            returnValue.Width  = binaryReader.ReadLittleEndianInt32();
            returnValue.Height = binaryReader.ReadLittleEndianInt32();
            return returnValue;
        }

        private static Dimensions DecodeJfif(BinaryReader binaryReader)
        {
            while (binaryReader.ReadByte() == 0xFF) // skipp FF
            {
                var marker      = binaryReader.ReadByte();
                var chunkLength = binaryReader.ReadLittleEndianInt16();

                // C2: progressive (from https://stackoverflow.com/a/60667939/1377948)
                if (marker == 0xC0 || marker == 0xC2)
                {
                    binaryReader.ReadByte();

                    Dimensions returnValue;
                    returnValue.Height = binaryReader.ReadLittleEndianInt16();
                    returnValue.Width  = binaryReader.ReadLittleEndianInt16();
                    return returnValue;
                }

                if (chunkLength < 0)
                {
                    var uChunkLength = (ushort)chunkLength;
                    binaryReader.ReadBytes(uChunkLength - 2);
                }
                else
                {
                    binaryReader.ReadBytes(chunkLength - 2);
                }
            }

            return Dimensions.ErrorValue;
        }

        // (from https://stackoverflow.com/a/60667939/1377948)
        private static Dimensions DecodeWebP(BinaryReader binaryReader)
        {
            binaryReader.ReadUInt32();  // Size
            binaryReader.ReadBytes(15); // WEBP, VP8 + more
            binaryReader.ReadBytes(3);  // SYNC

            Dimensions returnValue;

            // 14 bits (ignore last 2 bits of the 16 bit value) for width
            returnValue.Width = binaryReader.ReadUInt16() & 0x3FFF;

            // 14 bits (ignore last 2 bits of the 16 bit value) for height
            returnValue.Height = binaryReader.ReadUInt16() & 0x3FFF;

            return returnValue;
        }

        // =================================================================================
    }
}