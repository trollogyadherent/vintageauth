using System;
using System.Security.Cryptography;
using System.Text;

namespace VintageAuth {
    public class UuidUtil {
        public static string uuidFromStr(string str) {
            using (MD5 mD = MD5.Create())
            {
                byte[] bytes = Encoding.ASCII.GetBytes(str);
                byte[] array = mD.ComputeHash(bytes);
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < array.Length; i++)
                {
                    stringBuilder.Append(array[i].ToString("X2"));
                }
                return stringBuilder.ToString();
            }
        }
    }
}