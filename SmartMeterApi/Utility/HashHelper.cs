using System.Security.Cryptography;
using System.Text;

namespace SmartMeterApi.Utility
{
	public static class HashHelper
	{
        /*Sicherheitsprinzip:
          -	sicheres speichern der Passwörter
          - Passwörter werden nicht im Klartext gespeichert, sondern als Hash-Wert. 
		  - Der Salt ist ein zufällig generierter Wert, der dem Passwort hinzugefügt wird, bevor der Hash berechnet wird.
		  - Selbst wenn zwei Benutzer dasselbe Passwort wählen, führen unterschiedliche Salt-Werte zu unterschiedlichen Hashes. Dies erschwert es Angreifern, Rainbow-Table-Angriffe oder andere Vorverrechnungsangriffe durchzuführen.*/

        /// <summary>
        /// Creates a password hash using HMACSHA512 with a random salt.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="passwordHash">Output parameter for the generated password hash.</param>
        /// <param name="passwordSalt">Output parameter for the generated password salt.</param>
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512())
			{
				// Generate random salt
				passwordSalt = hmac.Key;

				// Compute hash for the password using the salt
				passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
			}
		}

		/// <summary>
		/// Verifies a password hash against the provided password and salt.
		/// </summary>
		/// <param name="password">The password to verify.</param>
		/// <param name="passwordSalt">The salt used to hash the original password.</param>
		/// <param name="storedHash">The stored password hash to compare against.</param>
		/// <returns>True if the password matches the stored hash, otherwise false.</returns>
		public static bool VerifyPasswordHash(string password, byte[] passwordSalt, byte[] storedHash)
		{
			using (var hmac = new HMACSHA512(passwordSalt))
			{
				// Compute hash for the entered password using the stored salt
				var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Compare computed hash with stored hash
                return storedHash.SequenceEqual(computedHash);
            }
		}
	}

}
