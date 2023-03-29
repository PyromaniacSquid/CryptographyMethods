using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ESignature
{
    internal class CryptoProvider
    {
        private static CspParameters cspParams;
        private static RSACryptoServiceProvider rsaProvider;
        private static DSACryptoServiceProvider dsaProvider;

        // Проверка на наличие контейнера с заданным именем
        public static bool CspContainerExists(string containerName)
        {
            var cspParams = new CspParameters
            {
                Flags = CspProviderFlags.UseExistingKey,
                KeyContainerName = containerName
            };

            try
            {
                var provider = new RSACryptoServiceProvider(cspParams);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        
        // Выбор контейнера
        public static void SetExistingCspContainer(string cName)
        {
            if (!CspContainerExists(cName))
                throw new InvalidOperationException("Контейнер заданного пользователя не существует");
            cspParams = new CspParameters()
            {
                KeyContainerName = cName,
            };
            cspParams.ProviderType = 1;
            rsaProvider = new RSACryptoServiceProvider(cspParams);
            cspParams.ProviderType = 13;
            dsaProvider = new DSACryptoServiceProvider(cspParams);
        }
        // Создание контейнера
        public static void CreateCspContainer(string name)
        {
            if (CspContainerExists(name)) throw new InvalidOperationException("Контейнер уже существует");
            CspParameters p = new CspParameters()
            {
                KeyContainerName = name,
            };
            p.ProviderType = 1;
            var q = new RSACryptoServiceProvider(p);
        }

        // Возвращает открытый ключ текущего контейнера
        public static byte[] GetCurrentPublicKey()
        {
            return rsaProvider.ExportCspBlob(false);
        }

        // Удаление ключей
        public static void RemoveKeysByName(string cName)
        {
            if (!CspContainerExists(cName)) return;
            CspParameters p = new CspParameters()
            {
                KeyContainerName = cName,
                Flags = CspProviderFlags.UseExistingKey,
            };
            var q = new RSACryptoServiceProvider(p);
            q.PersistKeyInCsp = false;
            q.Clear();
        }


        // Возвращает хеш-значение RIPEMD (для документа)
        public static byte[] RIPEMDHash(byte[] data)
        {
            RIPEMD160Managed ripemd = new RIPEMD160Managed();
            return ripemd.ComputeHash(data);
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
            byte[] docHash = RIPEMDHash(docContent);
            return rsaProvider.SignHash(docHash, "SHA1");
        }

        // Возвращает электронную подпись ключа
        public static byte[] GetKeySignature(byte[] key)
        {
            byte[] keyHash = SHAHash(key);
            return dsaProvider.SignHash(keyHash, "SHA1");
        }

        // Проверка электронной подписи открытого ключа автора
        public static bool CheckKeySignature(byte[] Signature, byte[] Blob)
        {
            //if (Signature == null) return false;
            byte[] hash = SHAHash(Blob);
            return dsaProvider.VerifyHash(hash, "SHA1", Signature);
        }

        // Проверка электронной подписи документа
        public static bool CheckDocSignature(byte[] Signature, byte[] docContent, byte[] key)
        {
            byte[] docHash = RIPEMDHash(docContent);
            CspParameters p = new CspParameters() { };
            p.ProviderType = 1;
                RSACryptoServiceProvider rsaCSP = new RSACryptoServiceProvider(p);
                rsaCSP.ImportCspBlob(key);
                return rsaCSP.VerifyHash(docHash, "SHA1", Signature);
        }
    }
}
