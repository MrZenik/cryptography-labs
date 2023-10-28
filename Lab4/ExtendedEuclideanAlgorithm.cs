namespace Lab4;

// Task 1
//1. Реалізувати у вигляді функції gcdex(a,b) ітераційний розширений алгоритм Евкліда пошуку трійки (d,x,y),
//   де ax+by = d. Протестити на прикладі  a=612 і b=342.
class ExtendedEuclideanAlgorithm
{
    public static void Demo()
    {
        const int a = 612;
        const int b = 342;

        Tuple<int, int, int> result = FindGcdAndCoefficients(a, b);

        int gcd = result.Item1;
        int x = result.Item2;
        int y = result.Item3;

        Console.WriteLine($"gcd({a}, {b}) = {gcd}");
        Console.WriteLine($"x = {x}, y = {y}");
    }

    static Tuple<int, int, int> FindGcdAndCoefficients(int a, int b)
    {
        int x0 = 1, y0 = 0, x1 = 0, y1 = 1;

        while (b != 0)
        {
            int temp = b;
            int q = a / b;
            b = a % b;
            a = temp;

            int oldX = x1, oldY = y1;
            x1 = x0 - q * x1;
            y1 = y0 - q * y1;
            x0 = oldX;
            y0 = oldY;
        }

        return Tuple.Create(a, x0, y0);
    }
}