namespace Lab8;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

class Lab8Code
{
    static bool IsPrime(BigInteger n, int k = 10)
    {
        if (n <= 1 || n == 4)
            return false;
        if (n <= 3)
            return true;

        BigInteger d = n - 1;
        while (d.IsEven)
        {
            d /= 2;
        }

        Random rand = new Random();
        for (int i = 0; i < k; i++)
        {
            BigInteger a;
            do
            {
                byte[] temp = new byte[n.ToByteArray().LongLength];
                rand.NextBytes(temp);
                temp[^1] &= 0x7F; // ensure positive number
                a = new BigInteger(temp);
            } while (a < 2 || a >= n - 2);

            BigInteger x = BigInteger.ModPow(a, d, n);
            if (x == 1 || x == n - 1)
                continue;

            while (d != n - 1)
            {
                x = BigInteger.ModPow(x, 2, n);
                d *= 2;

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

    static BigInteger GenerateLargePrime(int bits)
    {
        Random rand = new Random();
        BigInteger n;
        do
        {
            byte[] bytes = new byte[bits / 8];
            rand.NextBytes(bytes);
            bytes[^1] &= 0x7F; // ensure positive number
            n = new BigInteger(bytes);
        } while (!IsPrime(n));

        return n;
    }

    static BigInteger PrimitiveRoot(BigInteger p)
    {
        if (p == 2 || p == 3)
            return 2;

        BigInteger phi = p - 1;
        List<BigInteger> factors = PrimeFactors(phi);

        for (BigInteger g = 2; g < p; g++)
        {
            if (factors.All(factor => BigInteger.ModPow(g, phi / factor, p) != 1))
                return g;
        }

        throw new Exception("Primitive root not found");
    }

    static List<BigInteger> PrimeFactors(BigInteger n)
    {
        List<BigInteger> factors = new List<BigInteger>();
        BigInteger d = 2;

        while (d * d <= n)
        {
            while (n % d == 0)
            {
                factors.Add(d);
                n /= d;
            }
            d++;
        }

        if (n > 1)
            factors.Add(n);

        return factors.Distinct().ToList();
    }

    static Tuple<BigInteger, BigInteger> GenerateKeys(BigInteger p, BigInteger g)
    {
        Random rand = new Random();
        BigInteger privateKey;

        do
        {
            byte[] temp = new byte[p.ToByteArray().Length];
            rand.NextBytes(temp);
            privateKey = new BigInteger(temp);
        } while (privateKey < 2 || privateKey >= p - 1);

        BigInteger publicKey = BigInteger.ModPow(g, privateKey, p);

        return Tuple.Create(privateKey, publicKey);
    }


    public static void Demo()
    {
        BigInteger p = GenerateLargePrime(64);
        BigInteger g = PrimitiveRoot(p);

        Console.WriteLine($"p = {p}");
        Console.WriteLine($"g = {g}");

        var aliceKeys = GenerateKeys(p, g);
        var bobKeys = GenerateKeys(p, g);

        Console.WriteLine("\nAlpha: \n\tПриватний ключ: {0} \n\tПублічний ключ: {1}", aliceKeys.Item1, aliceKeys.Item2);
        Console.WriteLine("\nBeta: \n\tПриватний ключ: {0} \n\tПублічний ключ: {1}\n\n", bobKeys.Item1, bobKeys.Item2);

        BigInteger aliceSharedSecret = BigInteger.ModPow(bobKeys.Item2, aliceKeys.Item1, p);
        BigInteger bobSharedSecret = BigInteger.ModPow(aliceKeys.Item2, bobKeys.Item1, p);

        if (aliceSharedSecret == bobSharedSecret)
        {
            Console.WriteLine("Обмін ключами успішний. Спільний секретний ключ: {0}", aliceSharedSecret);
        }
        else
        {
            Console.WriteLine("Помилка в обміні ключами.");
        }
    }
}
