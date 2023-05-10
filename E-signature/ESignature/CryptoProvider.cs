using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ESignature
{
    internal class CryptoProvider
    {
        private static CspParameters cspParams;
//        private static RSACryptoServiceProvider rsaProvider;
        private static DSACryptoServiceProvider dsaProvider;
        private static RSA rsa_private;
        private static RSA rsa_public;
        private static X509Store x509storage;
        
        // Иницииализация хранилища
        public static void SetupCertStorage()
        {
            x509storage = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            x509storage.Open(OpenFlags.ReadOnly);
        }

        // Возвращает коллекцию сертификатов
        public static X509Certificate2Collection GetX509Certificate2Collection()
        {
            if (x509storage == null) SetupCertStorage();
            else if (!x509storage.IsOpen) x509storage.Open(OpenFlags.ReadOnly);
            return x509storage.Certificates;
        }

        // Удаляет сертификат
        public static void RemoveCert(X509Certificate2 certificate)
        {
            x509storage.Close();
            x509storage.Open(OpenFlags.ReadWrite);
            x509storage.Remove(certificate);
            x509storage.Close();
        }

        // Проверяет подлинность сертификата
        public static bool ValidateCertificate(X509Certificate2 certificate)
        {
            X509Chain chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            return chain.Build(certificate);
        }
        // Устанавлвиает закрытый ключ сертификата для подписи документа
        public static void SetPrivateKey(X509Certificate2 certificate)
        {
            //cspParams = new CspParameters() { ProviderType = 1 };
            //rsaProvider = new RSACryptoServiceProvider(cspParams);
            rsa_private = certificate.GetRSAPrivateKey();
            if (rsa_private == null) throw new Exception();
            //rsaProvider.ImportCspBlob(rsa);
        }
        // Устанавлвиает закрытый ключ сертификата для подписи документа
        public static void SetPublicKey(X509Certificate2 certificate)
        {
            //cspParams = new CspParameters() { ProviderType = 1 };
            //rsaProvider = new RSACryptoServiceProvider(cspParams);
            rsa_public = certificate.GetRSAPublicKey();
            if (rsa_public == null) throw new Exception();
            //rsaProvider.ImportCspBlob(rsa);
        }

        // Возвращает хеш-значение SHA1 (для ключа)
        public static byte[] SHAHash(byte[] data)
        {
            SHA1Managed sha1 = new SHA1Managed();
            return sha1.ComputeHash(data);
        }

        // Возвращает электронную подпись документа
        public static byte[] GetDocSignature(byte[] docContent)
        {
            byte[] docHash = SHAHash(docContent);
            return rsa_private.SignHash(docHash, HashAlgorithmName.SHA1, RSASignaturePadding.Pss);
            //return rsaProvider.SignHash(docHash, "SHA1");
        }

        // Проверка электронной подписи документа
        public static bool CheckDocSignature(byte[] Signature, byte[] docContent)
        {
            byte[] docHash = SHAHash(docContent);
            return rsa_public.VerifyHash(docHash, Signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pss);
            //CspParameters p = new CspParameters() { };
            //p.ProviderType = 1;
                //RSACryptoServiceProvider rsaCSP = new RSACryptoServiceProvider(p);
                //rsaCSP.ImportCspBlob(key);
                //return rsaCSP.VerifyHash(docHash, "SHA1", Signature);
        }
    }
}
