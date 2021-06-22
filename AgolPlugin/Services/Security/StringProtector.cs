using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace AgolPlugin.Services.Security
{
    internal static class StringProtector
    {
        public static string EncryptToBase64String(SecureString secureString)
        {
            return EncryptToBase64String(SecureStringToString(secureString));
        }
        public static SecureString DecryptFromBase64StringToSecureString(string ciphertext)
        {
            return StringToSecureString(DecryptFromBase64String(ciphertext));
        }

        public static string EncryptToBase64String(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
                return string.Empty;

            var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            var ciphertextBytes = ProtectedData.Protect(plaintextBytes, null, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(ciphertextBytes);
        }
        public static string DecryptFromBase64String(string ciphertext)
        {
            if (string.IsNullOrEmpty(ciphertext))
                return string.Empty;

            var ciphertextBytes = Convert.FromBase64String(ciphertext);
            var plaintextBytes = ProtectedData.Unprotect(ciphertextBytes, null, DataProtectionScope.CurrentUser);

            return Encoding.UTF8.GetString(plaintextBytes);
        }

        public static string SecureStringToString(SecureString secureString)
        {
            if (secureString == null) return null;
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
        public static SecureString StringToSecureString(string str)
        {
            var secure = new SecureString();
            foreach (char c in str)
            {
                secure.AppendChar(c);
            }
            return secure;
        }
    }
}