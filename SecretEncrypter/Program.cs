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
            Console.Write("Public key (Or leave empty and generates new pair):");
            var publicKeyString = Console.ReadLine();

            RSACryptoServiceProvider cryptoServiceProvider;

            if (string.IsNullOrEmpty(publicKeyString))
            {
                // New CSP with a new 2048 bit rsa key pair.
                cryptoServiceProvider = new RSACryptoServiceProvider(2048);

                var privateKey = cryptoServiceProvider.ExportParameters(true);
                var publicKey = cryptoServiceProvider.ExportParameters(false);

                Console.WriteLine();
                Console.WriteLine("Private key: " + RsaParameterToString(privateKey));
                Console.WriteLine();
                Console.WriteLine("Public key: " + RsaParameterToString(publicKey));
                Console.WriteLine();
            }
            else
            {
                var sr = new StringReader(publicKeyString);
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                var publicKey = (RSAParameters)xs.Deserialize(sr);

                cryptoServiceProvider = new RSACryptoServiceProvider();
                cryptoServiceProvider.ImportParameters(publicKey);
            }

            Console.Write("Path to the json file: ");
            var path = Console.ReadLine();

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

        private static string RsaParameterToString(RSAParameters rsaParam)
        {
            var sw = new StringWriter();
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, rsaParam);
            return sw.ToString();
        }
    }
}
