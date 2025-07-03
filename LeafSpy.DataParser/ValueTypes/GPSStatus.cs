/**
 * MIT License
 * 
 * Copyright (c) 2025 Eric Hobbs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
namespace LeafSpy.DataParser.ValueTypes
{
    public class GPSStatus : BaseValue
    {
        public byte[] RawData { get; private set; }
        public GPSStatus(string raw) : base(raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                RawData = [0, 0];
            }
            else
            {
                RawData = HexStringToByteArray(raw);
            }
        }

        public GpsStatusFlags GetFlags()
        {
            return (GpsStatusFlags)RawData[^1];
        }

        public int GetAccuracy()
        {
            if (!GetFlags().HasFlag(GpsStatusFlags.AccuracyValid))
                return 0;

            if (RawData.Length >= 3)
                return RawData[1];
            if (RawData.Length == 2)
                return RawData[0];
            return 0;
        }

        private static byte[] HexStringToByteArray(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return [];

            if (hex.Length % 2 != 0)
                hex = "0" + hex; // Pad with leading zero if needed

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);

            return bytes;
        }

        public override string ToString()
        {
            return GetFlags().ToString();
        }
    }
}
