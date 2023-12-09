using System.Text;

namespace Lab10;

class SHA1Algorithm
{
    public static void Demo()
    {
        string message = "What a beautiful world!";
        Console.WriteLine($"Хеш за алгоритмом SHA-1: {SHA1(message)}");
    }

    static string[] PrepareMessageForSha1(string message)
    {
        string bits =
            $"{BitString(message)}1{new string('0', (448 - message.Length * 8 - 1) % 512)}{LengthBitString(message)}";
        return Enumerable.Range(0, bits.Length / 512).Select(i => bits.Substring(i * 512, 512)).ToArray();
    }

    static string BitString(string message)
    {
        return string.Join("", Encoding.Default.GetBytes(message).Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
    }

    static string LengthBitString(string message)
    {
        long length = message.Length * 8;
        return BitConverter.GetBytes(length).Aggregate("", (s, b) => s + Convert.ToString(b, 2).PadLeft(8, '0'));
    }

    static uint LeftRotate(uint value, int shift)
    {
        return ((value << shift) | (value >> (32 - shift))) & 0xffffffff;
    }

    static uint[] MainLoop(uint a, uint b, uint c, uint d, uint e, uint[] w)
    {
        for (int i = 0; i < 80; i++)
        {
            uint f, k;

            if (i < 20)
            {
                f = (b & c) | (~b & d);
                k = 0x5A827999;
            }
            else if (i < 40)
            {
                f = b ^ c ^ d;
                k = 0x6ED9EBA1;
            }
            else if (i < 60)
            {
                f = (b & c) | (b & d) | (c & d);
                k = 0x8F1BBCDC;
            }
            else
            {
                f = b ^ c ^ d;
                k = 0xCA62C1D6;
            }

            uint temp = LeftRotate(a, 5) + f + e + k + w[i];
            e = d;
            d = c;
            c = LeftRotate(b, 30);
            b = a;
            a = temp & 0xffffffff;
        }

        return new[] { a, b, c, d, e };
    }

    static uint[] FinalAddition(uint h0, uint h1, uint h2, uint h3, uint h4, uint a, uint b, uint c, uint d, uint e)
    {
        return new[]
        {
            (h0 + a) & 0xffffffff,
            (h1 + b) & 0xffffffff,
            (h2 + c) & 0xffffffff,
            (h3 + d) & 0xffffffff,
            (h4 + e) & 0xffffffff
        };
    }

    private static string SHA1(string message)
    {
        uint h0 = 0x67452301;
        uint h1 = 0xEFCDAB89;
        uint h2 = 0x98BADCFE;
        uint h3 = 0x10325476;
        uint h4 = 0xC3D2E1F0;

        foreach (var block in PrepareMessageForSha1(message))
        {
            uint[] w = Enumerable.Range(0, 16).Select(i => Convert.ToUInt32(block.Substring(i * 32, 32), 2))
                .Concat(new uint[64]).ToArray();

            for (int i = 16; i < 80; i++)
            {
                w[i] = LeftRotate(w[i - 3] ^ w[i - 8] ^ w[i - 14] ^ w[i - 16], 1);
            }

            uint a = h0, b = h1, c = h2, d = h3, e = h4;
            var result = MainLoop(a, b, c, d, e, w);
            h0 = result[0];
            h1 = result[1];
            h2 = result[2];
            h3 = result[3];
            h4 = result[4];

            var finalResult = FinalAddition(h0, h1, h2, h3, h4, a, b, c, d, e);
            h0 = finalResult[0];
            h1 = finalResult[1];
            h2 = finalResult[2];
            h3 = finalResult[3];
            h4 = finalResult[4];
        }

        return $"{h0:x8}{h1:x8}{h2:x8}{h3:x8}{h4:x8}";
    }
}