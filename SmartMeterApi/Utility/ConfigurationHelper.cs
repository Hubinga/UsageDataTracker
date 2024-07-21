namespace SmartMeterApi.Utility
{
    /// <summary>
    /// Helper class to manage and retrieve configuration settings for JWT secret key.
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Retrieves the JWT secret key.
        /// </summary>
        /// <returns>The JWT secret key as a byte[].</returns>
        /// <exception cref="InvalidOperationException">Thrown if the JWT secret key is not configured properly.</exception>
        public static byte[] GetJwtSecretKey()
        {
            string? envKey = Environment.GetEnvironmentVariable("SMG_JWT_KEY");
            if (!string.IsNullOrEmpty(envKey))
            {
                return Convert.FromHexString(envKey); // Assuming the key is stored as a hex string
            }

            // If no key is found, throw an exception
            throw new InvalidOperationException("JWT Secret Key is not configured as environment variable: SMG_JWT_KEY.");
        }
    }
}
