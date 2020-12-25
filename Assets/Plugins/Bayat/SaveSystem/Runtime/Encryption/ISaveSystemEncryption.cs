using Bayat.SaveSystem.Storage;
using System;
using System.Security.Cryptography;

namespace Bayat.SaveSystem.Security
{

    /// <summary>
    /// Save System Encryption interface.
    /// </summary>
    public interface ISaveSystemEncryption
    {

        /// <summary>
        /// Gets the cryptographic stream by using the given stream for writting encrypted data. (Encrypting)
        /// </summary>
        /// <param name="stream">The raw stream</param>
        /// <param name="password">The encryption password</param>
        /// <returns>A cryptographic stream wrapper which has an underlying <see cref="CryptoStream"/> for writting encrypted data</returns>
        SaveSystemCryptoStream GetWriteStream(IStorageStream stream, string password);

        /// <summary>
        /// Gets the cryptographic stream by using the given stream for reading encrypted data. (Decrypting)
        /// </summary>
        /// <param name="stream">The raw stream</param>
        /// <param name="password">The decryption password</param>
        /// <returns>A cryptographic stream wrapper which has an underlying <see cref="CryptoStream"/> for reading encrypted data</returns>
        SaveSystemCryptoStream GetReadStream(IStorageStream stream, string password);

    }

    /// <summary>
    /// Save System cryptographic stream wrapper.
    /// </summary>
    public abstract class SaveSystemCryptoStream : IDisposable
    {

        /// <summary>
        /// The underlying <see cref="CryptoStream"/>.
        /// </summary>
        public readonly CryptoStream UnderlyingCryptoStream;

        public SaveSystemCryptoStream(CryptoStream cryptoStream)
        {
            this.UnderlyingCryptoStream = cryptoStream;
        }

        public virtual void Dispose()
        {
            this.UnderlyingCryptoStream.Dispose();
        }

    }

}