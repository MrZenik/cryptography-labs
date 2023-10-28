namespace Lab2;

class VigenereCipher
{
    private const string UkrainianAlphabet = "абвгґдеєжзиіїйклмнопрстуфхцчшщьюя";

    public static string Encrypt(string text, string key)
    {
        string encryptedText = "";
        int textLength = text.Length;

        for (int i = 0; i < textLength; i++)
        {
            char plainChar = text[i];
            char keyChar = key[i % key.Length];
            int shift = UkrainianAlphabet.IndexOf(keyChar);

            if (UkrainianAlphabet.Contains(plainChar.ToString()))
            {
                int newIndex = (UkrainianAlphabet.IndexOf(plainChar) + shift) % UkrainianAlphabet.Length;
                encryptedText += UkrainianAlphabet[newIndex];
            }
            else
            {
                encryptedText += plainChar;
            }
        }

        return encryptedText;
    }

    public static string Decrypt(string text, string key)
    {
        string decryptedText = "";
        int textLength = text.Length;

        for (int i = 0; i < textLength; i++)
        {
            char cipherChar = text[i];
            char keyChar = key[i % key.Length];
            int shift = UkrainianAlphabet.IndexOf(keyChar);

            if (UkrainianAlphabet.Contains(cipherChar.ToString()))
            {
                int newIndex = (UkrainianAlphabet.IndexOf(cipherChar) - shift + UkrainianAlphabet.Length) % UkrainianAlphabet.Length;
                decryptedText += UkrainianAlphabet[newIndex];
            }
            else
            {
                decryptedText += cipherChar;
            }
        }

        return decryptedText;
    }
    
}