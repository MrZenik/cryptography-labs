using System.Text;

namespace Lab3;

static class DES
{
    private const int Encrypt = 1;
    private const int Decrypt = 0;

    private static uint Bitnum(byte[] a, int b, int c)
    {
        return (uint)(((a[b / 8] >> (7 - (b % 8))) & 0x01) << (c));
    }

    private static byte Bitnumintr(uint a, int b, int c)
    {
        return (byte)((((a) >> (31 - (b))) & 0x00000001) << (c));
    }

    private static uint Bitnumintl(uint a, int b, int c)
    {
        return ((((a) << (b)) & 0x80000000) >> (c));
    }

    private static uint Sboxbit(byte a)
    {
        return (uint)(((a) & 0x20) | (((a) & 0x1f) >> 1) | (((a) & 0x01) << 4));
    }

    private static readonly byte[] Sbox1 =
    {
        14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7,
        0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8,
        4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0,
        15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13
    };

    private static readonly byte[] Sbox2 =
    {
        15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10,
        3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5,
        0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15,
        13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9
    };

    private static readonly byte[] Sbox3 =
    {
        10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8,
        13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1,
        13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7,
        1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12
    };

    private static readonly byte[] Sbox4 =
    {
        7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15,
        13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9,
        10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4,
        3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14
    };

    private static readonly byte[] Sbox5 =
    {
        2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9,
        14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6,
        4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14,
        11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3
    };

    private static readonly byte[] Sbox6 =
    {
        12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11,
        10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8,
        9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6,
        4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13
    };

    private static readonly byte[] Sbox7 =
    {
        4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1,
        13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6,
        1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2,
        6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12
    };

    private static readonly byte[] Sbox8 =
    {
        13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7,
        1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2,
        7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8,
        2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11
    };

    public static void KeySchedule(byte[] key, byte[,] schedule, uint mode)
    {
        uint i, j, toGen, c, d;
        uint[] keyRndShift = { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };
        uint[] keyPermC =
        {
            56, 48, 40, 32, 24, 16, 8, 0, 57, 49, 41, 33, 25, 17,
            9, 1, 58, 50, 42, 34, 26, 18, 10, 2, 59, 51, 43, 35
        };
        uint[] keyPermD =
        {
            62, 54, 46, 38, 30, 22, 14, 6, 61, 53, 45, 37, 29, 21,
            13, 5, 60, 52, 44, 36, 28, 20, 12, 4, 27, 19, 11, 3
        };
        uint[] keyCompression =
        {
            13, 16, 10, 23, 0, 4, 2, 27, 14, 5, 20, 9,
            22, 18, 11, 3, 25, 7, 15, 6, 26, 19, 12, 1,
            40, 51, 30, 36, 46, 54, 29, 39, 50, 44, 32, 47,
            43, 48, 38, 55, 33, 52, 45, 41, 49, 35, 28, 31
        };

        for (i = 0, j = 31, c = 0; i < 28; ++i, --j)
            c |= Bitnum(key, (int)keyPermC[i], (int)j);

        for (i = 0, j = 31, d = 0; i < 28; ++i, --j)
            d |= Bitnum(key, (int)keyPermD[i], (int)j);

        for (i = 0; i < 16; ++i)
        {
            c = ((c << (int)keyRndShift[i]) | (c >> (28 - (int)keyRndShift[i]))) & 0xfffffff0;
            d = ((d << (int)keyRndShift[i]) | (d >> (28 - (int)keyRndShift[i]))) & 0xfffffff0;

            if (mode == Decrypt)
                toGen = 15 - i;
            else
                toGen = i;

            for (j = 0; j < 6; ++j)
                schedule[toGen, j] = 0;

            for (j = 0; j < 24; ++j)
                schedule[toGen, j / 8] |= Bitnumintr(c, (int)keyCompression[j], (int)(7 - (j % 8)));

            for (; j < 48; ++j)
                schedule[toGen, j / 8] |= Bitnumintr(d, (int)keyCompression[j] - 28, (int)(7 - (j % 8)));
        }
    }

    private static void IP(uint[] state, byte[] input)
    {
        state[0] = Bitnum(input, 57, 31) | Bitnum(input, 49, 30) | Bitnum(input, 41, 29) | Bitnum(input, 33, 28) |
                   Bitnum(input, 25, 27) | Bitnum(input, 17, 26) | Bitnum(input, 9, 25) | Bitnum(input, 1, 24) |
                   Bitnum(input, 59, 23) | Bitnum(input, 51, 22) | Bitnum(input, 43, 21) | Bitnum(input, 35, 20) |
                   Bitnum(input, 27, 19) | Bitnum(input, 19, 18) | Bitnum(input, 11, 17) | Bitnum(input, 3, 16) |
                   Bitnum(input, 61, 15) | Bitnum(input, 53, 14) | Bitnum(input, 45, 13) | Bitnum(input, 37, 12) |
                   Bitnum(input, 29, 11) | Bitnum(input, 21, 10) | Bitnum(input, 13, 9) | Bitnum(input, 5, 8) |
                   Bitnum(input, 63, 7) | Bitnum(input, 55, 6) | Bitnum(input, 47, 5) | Bitnum(input, 39, 4) |
                   Bitnum(input, 31, 3) | Bitnum(input, 23, 2) | Bitnum(input, 15, 1) | Bitnum(input, 7, 0);

        state[1] = Bitnum(input, 56, 31) | Bitnum(input, 48, 30) | Bitnum(input, 40, 29) | Bitnum(input, 32, 28) |
                   Bitnum(input, 24, 27) | Bitnum(input, 16, 26) | Bitnum(input, 8, 25) | Bitnum(input, 0, 24) |
                   Bitnum(input, 58, 23) | Bitnum(input, 50, 22) | Bitnum(input, 42, 21) | Bitnum(input, 34, 20) |
                   Bitnum(input, 26, 19) | Bitnum(input, 18, 18) | Bitnum(input, 10, 17) | Bitnum(input, 2, 16) |
                   Bitnum(input, 60, 15) | Bitnum(input, 52, 14) | Bitnum(input, 44, 13) | Bitnum(input, 36, 12) |
                   Bitnum(input, 28, 11) | Bitnum(input, 20, 10) | Bitnum(input, 12, 9) | Bitnum(input, 4, 8) |
                   Bitnum(input, 62, 7) | Bitnum(input, 54, 6) | Bitnum(input, 46, 5) | Bitnum(input, 38, 4) |
                   Bitnum(input, 30, 3) | Bitnum(input, 22, 2) | Bitnum(input, 14, 1) | Bitnum(input, 6, 0);
    }

    private static void InvIP(uint[] state, byte[] input)
    {
        input[0] = (byte)(Bitnumintr(state[1], 7, 7) | Bitnumintr(state[0], 7, 6) | Bitnumintr(state[1], 15, 5) |
                          Bitnumintr(state[0], 15, 4) | Bitnumintr(state[1], 23, 3) | Bitnumintr(state[0], 23, 2) |
                          Bitnumintr(state[1], 31, 1) | Bitnumintr(state[0], 31, 0));

        input[1] = (byte)(Bitnumintr(state[1], 6, 7) | Bitnumintr(state[0], 6, 6) | Bitnumintr(state[1], 14, 5) |
                          Bitnumintr(state[0], 14, 4) | Bitnumintr(state[1], 22, 3) | Bitnumintr(state[0], 22, 2) |
                          Bitnumintr(state[1], 30, 1) | Bitnumintr(state[0], 30, 0));

        input[2] = (byte)(Bitnumintr(state[1], 5, 7) | Bitnumintr(state[0], 5, 6) | Bitnumintr(state[1], 13, 5) |
                          Bitnumintr(state[0], 13, 4) | Bitnumintr(state[1], 21, 3) | Bitnumintr(state[0], 21, 2) |
                          Bitnumintr(state[1], 29, 1) | Bitnumintr(state[0], 29, 0));

        input[3] = (byte)(Bitnumintr(state[1], 4, 7) | Bitnumintr(state[0], 4, 6) | Bitnumintr(state[1], 12, 5) |
                          Bitnumintr(state[0], 12, 4) | Bitnumintr(state[1], 20, 3) | Bitnumintr(state[0], 20, 2) |
                          Bitnumintr(state[1], 28, 1) | Bitnumintr(state[0], 28, 0));

        input[4] = (byte)(Bitnumintr(state[1], 3, 7) | Bitnumintr(state[0], 3, 6) | Bitnumintr(state[1], 11, 5) |
                          Bitnumintr(state[0], 11, 4) | Bitnumintr(state[1], 19, 3) | Bitnumintr(state[0], 19, 2) |
                          Bitnumintr(state[1], 27, 1) | Bitnumintr(state[0], 27, 0));

        input[5] = (byte)(Bitnumintr(state[1], 2, 7) | Bitnumintr(state[0], 2, 6) | Bitnumintr(state[1], 10, 5) |
                          Bitnumintr(state[0], 10, 4) | Bitnumintr(state[1], 18, 3) | Bitnumintr(state[0], 18, 2) |
                          Bitnumintr(state[1], 26, 1) | Bitnumintr(state[0], 26, 0));

        input[6] = (byte)(Bitnumintr(state[1], 1, 7) | Bitnumintr(state[0], 1, 6) | Bitnumintr(state[1], 9, 5) |
                          Bitnumintr(state[0], 9, 4) | Bitnumintr(state[1], 17, 3) | Bitnumintr(state[0], 17, 2) |
                          Bitnumintr(state[1], 25, 1) | Bitnumintr(state[0], 25, 0));

        input[7] = (byte)(Bitnumintr(state[1], 0, 7) | Bitnumintr(state[0], 0, 6) | Bitnumintr(state[1], 8, 5) |
                          Bitnumintr(state[0], 8, 4) | Bitnumintr(state[1], 16, 3) | Bitnumintr(state[0], 16, 2) |
                          Bitnumintr(state[1], 24, 1) | Bitnumintr(state[0], 24, 0));
    }

    private static uint F(uint state, byte[] key)
    {
        byte[] lrgstate = new byte[6];
        uint t1, t2;

        t1 = Bitnumintl(state, 31, 0) | ((state & 0xf0000000) >> 1) | Bitnumintl(state, 4, 5) |
             Bitnumintl(state, 3, 6) | ((state & 0x0f000000) >> 3) | Bitnumintl(state, 8, 11) |
             Bitnumintl(state, 7, 12) | ((state & 0x00f00000) >> 5) | Bitnumintl(state, 12, 17) |
             Bitnumintl(state, 11, 18) | ((state & 0x000f0000) >> 7) | Bitnumintl(state, 16, 23);

        t2 = Bitnumintl(state, 15, 0) | ((state & 0x0000f000) << 15) | Bitnumintl(state, 20, 5) |
             Bitnumintl(state, 19, 6) | ((state & 0x00000f00) << 13) | Bitnumintl(state, 24, 11) |
             Bitnumintl(state, 23, 12) | ((state & 0x000000f0) << 11) | Bitnumintl(state, 28, 17) |
             Bitnumintl(state, 27, 18) | ((state & 0x0000000f) << 9) | Bitnumintl(state, 0, 23);

        lrgstate[0] = (byte)((t1 >> 24) & 0x000000ff);
        lrgstate[1] = (byte)((t1 >> 16) & 0x000000ff);
        lrgstate[2] = (byte)((t1 >> 8) & 0x000000ff);
        lrgstate[3] = (byte)((t2 >> 24) & 0x000000ff);
        lrgstate[4] = (byte)((t2 >> 16) & 0x000000ff);
        lrgstate[5] = (byte)((t2 >> 8) & 0x000000ff);

        lrgstate[0] ^= key[0];
        lrgstate[1] ^= key[1];
        lrgstate[2] ^= key[2];
        lrgstate[3] ^= key[3];
        lrgstate[4] ^= key[4];
        lrgstate[5] ^= key[5];

        state = (uint)((Sbox1[Sboxbit((byte)(lrgstate[0] >> 2))] << 28) |
                       (Sbox2[Sboxbit((byte)(((lrgstate[0] & 0x03) << 4) | (lrgstate[1] >> 4)))] << 24) |
                       (Sbox3[Sboxbit((byte)(((lrgstate[1] & 0x0f) << 2) | (lrgstate[2] >> 6)))] << 20) |
                       (Sbox4[Sboxbit((byte)(lrgstate[2] & 0x3f))] << 16) |
                       (Sbox5[Sboxbit((byte)(lrgstate[3] >> 2))] << 12) |
                       (Sbox6[Sboxbit((byte)(((lrgstate[3] & 0x03) << 4) | (lrgstate[4] >> 4)))] << 8) |
                       (Sbox7[Sboxbit((byte)(((lrgstate[4] & 0x0f) << 2) | (lrgstate[5] >> 6)))] << 4) |
                       Sbox8[Sboxbit((byte)(lrgstate[5] & 0x3f))]);

        state = Bitnumintl(state, 15, 0) | Bitnumintl(state, 6, 1) | Bitnumintl(state, 19, 2) |
                Bitnumintl(state, 20, 3) | Bitnumintl(state, 28, 4) | Bitnumintl(state, 11, 5) |
                Bitnumintl(state, 27, 6) | Bitnumintl(state, 16, 7) | Bitnumintl(state, 0, 8) |
                Bitnumintl(state, 14, 9) | Bitnumintl(state, 22, 10) | Bitnumintl(state, 25, 11) |
                Bitnumintl(state, 4, 12) | Bitnumintl(state, 17, 13) | Bitnumintl(state, 30, 14) |
                Bitnumintl(state, 9, 15) | Bitnumintl(state, 1, 16) | Bitnumintl(state, 7, 17) |
                Bitnumintl(state, 23, 18) | Bitnumintl(state, 13, 19) | Bitnumintl(state, 31, 20) |
                Bitnumintl(state, 26, 21) | Bitnumintl(state, 2, 22) | Bitnumintl(state, 8, 23) |
                Bitnumintl(state, 18, 24) | Bitnumintl(state, 12, 25) | Bitnumintl(state, 29, 26) |
                Bitnumintl(state, 5, 27) | Bitnumintl(state, 21, 28) | Bitnumintl(state, 10, 29) |
                Bitnumintl(state, 3, 30) | Bitnumintl(state, 24, 31);

        return (state);
    }

    public static void Crypt(byte[] input, byte[] output, byte[][] key)
    {
        uint[] state = new uint[2];
        uint idx, t;

        IP(state, input);

        for (idx = 0; idx < 15; ++idx)
        {
            t = state[1];
            state[1] = F(state[1], key[idx]) ^ state[0];
            state[0] = t;
        }

        state[0] = F(state[1], key[15]) ^ state[0];

        InvIP(state, output);
    }

    private static void PrintBytes(byte[] hash)
    {
        for (int i = 0; i < 8; ++i)
            Console.Write("{0:x2} ", hash[i]);

        Console.WriteLine();
    }

    private static T[][] ToJaggedArray<T>(T[,] twoDimensionalArray)
    {
        int rowsFirstIndex = twoDimensionalArray.GetLowerBound(0);
        int rowsLastIndex = twoDimensionalArray.GetUpperBound(0);
        int numberOfRows = rowsLastIndex + 1;

        int columnsFirstIndex = twoDimensionalArray.GetLowerBound(1);
        int columnsLastIndex = twoDimensionalArray.GetUpperBound(1);
        int numberOfColumns = columnsLastIndex + 1;

        T[][] jaggedArray = new T[numberOfRows][];

        for (int i = rowsFirstIndex; i <= rowsLastIndex; i++)
        {
            jaggedArray[i] = new T[numberOfColumns];

            for (int j = columnsFirstIndex; j <= columnsLastIndex; j++)
            {
                jaggedArray[i][j] = twoDimensionalArray[i, j];
            }
        }

        return jaggedArray;
    }

    public static void Demo()
    {
        string textString = "Hello World";
        byte[] text = Encoding.ASCII.GetBytes(textString);
        byte[] key = { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };
        byte[] output = new byte[8];
        byte[,] schedule = new byte[16, 6];

        Console.WriteLine("Original: \n\tin text: " + textString);
        Console.Write("\tin bytes: ");
        PrintBytes(text);

        KeySchedule(key, schedule, Encrypt);
        Crypt(text, output, ToJaggedArray(schedule));
        Console.Write("Encrypt: \n\tin hash: ");
        PrintBytes(output);
        Console.WriteLine("\tin text: " + Encoding.ASCII.GetString(output));

        KeySchedule(key, schedule, Decrypt);
        Crypt(output, text, ToJaggedArray(schedule));
        Console.Write("Decrypt: \n\tin hash: ");
        PrintBytes(text);
        Console.Write("\tin text: " + Encoding.ASCII.GetString(text));
    }
}