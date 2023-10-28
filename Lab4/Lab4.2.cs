namespace Lab4;

/*
 * Завдання 2
 * 2. Реалізувати у вигляді функції inverse_element(a,n) пошук розв'язку рівняння ax=1 (mod n),
 * тобто знаходження мультиплікативноо оберненого елемента a^(-1) по модулю n,
 * використовуючи gcdex(a,b).  Протестити на прикладі  a = 5 і  n=18.
 */
class Lab4_2
{
    private static int GCD(int a, int b, out int x, out int y)
    {
        if (b == 0)
        {
            x = 1;
            y = 0;
            return a;
        }

        int x1, y1;
        int gcd = GCD(b, a % b, out x1, out y1);
        
        x = y1;
        y = x1 - (a / b) * y1;

        return gcd;
    }

    static int ModuloInverse(int a, int n)
    {
        int x, y;
        int gcd = GCD(a, n, out x, out y);

        if (gcd != 1)
        {
            throw new ArithmeticException("Multiplicative inverse does not exist.");
        }
        
        x = (x % n + n) % n;

        return x;
    }

    public static void Demo()
    {
        int a = 5;
        int n = 18;
        try
        {
            int inverse = ModuloInverse(a, n);
            Console.WriteLine($"Мультиплікативний обернений елемент числа {a} за модулем {n} дорівнює {inverse}");
        }
        catch (ArithmeticException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}