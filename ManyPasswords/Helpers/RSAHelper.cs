using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace 好多密码_UWP.Helpers
{
    public static class RSAHelper
    {
        public static string RSAEncrypt(string normaltxt)
        {
            byte[] bytes = Encoding.Default.GetBytes(normaltxt);
            byte[] encryptBytes = new RSACryptoServiceProvider(new CspParameters()).Encrypt(bytes, false);
            return Convert.ToBase64String(encryptBytes);
        }

        public static string RSADecrypt(string securityTxt)
        {
            try
            {
                var bytes = Convert.FromBase64String(securityTxt);
                byte[] decryptBytes = new RSACryptoServiceProvider(new CspParameters()).Decrypt(bytes, false);
                return Encoding.Default.GetString(decryptBytes);
            }
            catch (Exception)
            {
                return "";
            }
        }

    }
}
