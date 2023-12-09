namespace lab7.task1;

class PrimalityTester
{
    static int ModExp(int baseValue, int exponent, int modulus)
    {
        int result = 1;
        baseValue %= modulus;

        while (exponent > 0)
        {
            if (exponent % 2 == 1)
                result = (result * baseValue) % modulus;

            exponent >>= 1;
            baseValue = (baseValue * baseValue) % modulus;
        }

        return result;
    }

    static bool MillerRabinTest(int n, int k)
    {
        if (n < 2)
            return false;
        if (n == 2)
            return true;
        if (n % 2 == 0)
            return false;

        int r = 0;
        int d = n - 1;
        while (d % 2 == 0)
        {
            r += 1;
            d >>= 1;
        }

        Random rand = new Random();

        for (int i = 0; i < k; i++)
        {
            int a = 2 + rand.Next(n - 4);
            int x = ModExp(a, d, n);

            if (x == 1 || x == n - 1)
                continue;

            for (int j = 0; j < r - 1; j++)
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

    public static void Demo()
    {
        int number = 71;
        int iterations = 10;

        if (MillerRabinTest(number, iterations))
        {
            double probability = Math.Round((1 - Math.Pow(4.0, -iterations)), 10);
            Console.WriteLine($"{number} є простим числом з ймовірністю {probability}");
        }
        else
        {
            Console.WriteLine($"{number} є складеним числом");
        }
    }
}