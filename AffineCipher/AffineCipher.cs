namespace AffineCipher;

public static class AffineCipher
{
    private const int A = 3;
    private const int S = 20;
    private const int N = 26;

    public static string EncryptMessage(char[] textArray)
    {
        string cipherText = "";
        foreach (var t in textArray)
        {
            if (t != ' ')
            {
                cipherText += (char)((A * (t - 'A') + S) % N + 'A');
            }
            else
            {
                cipherText += t;
            }
        }

        return cipherText;
    }

    public static string DecryptCipher(string cipherText)
    {
        var decryptedText = "";
        var aInv = 0;

        for (var i = 0; i < N; i++)
        {
            var flag = (A * i) % N;
            if (flag == 1)
            {
                aInv = i;
            }
        }

        foreach (var t in cipherText)
        {
            if (t != ' ')
            {
                decryptedText += (char)(aInv * (t + 'A' - S) % N + 'A');
            }
            else
            {
                decryptedText += t;
            }
        }

        return decryptedText;
    }
}