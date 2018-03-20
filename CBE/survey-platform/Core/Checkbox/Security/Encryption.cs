using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Checkbox.Security
{
	/// <summary>
	/// Summary description for Encryption.
	/// </summary>
	public class Encryption
	{
		private Encryption()
		{
		}

		/// <summary>
		/// Hash a plaintext string using the MD5 hash algorithm
		/// </summary>
		/// <param name="plaintext">A plaintext string to encrypt</param>
		/// <returns>An encrypted string</returns>
		public static string HashOldString(string plaintext)
		{
			var sha1 = new MD5CryptoServiceProvider();
		    var hashedArray = sha1.ComputeHash(Encoding.ASCII.GetBytes(plaintext));
			var hashedString = Encoding.ASCII.GetString(hashedArray);
			return hashedString;
		}

        /// <summary>
        ///  Hash a plaintext string using the SHA 512 algorithim
        /// </summary>
        /// <param name="plaintext">A plaintext string to encrypt</param>
        /// <param name="salt">Salt to apply to the encryption</param>
        /// <returns>An encrtyped string</returns>
        public static string HashString(string plaintext, string salt)
        {
            var sha512 = new SHA512CryptoServiceProvider();
            var splitSalt = salt.Split('-');
            var hashedArray = sha512.ComputeHash(Encoding.ASCII.GetBytes(splitSalt[0] + plaintext + salt));
            var hashedString = Encoding.ASCII.GetString(hashedArray);
            return hashedString;
        }

        /// <summary>
        /// Hash a plaintext string using the MD5 hash algorithm.
        /// The way that ACII encoded text is handled has changed in the 2.0 framework. This method emulates
        /// the behavior exhibited in the 1.0 and 1.1 frameworks. New passwords should NEVER be encrypted with
        /// this method. Its only intended to provided backwards compatibility with passwords which where 
        /// hashed using the 1.1 framework.
        /// </summary>
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public static string HashStringDotNetOneFormat(string plaintext)
        {
            var sha1 = new MD5CryptoServiceProvider();
            var hashedArray = sha1.ComputeHash(Encoding.ASCII.GetBytes(plaintext));

            //The way in which ASCIIEncoding.ASCII.GetString(string s) handles ASCII that is not in the
            //predefined 128 characters is not the same in the 2.0 framework as it was in the 1.1 framework.
            //In 1.1 the most significant bit was dropped from none standard ASCII. In 2.0 none standard
            //ASCII is replaced with a question mark. This method emulates the behavior exhibited in
            //the 1.1 framework.
            var fixedByteArray = new byte[sha1.Hash.Length];
            for (var i = 0; i < sha1.Hash.Length; i++)
            {
                fixedByteArray[i] = (byte)(sha1.Hash[i] & 127);
            }
            
            var hashedString = Encoding.ASCII.GetString(fixedByteArray);
            return hashedString;    
        }

        
	}
}
