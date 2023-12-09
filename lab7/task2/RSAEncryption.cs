using System.Numerics;
using System.Text;

namespace lab7.task2;

class RSAEncryption
{
    static BigInteger StringToNumber(string str)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        Array.Reverse(bytes);
        return new BigInteger(bytes);
    }

    static string NumberToString(BigInteger num)
    {
        byte[] bytes = num.ToByteArray();
        Array.Reverse(bytes);
        return Encoding.UTF8.GetString(bytes);
    }

    static BigInteger ModExp(BigInteger baseValue, BigInteger exponent, BigInteger modulus)
    {
        BigInteger result = 1;
        baseValue = baseValue % modulus;

        while (exponent > 0)
        {
            if (exponent % 2 == 1)
                result = (result * baseValue) % modulus;

            exponent >>= 1;
            baseValue = (baseValue * baseValue) % modulus;
        }

        return result;
    }

    static bool MillerRabinTest(BigInteger n, int k)
    {
        if (n < 2)
            return false;
        if (n == 2)
            return true;
        if (n.IsEven)
            return false;

        BigInteger r = 0;
        BigInteger d = n - 1;

        while (d.IsEven)
        {
            r += 1;
            d >>= 1;
        }

        Random rand = new Random();

        for (int i = 0; i < k; i++)
        {
            BigInteger a = 2 + BigInteger.Remainder(rand.Next(), n - 4);
            BigInteger x = ModExp(a, d, n);

            if (x == 1 || x == n - 1)
                continue;

            for (BigInteger j = 0; j < r - 1; j++)
            {
                x = ModExp(x, 2, n);

                if (x == 1)
                    return false;
                if (x == n - 1)
                    break;
            }

            if (x != n - 1)
                return false;
        }

        return true;
    }

    static BigInteger GeneratePrime(int bits)
    {
        Random rand = new Random();
        BigInteger n;

        do
        {
            n = BigInteger.Pow(2, bits - 1) + BigInteger.Remainder(rand.Next(), BigInteger.Pow(2, bits - 1));
        } while (n.IsEven || !MillerRabinTest(n, 5));

        return n;
    }

    static Tuple<BigInteger, BigInteger> ExtendedGCD(BigInteger a, BigInteger b)
    {
        BigInteger x0 = 1, x1 = 0, y0 = 0, y1 = 1;

        while (b != 0)
        {
            BigInteger q, r;
            q = BigInteger.DivRem(a, b, out r);
            a = b;
            b = r;
            BigInteger temp = x1;
            x1 = x0 - q * x1;
            x0 = temp;
            temp = y1;
            y1 = y0 - q * y1;
            y0 = temp;
        }

        return Tuple.Create(x0, y0);
    }

    static BigInteger ModInverse(BigInteger e, BigInteger phi)
    {
        BigInteger x = ExtendedGCD(e, phi).Item1;
        x %= phi;

        if (x < 0)
            x += phi;

        return x;
    }

    static BigInteger GenerateE(BigInteger phi)
    {
        Random rand = new Random();
        BigInteger e;

        do
        {
            e = BigInteger.Remainder(rand.Next(), phi - 1) + 2;
        } while (BigInteger.GreatestCommonDivisor(e, phi) != 1);

        return e;
    }

    static BigInteger Encrypt(BigInteger message, BigInteger e, BigInteger n)
    {
        return ModExp(message, e, n);
    }

    static BigInteger Decrypt(BigInteger ciphertext, BigInteger d, BigInteger n)
    {
        return ModExp(ciphertext, d, n);
    }

    public static void Demo()
    {
        int bits = 512;
        Random rand = new Random();

        BigInteger p = GeneratePrime(bits);
        BigInteger q = GeneratePrime(bits);

        BigInteger n = p * q;
        BigInteger phi = (p - 1) * (q - 1);

        BigInteger e = GenerateE(phi);
        BigInteger d = ModInverse(e, phi);

        Console.WriteLine($"Відкритий ключ: ({e}, {n})");
        Console.WriteLine($"Закритий ключ: ({d}, {n})");

        string messageStr = "Hello World";
        BigInteger messageNum = StringToNumber(messageStr);

        if (messageNum >= n)
            throw new Exception("Повідомлення занадто велике");

        BigInteger ciphertext = Encrypt(messageNum, e, n);
        Console.WriteLine($"Зашифроване повідомлення: {ciphertext}");

        BigInteger decryptedNum = Decrypt(ciphertext, d, n);
        string decryptedStr = NumberToString(decryptedNum);
        Console.WriteLine($"Розшифроване повідомлення: {decryptedStr}");
    }
}