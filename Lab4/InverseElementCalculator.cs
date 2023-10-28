namespace Lab4;

/*
 * Завдання 4
 * 4. Реалізувати у вигляді функції inverse_element_2(a,p) знаходження мультиплікативного оберненого елемента a^(-1) по модулю числа n,
 *    використовуючи інший спосіб (теорему Ейлера або малу теорему Ферма у випадку простого числа n=p).
 *    Протестити на прикладі  a= 5 і  n=18.
 */
public class InverseElementCalculator
{
    public static int InverseElement(int a, int n)
    {
        int phi = EulerTotientFunction(n); // Розраховуємо значення функції Ейлера для n
        int result = ModuloPower(a, phi - 1, n); // Знаходимо a^(-1) за модулем n
        return result;
    }

    public static int EulerTotientFunction(int n)
    {
        int result = 1;
        for (int i = 2; i < n; i++)
        {
            if (GCD(n, i) == 1)
            {
                result++;
            }
        }

        return result;
    }

    public static int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }

    public static int ModuloPower(int a, int b, int n)
    {
        if (b == 0)
            return 1;
        if (b % 2 == 0)
        {
            int temp = ModuloPower(a, b / 2, n);
            return (temp * temp) % n;
        }
        else
        {
            return (a * ModuloPower(a, b - 1, n)) % n;
        }
    }

    public static void Demo()
    {
        int a = 5;
        int n = 18;
        int result = InverseElement(a, n);
        Console.WriteLine($"Mультиплікативний обернений елемент {a} по модулю {n} дорівнює {result}");
    }
}