namespace Lab1;

public class CipherOfPermutations
{
    public static string Encrypt(string input, string rowKey, string columnKey)
    {
        int rowKeyLength = rowKey.Length;
        int columnKeyLength = columnKey.Length;
        char[,] matrix = new char[rowKeyLength, columnKeyLength];
        int index = 0;

        for (int i = 0; i < rowKeyLength; i++)
        {
            for (int j = 0; j < columnKeyLength; j++)
            {
                if (index < input.Length)
                {
                    matrix[i, j] = input[index++];
                }
            }
        }

        var sortedMatrix = rowKey.Select((c, i) => new { Char = c, Index = i })
            .OrderBy(item => item.Index)
            .Select(item => item.Char)
            .ToArray();

        string encryptedText = string.Concat(columnKey.OrderBy(c => c)
            .Select(ch => string.Concat(Enumerable.Range(0, rowKeyLength)
            .Select(i => matrix[Array.IndexOf(sortedMatrix, rowKey[i]), columnKey.IndexOf(ch)]))));

        return encryptedText;
    }

    public static string Decrypt(string encryptedText, string rowKey, string columnKey)
    {
        int rowKeyLength = rowKey.Length;
        int columnKeyLength = columnKey.Length;
        char[,] matrix = new char[rowKeyLength, columnKeyLength];
        int index = 0;

        foreach (char ch in columnKey.OrderBy(c => c))
        {
            int colIndex = columnKey.IndexOf(ch);

            for (int i = 0; i < rowKeyLength; i++)
            {
                if (index < encryptedText.Length)
                {
                    matrix[i, colIndex] = encryptedText[index++];
                }
            }
        }

        var originalOrder = rowKey.Select((c, i) => new { Char = c, Index = i })
            .OrderBy(item => item.Index)
            .Select(item => item.Char)
            .ToArray();

        string decryptedText = string.Concat(Enumerable.Range(0, rowKeyLength)
            .Select(i => string.Concat(Enumerable.Range(0, columnKeyLength)
                .Select(j => matrix[Array.IndexOf(originalOrder, rowKey[i]), j]))));

        return decryptedText.Trim();
    }
}