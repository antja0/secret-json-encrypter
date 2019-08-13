using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable once IdentifierTypo
namespace SecretEncrypter
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("Path to the json file: ");
            var path = Console.ReadLine();

            // New CSP with a new 2048 bit rsa key pair.
            var cryptoServiceProvider = new RSACryptoServiceProvider(2048);

            var privateKey = cryptoServiceProvider.ExportParameters(true);
            var pubKey = cryptoServiceProvider.ExportParameters(false);

            Console.WriteLine("Encrypting secrets...");

            JObject secrets = JObject.Parse(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, path)));

            var encryptedSecrets = new Dictionary<string, string>();
            foreach (var (key, value) in secrets)
            {
                string valueString = value.ToObject<string>();
                var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(valueString);
                var bytesCypherText = cryptoServiceProvider.Encrypt(bytesPlainTextData, true);
                var cypherText = Convert.ToBase64String(bytesCypherText);
                encryptedSecrets.Add(key, cypherText);
            }

            Console.WriteLine("Secrets encrypted. Writing json...");
            using (StreamWriter file = File.CreateText(@"secrets.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, encryptedSecrets);
            }
            Console.WriteLine("Success!");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
