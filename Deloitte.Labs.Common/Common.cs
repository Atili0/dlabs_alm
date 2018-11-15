namespace Deloitte.Labs
{
    using System.IO.Compression;
    using System.Security.Cryptography;
    using System.Text;

    using System;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using Microsoft.Xrm.Sdk;

    public class Common
    {
        public enum ModeAsynPlugin {
            Asyn = 1,
            Sync = 0
        }

        public static string CreateFolder(string Entity)
        {
            var _completePath = String.Format("{0}\\Data\\{1}_{2}", System.Environment.CurrentDirectory, Entity,
                DateTime.UtcNow.ToFileTime());
            DirectoryInfo _info = System.IO.Directory.CreateDirectory(_completePath);
            return _info.FullName;
        }

        public static string CreateFolder(string Path, string Entity)
        {
            var _completePath = String.Format("{0}\\Data\\{1}_{2}", Path, Entity,
                DateTime.UtcNow.ToFileTime());
            DirectoryInfo _info = System.IO.Directory.CreateDirectory(_completePath);
            return _info.FullName;
        }

        public static String CreateFile(ObjectExporter pEntity)
        {
            string _filePath = String.Format("{0}\\{1}_{2}.dlbas", pEntity.FullPath, pEntity.Entity, pEntity.index);

            System.IO.File.WriteAllText(_filePath, pEntity.JsonSerializable, Encoding.UTF8);

            if (pEntity.Encrypt)
            {
                string _fileEnrypt = String.Format("{0}\\{1}_{2}_e.dlabs", pEntity.CompletePath, pEntity.Entity,
                    pEntity.index);
                new Common().EncryptFile(_filePath, _fileEnrypt, pEntity.KeyWord);
                System.IO.File.Delete(_filePath);
            }

            return _filePath;
        }

        public static string[] GetFileOnFolder(Super pSuper)
        {
            return Directory.GetFiles(pSuper.Path);
        }

        public void EncryptFile(string inputFile, string outputFile, string skey)
        {
            try
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    byte[] key = ASCIIEncoding.UTF8.GetBytes(skey);

                    /* This is for demostrating purposes only. 
                     * Ideally you will want the IV key to be different from your key and you should always generate a new one for each encryption in other to achieve maximum security*/
                    byte[] IV = ASCIIEncoding.UTF8.GetBytes(skey);

                    using (FileStream fsCrypt = new FileStream(outputFile, FileMode.Create))
                    {
                        using (ICryptoTransform encryptor = aes.CreateEncryptor(key, IV))
                        {
                            using (CryptoStream cs = new CryptoStream(fsCrypt, encryptor, CryptoStreamMode.Write))
                            {
                                using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                                {
                                    int data;
                                    while ((data = fsIn.ReadByte()) != -1)
                                    {
                                        cs.WriteByte((byte)data);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // failed to encrypt file
            }
        }

        public static void Compress(String pPath)
        {
            GZipStream outStream = new GZipStream(File.OpenWrite(pPath), CompressionMode.Compress);
        }

        public static void Decompress(ObjectExporter pEntity)
        {
            GZipStream instream = new GZipStream(File.OpenRead(pEntity.FullPath), CompressionMode.Decompress);
        }
    }

    public static class Utils
    {
        public static string ToJson(this object obj, bool useSimpleDictionaryFormat = true)
        {
            var jsonSerializer = new DataContractJsonSerializer(obj.GetType());
            using (var stream = new MemoryStream())
            {
                jsonSerializer.WriteObject(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public static OptionSetValue ToOptionSetValue(this int value)
        {
            return new OptionSetValue(value);
        }
    }
}
