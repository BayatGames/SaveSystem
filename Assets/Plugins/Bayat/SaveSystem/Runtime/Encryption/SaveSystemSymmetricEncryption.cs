using Bayat.SaveSystem.Storage;
using System.Security.Cryptography;

namespace Bayat.SaveSystem.Security
{

    /// <summary>
    /// Save System symmetric algorithm encryption implementation.
    /// </summary>
    public class SaveSystemSymmetricEncryption : ISaveSystemEncryption
    {

        /// <summary>
        /// The default symmetric algorithm name.
        /// </summary>
        public const string DefaultAlgorithmName = "aes";

        private const int ivSize = 16;
        private const int keySize = 16;
        private const int pwIterations = 100;

        public readonly string AlgorithmName = DefaultAlgorithmName;

        public SaveSystemSymmetricEncryption() : this(DefaultAlgorithmName)
        {

        }

        public SaveSystemSymmetricEncryption(string algorithmName)
        {
            if (string.IsNullOrEmpty(algorithmName))
            {
                this.AlgorithmName = DefaultAlgorithmName;
            }
            else
            {
                this.AlgorithmName = algorithmName;
            }
        }

        /// <summary>
        /// Gets the cryptographic stream by using the given stream for writting encrypted data. (Encrypting)
        /// </summary>
        /// <param name="stream">The raw stream</param>
        /// <param name="password">The encryption password</param>
        /// <returns>A cryptographic stream wrapper which has an underlying <see cref="CryptoStream"/> for writting encrypted data</returns>
        public virtual SaveSystemCryptoStream GetWriteStream(IStorageStream stream, string password)
        {
            var alg = SymmetricAlgorithm.Create(this.AlgorithmName);
            alg.Mode = CipherMode.CBC;
            alg.Padding = PaddingMode.PKCS7;
            alg.GenerateIV();
            var key = new Rfc2898DeriveBytes(password, alg.IV, pwIterations);
            alg.Key = key.GetBytes(keySize);

            // Write the IV to the stream
            stream.UnderlyingStream.Write(alg.IV, 0, ivSize);
            var encryptor = alg.CreateEncryptor();
            return new SaveSystemSymmetricCryptoStream(new CryptoStream(stream.UnderlyingStream, encryptor, CryptoStreamMode.Write), alg, encryptor);
        }

        /// <summary>
        /// Gets the cryptographic stream by using the given stream for reading encrypted data. (Decrypting)
        /// </summary>
        /// <param name="stream">The raw stream</param>
        /// <param name="password">The decryption password</param>
        /// <returns>A cryptographic stream wrapper which has an underlying <see cref="CryptoStream"/> for reading encrypted data</returns>
        public virtual SaveSystemCryptoStream GetReadStream(IStorageStream stream, string password)
        {
            var alg = SymmetricAlgorithm.Create(this.AlgorithmName);
            alg.Mode = CipherMode.CBC;
            alg.Padding = PaddingMode.PKCS7;
            var thisIV = new byte[ivSize];

            // Read the IV from the stream
            stream.UnderlyingStream.Read(thisIV, 0, ivSize);
            alg.IV = thisIV;

            var key = new Rfc2898DeriveBytes(password, alg.IV, pwIterations);
            alg.Key = key.GetBytes(keySize);

            var decryptor = alg.CreateDecryptor();
            return new SaveSystemSymmetricCryptoStream(new CryptoStream(stream.UnderlyingStream, decryptor, CryptoStreamMode.Read), alg, decryptor);
        }

    }

    /// <summary>
    /// Save System symmetric encryption crypto stream wrapper.
    /// </summary>
    public class SaveSystemSymmetricCryptoStream : SaveSystemCryptoStream
    {

        /// <summary>
        /// The encryption algorithm.
        /// </summary>
        public readonly SymmetricAlgorithm Algorithm;

        /// <summary>
        /// The crypto transform, whether is encryptor or decryptor.
        /// </summary>
        public readonly ICryptoTransform CryptoTransform;

        public SaveSystemSymmetricCryptoStream(CryptoStream cryptoStream, SymmetricAlgorithm algorithm, ICryptoTransform cryptoTransform) : base(cryptoStream)
        {
            this.Algorithm = algorithm;
            this.CryptoTransform = cryptoTransform;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.Algorithm.Dispose();
            this.CryptoTransform.Dispose();
        }

    }

}