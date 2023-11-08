// See https://aka.ms/new-console-template for more information

string plaintext = "HELLO WORLD";
Console.WriteLine("Огриніальний текст: " + plaintext);

string ciphertext = AffineCipher.AffineCipher.EncryptMessage(plaintext.ToCharArray());
Console.WriteLine("Зашифровано: " + ciphertext);

string decryptedText = AffineCipher.AffineCipher.DecryptCipher(ciphertext);
Console.WriteLine("Розшифровано: " + decryptedText);