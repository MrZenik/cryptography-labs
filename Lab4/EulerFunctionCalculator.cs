namespace Lab4;

public class EulerFunctionCalculator
{
    static int EulerPhi(int m)
    {
        if (m < 1)
        {
            throw new ArgumentException("Число m повинно бути більше або рівним 1");
        }

        int result = m;

        for (int i = 2; i * i <= m; i++)
        {
            if (m % i == 0)
            {
                while (m % i == 0)
                {
                    m /= i;
                }
                result -= result / i;
            }
        }
        
        if (m > 1)
        {
            result -= result / m;
        }

        return result;
    }

    public static void Demo(int m)
    {
        int phiM = EulerPhi(m); 

        Console.WriteLine($"Значення функції Ейлера для {m} дорівнює {phiM}");
    }
}