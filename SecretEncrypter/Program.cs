using System;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;

// ReSharper disable once IdentifierTypo
namespace SecretEncrypter
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            Console.Write("Path to the json file: ");
            var path = Console.ReadLine();

            // New CSP with a new 2048 bit rsa key pair.
            var cryptoServiceProvider = new RSACryptoServiceProvider(2048);

            var privateKey = cryptoServiceProvider.ExportParameters(true);
            var pubKey = cryptoServiceProvider.ExportParameters(false);

            JObject secrets = JObject.Parse(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, path)));

            foreach (var (key, value) in secrets)
            {
                string valueString = value.First.ToObject<string>();
                var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(valueString);
                var bytesCypherText = cryptoServiceProvider.Encrypt(bytesPlainTextData, false);
                var cypherText = Convert.ToBase64String(bytesCypherText);
                Console.Write(cypherText);
            }

            Console.ReadKey();
        }
    }
}
