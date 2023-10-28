// See https://aka.ms/new-console-template for more information

using Lab1;

string inputText = "програмнезабезпечення";
string rowKey = "ширф";
string columnKey = "крипто";

string encryptedText = CipherOfPermutations.Encrypt(inputText, rowKey, columnKey);
Console.WriteLine("Encrypted Text: " + encryptedText);

string decryptedText = CipherOfPermutations.Decrypt(encryptedText, rowKey, columnKey);
Console.WriteLine("Decrypted Text: " + decryptedText);
