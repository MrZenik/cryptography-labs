namespace Lab5;

/*
 * Реалізувати у вигляді функцій mul02 і mul03 множення довільного байту на елементи (байти) 02 і 03
 * над полем Галуа GF (2^8) за модулем m(x) = x^8 + x^4 + x^3 + x + 1,
 * використавши методику зсуву бітів і операцію ХОR (малюнки 2-4)
   Протестувати на прикладах: D4 * 02 = B3, BF * 03 = DA
   Зауваження:
   1) 02 = x, 03 = x + 1
   2) Множення на 03 зводиться до множення на 02:   BF * 03 = BF * (02+01) = BF * 02 + BF
*/
public class Lab5
{
    static byte Mul02(byte b)
    {
        // Множення на 02 в полі Галуа GF(2^8)
        byte result = (byte)(b << 1); // Зсуваємо байт на 1 біт вліво
        if ((b & 0x80) != 0) // Перевіряємо найстарший біт
            result ^= 0x1B; // XOR з 0x1B, якщо найстарший біт був 1
        return result;
    }

    static byte Mul03(byte b)
    {
        // Множення на 03 в полі Галуа GF(2^8)
        return (byte)(Mul02(b) ^ b);
    }

    public static void Demo()
    {
        byte d4 = 0xD4;
        byte bf = 0xBF;

        byte result1 = Mul02(d4);
        byte result2 = Mul03(bf);

        Console.WriteLine("D4 * 02 = {0:X2}", result1); // Виводимо результат в шістнадцятковому форматі
        Console.WriteLine("BF * 03 = {0:X2}", result2);
    }
}