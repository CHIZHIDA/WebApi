using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/*
 *     RSA 算法 操作规则
 *     1.为保证跨语言平台通用,RSA密钥格式必须为PCKS1
 *     2.加密或签名之后的base64中可能包含空格或加号,如果作为http请求参数,请url做encode
 *     3.加密解密和签名验签一定不要使用同一组密钥对
 *     4.为保证传输数据安全,通讯的双方请各自生成一组密钥对,私钥各自保留,彼此公布公钥
 *     5.加密之前请一定先签名
 *     
 *     示例:
 *     A为请求方,B为接收方
 *     
 *     以下是请求方的操作:
 *     假设A传送内容为mark
 *     1.组装报文,如:RSA_mark    使用固定的消息头方便对方知道解密成功
 *     2.用A的私钥对 RSA_mark 签名,假设签名结果为:XJ9B5D1
 *     3.把签名结果z组装在原报文末尾:RSA_mark_XJ9B5D1
 *     4.用B的公钥对 RSA_mark_XJ9B5D1 加密,结果假设为: NE03WBEN12=
 *     5.将 NE03WBEN12= 发送给B
 *     
 *     以下是接收方的操作:
 *     1.接到密文 NE03WBEN12= 
 *     2.使用B的私钥解密获得:RSA_mark_XJ9B5D1
 *     3.判断报文消息头是否为 RSA,以检验A是否是用B的公钥进行加密
 *     4.解密成功后,截取签名的末尾 XJ9B5D1
 *     5.用A的公钥对消息体验签,待验证的消息体 RSA_mark ,签名值 NE03WBEN12=
 *     6.若成功验签,贼说明该消息来自A
 */

namespace Token.Methods
{
    public class ServerEncryptionHelper
    {
        private const int RsaKeySize = 1024;        //要使用的密钥的大小（以位为单位）
        private const string publicKeyFileName = "ServerRSA.Pub";         //公钥
        private const string privateKeyFileName = "ServerRSA.Private";    //私钥
        private static string basePathToStoreKeys = ConfigurationManager.AppSettings["basePathToStoreServerKeys"];//Directory.GetCurrentDirectory();   //当前程序目录

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

        //用公钥加密文本框内容
        public static string PubKeyEncryption(string enStr)
        {
            var encryptedString = Encrypt(enStr, Path.Combine(basePathToStoreKeys, publicKeyFileName));

            return encryptedString;
        }

        //用私钥解密文本框内容
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
                    //加载公钥读取xml
                    string publicXmlKey = File.ReadAllText(pathToPublicKey);
                    rsa.FromXmlString(publicXmlKey);

                    byte[] bytesToEncrypt = Encoding.Unicode.GetBytes(plainText);

                    //分段加密 
                    //1024位的证书，加密时最大支持117个字节，解密时为128；2048位的证书，加密时最大支持245个字节，解密时为256。
                    int keySize = rsa.KeySize / 8;
                    int bufferSize = keySize - 11;
                    byte[] buffer = new byte[bufferSize];

                    //内存流,为系统内存提供读写操作
                    MemoryStream msInput = new MemoryStream(bytesToEncrypt);
                    MemoryStream msOuput = new MemoryStream();
                    int readLen = msInput.Read(buffer, 0, bufferSize);

                    while (readLen > 0)
                    {
                        byte[] dataToEnc = new byte[readLen];
                        Array.Copy(buffer, 0, dataToEnc, 0, readLen);
                        //加密  使用从缓冲区读取的数据将字节块写入当前流
                        byte[] encData = rsa.Encrypt(dataToEnc, false);
                        msOuput.Write(encData, 0, encData.Length);
                        readLen = msInput.Read(buffer, 0, bufferSize);
                    }

                    msInput.Close();
                    byte[] result = msOuput.ToArray();    //得到加密结果
                    msOuput.Close();
                    //var bytesEncrypted = rsa.Encrypt(bytesToEncrypt, false);

                    return Convert.ToBase64String(result);
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

                    //分段解密
                    //1024位的证书，加密时最大支持117个字节，解密时为128；2048位的证书，加密时最大支持245个字节，解密时为256。
                    int keySize = rsa.KeySize / 8;
                    byte[] buffer = new byte[keySize];

                    //内存流,为系统内存提供读写操作
                    MemoryStream msInput = new MemoryStream(bytesEncrypted);
                    MemoryStream msOuput = new MemoryStream();
                    int readLen = msInput.Read(buffer, 0, keySize);

                    while (readLen > 0)
                    {
                        byte[] dataToDec = new byte[readLen];
                        Array.Copy(buffer, 0, dataToDec, 0, readLen);
                        //解密    使用从缓冲区读取的数据将字节块写入当前流
                        byte[] encData = rsa.Decrypt(dataToDec, false);
                        msOuput.Write(encData, 0, encData.Length);
                        readLen = msInput.Read(buffer, 0, keySize);
                    }

                    //关闭内存流
                    msInput.Close();
                    byte[] result = msOuput.ToArray();    //得到解密结果
                    msOuput.Close();

                    return System.Text.Encoding.Unicode.GetString(result);
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
        /// 验签
        /// </summary>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static bool CheckSign(string message)
        {
            //截取第一个下划线'_'前的文本为消息头，最后一个下划线'_'后的文本为签名
            string[] list = message.Split('_');
            string messageHeader = list[0];
            //待验签的数据
            string buffer = list[list.Length - 1];

            //查看消息头是否正确
            if (messageHeader != ConfigurationManager.AppSettings["messageHeader"])
            {
                return false;
            }

            //文本截取签名（含下划线'_'）后，是已签名的数据
            string signature = message.Substring(0, message.Length - buffer.Length - 1);
            byte[] hashByteSignature = Encoding.Unicode.GetBytes(signature);

            //加载发送方的公钥进行验签
            var rsa = new RSACryptoServiceProvider();
            var publicXmlKey = File.ReadAllText(Path.Combine(ConfigurationManager.AppSettings["basePathToStoreClientKeys"], "ClientRSA.Pub"));
            rsa.FromXmlString(publicXmlKey);
            
            //MD5 mD5 = new MD5CryptoServiceProvider();
            //rsa.VerifyData(hashByteSignature, mD5, Convert.FromBase64String(buffer));
            //rsa.VerifyData(hashByteSignature, CryptoConfig.MapNameToOID("MD5"), Convert.FromBase64String(buffer));

            //哈希算法：SHA1(160bit)、SHA256(256bit)、MD5(128bit)
            return rsa.VerifyData(hashByteSignature, CryptoConfig.MapNameToOID("SHA1"), Convert.FromBase64String(buffer));
        }
    }
}