using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;

/*
 *   Sign签名规则：
 *  1、	根据非业务参数和业务参数拼接字符串并按照首字母排序
 *  注意：如果首字母相同，则按照第二个字母排序，一次类推
 *  如：RSA_13510103189_1540803537222_1_1540803537_10
 *    :	RSA_{mobile.value}_{sn.value}_{source.value}_{timestamp.value}_{ua.value}
 *     2、	使用对应平台的私钥进行签名的到sign
 */


namespace Token.Methods
{
    //1.公钥加密私钥解密，用于加解密
    //2.私钥加密公钥解密，用于签名
    /// <summary>
    /// 客户端密钥加解密
    /// </summary>
    public class ClientEncryptionHelper
    {
        private const int RsaKeySize = 1024;                        //要使用的密钥的大小（以位为单位）
        private const string publicKeyFileName = "ClientRSA.Pub";         //公钥文件名称
        private const string privateKeyFileName = "ClientRSA.Private";    //私钥文件名称
        private static string basePathToStoreKeys = ConfigurationManager.AppSettings["basePathToStoreClientKeys"];//Directory.GetCurrentDirectory();   //当前程序目录

        /// <summary>
        ///在给定路径中生成XML格式的私钥和公钥。
        /// </summary>
        public static string GenerateKeys()
        {
            string path = basePathToStoreKeys;

            using (var rsa = new RSACryptoServiceProvider(RsaKeySize))
            {
                try
                {
                    // 获取私钥和公钥。
                    var publicKey = rsa.ToXmlString(false);
                    var privateKey = rsa.ToXmlString(true);

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    bool result = false;
                    string resultMsg = "该路径已存在密钥对，生成失败";

                    // 保存到磁盘
                    if (!File.Exists(Path.Combine(path, publicKeyFileName)))
                    {
                        File.WriteAllText(Path.Combine(path, publicKeyFileName), publicKey);
                        result = true;
                    }
                    if (!File.Exists(Path.Combine(path, privateKeyFileName)))
                    {
                        File.WriteAllText(Path.Combine(path, privateKeyFileName), privateKey);
                        result = true;
                    }
                    if (result)
                    {
                        resultMsg = string.Format("生成的RSA密钥对的路径: {0}\\ [{1}, {2}]", path, publicKeyFileName, privateKeyFileName);
                    }

                    return resultMsg;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        //用公钥加密纯文本
        public static string PubKeyEncryption(string enStr)
        {
            var encryptedString = Encrypt(enStr.ToString(), Path.Combine(basePathToStoreKeys, publicKeyFileName));

            return encryptedString;
        }

        //用私钥加密纯文本
        public static string PriKeyEncrypted(string enStr)
        {
            var encryptedString = Encrypt(enStr, Path.Combine(basePathToStoreKeys, privateKeyFileName));

            return encryptedString;
        }

        //用私钥解密纯文本
        public static string PriKeyDecrypted(string deStr)
        {
            var decryptedString = Decrypt(deStr, Path.Combine(basePathToStoreKeys, privateKeyFileName));

            return decryptedString;
        }

        /// <summary>
        /// 用给定路径的RSA公钥文件加密纯文本。
        /// </summary>
        /// <param name="plainText">要加密的文本</param>
        /// <param name="pathToPublicKey">用于加密的公钥路径.</param>
        /// <returns>表示加密数据的64位编码字符串.</returns>
        private static string Encrypt(string plainText, string pathToPublicKey)
        {
            using (var rsa = new RSACryptoServiceProvider(RsaKeySize))
            {
                try
                {
                    //加载公钥
                    var publicXmlKey = File.ReadAllText(pathToPublicKey);
                    rsa.FromXmlString(publicXmlKey);

                    var bytesToEncrypt = System.Text.Encoding.Unicode.GetBytes(plainText);

                    var bytesEncrypted = rsa.Encrypt(bytesToEncrypt, false);

                    return Convert.ToBase64String(bytesEncrypted);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        /// <summary>
        /// Decrypts encrypted text given a RSA private key file path.给定路径的RSA私钥文件解密 加密文本
        /// </summary>
        /// <param name="encryptedText">加密的密文</param>
        /// <param name="pathToPrivateKey">用于解密的私钥路径.</param>
        /// <returns>未加密数据的字符串</returns>
        private static string Decrypt(string encryptedText, string pathToPrivateKey)
        {
            using (var rsa = new RSACryptoServiceProvider(RsaKeySize))
            {
                try
                {
                    //加载私钥
                    var privateXmlKey = File.ReadAllText(pathToPrivateKey);
                    rsa.FromXmlString(privateXmlKey);

                    var bytesEncrypted = Convert.FromBase64String(encryptedText);

                    var bytesPlainText = rsa.Decrypt(bytesEncrypted, false);

                    return System.Text.Encoding.Unicode.GetString(bytesPlainText);
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        /// <summary>
        /// 用私钥生成对文本签名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string privateToSign(string str)
        {
            //判空
            if(string.IsNullOrEmpty(str))
            {
                return null;
            }

            //要签名文本编码为base64字节
            byte[] hashByteSignture = System.Text.Encoding.Unicode.GetBytes(str);

            //加载私钥
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            var privateXmlKey = File.ReadAllText(Path.Combine(basePathToStoreKeys, privateKeyFileName));
            rsa.FromXmlString(privateXmlKey);

            //设置MD5签名
            //MD5 mD5 = new MD5CryptoServiceProvider();
            //byte[] sign = rsa.SignData(hashByteSignture, mD5);

            //哈希算法：SHA1(160bit)、SHA256(256bit)、MD5(128bit)
            byte[] sign = rsa.SignData(hashByteSignture, CryptoConfig.MapNameToOID("SHA1"));

            return Convert.ToBase64String(sign);
        }
    }
}