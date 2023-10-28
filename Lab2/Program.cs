// See https://aka.ms/new-console-template for more information

using Lab2;

string plaintext = "наступаємонасвітанку";
string keyword = "віженервіженервіжене";

string encryptedText = VigenereCipher.Encrypt(plaintext, keyword);
string decryptedText = VigenereCipher.Decrypt(encryptedText, keyword);

Console.WriteLine("Оригінальний текст: " + plaintext);
Console.WriteLine("Зашифрований текст: " + encryptedText);
Console.WriteLine("Розшифрований текст: " + decryptedText);