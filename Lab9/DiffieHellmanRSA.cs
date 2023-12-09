using System.Numerics;
using System.Text;

namespace Lab9;

public class DiffieHellmanRSA
{
    static BigInteger ModularExponentiation(BigInteger baseValue, BigInteger exponent, BigInteger modulus)
    {
        return BigInteger.ModPow(baseValue, exponent, modulus);
    }

    static bool IsPrime(BigInteger number, int iterations = 10)
    {
        if (number < 2)
            return false;
        if (number == 2)
            return true;
        if (number.IsEven)
            return false;

        BigInteger d = number - 1;
        int r = 0;
        while (d.IsEven)
        {
            d /= 2;
            r += 1;
        }

        Random random = new Random();
        for (int i = 0; i < iterations; i++)
        {
            BigInteger a = 2 + BigIntegerExtensions.RandomNumberBelow(number - 4, random);
            BigInteger x = ModularExponentiation(a, d, number);
            if (x == 1 || x == number - 1)
                continue;

            for (int j = 0; j < r - 1; j++)
            {
                x = ModularExponentiation(x, 2, number);
                if (x == 1)
                    return false;
                if (x == number - 1)
                    break;
            }

            if (x != number - 1)
                return false;
        }

        return true;
    }

    static BigInteger GenerateLargePrime(int bits)
    {
        BigInteger prime;
        do
        {
            prime = BigIntegerExtensions.RandomBigInteger(bits);
            prime |= BigInteger.One << (bits - 1) | BigInteger.One;
        } while (!IsPrime(prime));

        return prime;
    }

    static BigInteger FindPrimitiveRoot(BigInteger p)
    {
        BigInteger phi = p - 1;
        var factors = PrimeFactors(phi);
        for (BigInteger g = 2; g < phi; g++)
        {
            bool isPrimitiveRoot = factors.All(factor => ModularExponentiation(g, phi / factor, p) != 1);
            if (isPrimitiveRoot)
                return g;
        }

        throw new InvalidOperationException("Primitive root not found.");
    }

    static List<BigInteger> PrimeFactors(BigInteger n)
    {
        List<BigInteger> factors = new List<BigInteger>();
        BigInteger d = 2;
        while (d * d <= n)
        {
            while (n % d == 0)
            {
                if (!factors.Contains(d))
                    factors.Add(d);
                n /= d;
            }

            d += 1;
        }

        if (n > 1)
            factors.Add(n);

        return factors;
    }

    static (BigInteger[], BigInteger) GenerateKeys(int bits)
    {
        BigInteger p = GenerateLargePrime(bits);
        BigInteger g = FindPrimitiveRoot(p);
        BigInteger x = BigIntegerExtensions.RandomNumberInRange(2, p - 1);
        BigInteger y = ModularExponentiation(g, x, p);

        return (new[] { p, g, y }, x);
    }

    static (BigInteger, BigInteger) Encrypt(BigInteger[] publicKey, BigInteger message)
    {
        BigInteger p = publicKey[0];
        BigInteger g = publicKey[1];
        BigInteger y = publicKey[2];

        BigInteger k = BigIntegerExtensions.RandomNumberInRange(2, p - 1);
        BigInteger a = ModularExponentiation(g, k, p);
        BigInteger b = (message * ModularExponentiation(y, k, p)) % p;

        return (a, b);
    }

    static BigInteger Decrypt(BigInteger privateKey, (BigInteger, BigInteger) encryptedMessage, BigInteger p)
    {
        BigInteger a = encryptedMessage.Item1;
        BigInteger b = encryptedMessage.Item2;

        BigInteger s = ModularExponentiation(a, privateKey, p);
        return (b * ModularExponentiation(s, p - 2, p)) % p;
    }

    static string NumbersToString(IEnumerable<byte> numbers)
    {
        return Encoding.UTF8.GetString(numbers.ToArray());
    }

    static List<BigInteger> StringToNumbers(string str)
    {
        return Encoding.UTF8.GetBytes(str).Select(b => (BigInteger)b).ToList();
    }

    static List<(BigInteger, BigInteger)> EncryptString(BigInteger[] publicKey, string text)
    {
        var numbers = StringToNumbers(text);
        return numbers.Select(number => Encrypt(publicKey, number)).ToList();
    }

    static string DecryptString(BigInteger privateKey,
        List<(BigInteger, BigInteger)> encryptedData, BigInteger p)
    {
        var decryptedNumbers = encryptedData.Select(data => Decrypt(privateKey, data, p));
        return NumbersToString(decryptedNumbers.Select(b => (byte)b));
    }

    public static void Demo()
    {
        var keys = GenerateKeys(64);
        Console.WriteLine($"Public Key: [{keys.Item1[0]}, {keys.Item1[1]}, {keys.Item1[2]}]");
        Console.WriteLine($"Private Key: {keys.Item2}");

        string message = "Hello World!";
        Console.WriteLine($"Original Message: {message}");

        var encryptedMessage = EncryptString(keys.Item1, message);
        Console.WriteLine($"Encrypted Message: {string.Join(", ", encryptedMessage)}");

        var decryptedMessage = DecryptString(keys.Item2, encryptedMessage, keys.Item1[0]);
        Console.WriteLine($"Decrypted Message: {decryptedMessage}");
    }
}

public static class BigIntegerExtensions
{
    public static BigInteger RandomBigInteger(int bitLength)
    {
        Random random = new Random();
        byte[] data = new byte[bitLength / 8];
        random.NextBytes(data);
        return new BigInteger(data);
    }

    public static BigInteger RandomNumberBelow(BigInteger below, Random random)
    {
        BigInteger result;
        do
        {
            byte[] bytes = new byte[below.ToByteArray().Length];
            random.NextBytes(bytes);
            result = new BigInteger(bytes);
        } while (result >= below);

        return result;
    }

    public static BigInteger RandomNumberInRange(BigInteger min, BigInteger max)
    {
        Random random = new Random();
        BigInteger result;
        do
        {
            byte[] bytes = new byte[max.ToByteArray().Length];
            random.NextBytes(bytes);
            result = new BigInteger(bytes);
        } while (result < min || result > max);

        return result;
    }
}