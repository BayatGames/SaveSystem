using Bayat.Json;
using Bayat.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Bayat.SaveSystem.Storage
{

    /// <summary>
    /// The storage interface, inherit <see cref="StorageBase"/> for implementing your own storage.
    /// </summary>
    public interface IStorage
    {

        /// <summary>
        /// The storage text encoding.
        /// </summary>
        Encoding TextEncoding { get; set; }

        /// <summary>
        /// Gets or sets whether to use meta data or not.
        /// </summary>
        bool UseMetaData { get; set; }

        /// <summary>
        /// Gets or sets whether to use catalog or not.
        /// </summary>
        bool UseCatalog { get; set; }

        /// <summary>
        /// The backup item identifier suffix of this storage.
        /// </summary>
        string BackupSuffix { get; }

        /// <summary>
        /// The meta item identifier suffix of this storage.
        /// </summary>
        string MetaSuffix { get; }

        /// <summary>
        /// The catalog identifier of this storage.
        /// </summary>
        string CatalogIdentifier { get; }

        /// <summary>
        /// Retrieves a writable stream from the storage.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <returns>A writable storage stream wrapper</returns>
        Task<IStorageStream> GetWriteStream(string identifier);

        /// <summary>
        /// Commits the writable stream changes.
        /// </summary>
        /// <param name="stream">The stream to commit its changes</param>
        Task CommitWriteStream(IStorageStream stream);

        /// <summary>
        /// Retrieves a readable stream from the storage.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <returns>A readable storage stream wrapper</returns>
        Task<IStorageStream> GetReadStream(string identifier);

        /// <summary>
        /// Writes all the text data.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <param name="data">The text data</param>
        Task WriteAllText(string identifier, string data);

        /// <summary>
        /// Reads all the text data.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <returns>The text data</returns>
        Task<string> ReadAllText(string identifier);

        /// <summary>
        /// Writes all the binary data.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <param name="data">The binary data</param>
        Task WriteAllBytes(string identifier, byte[] data);

        /// <summary>
        /// Reads all the binary data.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <returns>The binary data</returns>
        Task<byte[]> ReadAllBytes(string identifier);

        /// <summary>
        /// Updates the storage item meta data.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <param name="isWrite">Specifies whether it is a write/read process</param>
        /// <param name="encrypted">Whether the item is encrypted or not</param>
        /// <returns></returns>
        Task UpdateMetaData(string identifier, bool isWrite, bool encrypted);

        /// <summary>
        /// Saves the meta data for the specified item.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <param name="metaData">The meta data</param>
        Task SaveMetaData(string identifier, StorageMetaData metaData);

        /// <summary>
        /// Loads the meta data for the specified item.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <returns>The saved storage meta data if exists, otherwise returns a new instance of <see cref="StorageMetaData"/></returns>
        Task<StorageMetaData> LoadMetaData(string identifier);

        /// <summary>
        /// Deletes the specified item meta data.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>True if operation was successful otherwise false</returns>
        Task<StorageDeleteOperationResult> DeleteMetaData(string identifier);

        /// <summary>
        /// Checks whether the specified item has meta data or not.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <returns>True if the specified item has meta data otherwise false</returns>
        Task<bool> HasMetaData(string identifier);

        /// <summary>
        /// Saves the catalog.
        /// </summary>
        /// <param name="catalog">The catalog list</param>
        Task SaveCatalog(List<string> catalog);

        /// <summary>
        /// Loads the catalog.
        /// </summary>
        /// <returns>A list of items available in the catalog</returns>
        Task<List<string>> LoadCatalog();

        /// <summary>
        /// Clears all items in the storage, this method will delete *all* items in the storage so use it at your own risk.
        /// </summary>
        /// <returns>True if operation was successful otherwise false</returns>
        Task<StorageClearOperationResult> Clear();

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <returns>True if operation was successful otherwise false</returns>
        Task<StorageDeleteOperationResult> Delete(string identifier);

        /// <summary>
        /// Checks whether the specified item exists or not.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <returns>True if item exists otherwise false</returns>
        Task<bool> Exists(string identifier);

        /// <summary>
        /// Moves the specified item to the new location.
        /// </summary>
        /// <param name="oldIdentifier">The item source location<</param>
        /// <param name="newIdentifier">The itme destination location</param>
        /// <param name="replace">Replace the item at the destination or not if exists</param>
        /// <returns>True if operation was successful otherwise false</returns>
        Task<StorageMoveOperationResult> Move(string oldIdentifier, string newIdentifier, bool replace);

        /// <summary>
        /// Copies the specified item to the new location.
        /// </summary>
        /// <param name="fromIdentifier">The item source location</param>
        /// <param name="toIdentifier">The itme destination location</param>
        /// <param name="replace">Replace the item at the destination or not if exists</param>
        /// <returns>True if operation was successful otherwise false</returns>
        Task<StorageCopyOperationResult> Copy(string fromIdentifier, string toIdentifier, bool replace);

        /// <summary>
        /// Lists items available at the specified location.
        /// </summary>
        /// <param name="identifier">The location to look for items</param>
        /// <param name="options">The list options</param>
        /// <returns>An array of items found at the specified location</returns>
        Task<string[]> List(string identifier, StorageListOptions options);

        /// <summary>
        /// Lists all available items.
        /// </summary>
        /// <returns>An array of all items found</returns>
        Task<string[]> ListAll();

        /// <summary>
        /// Creates a new backup for the storage item.
        /// </summary>
        /// <returns>The created backup information <see cref="StorageBackup"/></returns>
        Task<StorageBackup> CreateBackup(string identifier);

        /// <summary>
        /// Gets the latest backup information.
        /// </summary>
        /// <param name="identifier">The storage item identifier to get latest backup from</param>
        /// <returns>The latest backup information</returns>
        Task<StorageBackup> GetLatestBackup(string identifier);

        /// <summary>
        /// Restores the latest backup for the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier to restore backup</param>
        /// <returns>True if operation was successful otherwise false</returns>
        Task<bool> RestoreLatestBackup(string identifier);

        /// <summary>
        /// Restores the backup for the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier to restore backup</param>
        /// <param name="backup">The backup information</param>
        /// <returns>True if operation was successful otherwise false</returns>
        Task<bool> RestoreBackup(string identifier, StorageBackup backup);

        /// <summary>
        /// Gets the storage item backups.
        /// </summary>
        /// <param name="identifier">The storage item identifier to get backups from</param>
        /// <returns>The storage item backups</returns>
        Task<List<StorageBackup>> GetBackups(string identifier);

        /// <summary>
        /// Deletes the specified backup for the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="backup">The backup information</param>
        /// <returns>True if operation was successful otherwise false</returns>
        Task<StorageDeleteOperationResult> DeleteBackup(string identifier, StorageBackup backup);

        /// <summary>
        /// Deletes all the backups for the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <returns>True if operation was successful otherwise false</returns>
        Task<bool> DeleteBackups(string identifier);

    }

    /// <summary>
    /// Storage stream wrapper.
    /// </summary>
    public abstract class StorageStream : IStorageStream
    {

        protected readonly string identifier;
        protected readonly Stream underlyingStream;

        /// <summary>
        /// The item identifier.
        /// </summary>
        public virtual string Identifier
        {
            get
            {
                return this.identifier;
            }
        }

        /// <summary>
        /// The underlying stream.
        /// </summary>
        public virtual Stream UnderlyingStream
        {
            get
            {
                return this.underlyingStream;
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="StorageStream"/>
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <param name="stream">The underlying stream</param>
        public StorageStream(string identifier, Stream stream)
        {
            this.identifier = identifier;
            this.underlyingStream = stream;
        }

        /// <summary>
        /// Disposes the underlying stream.
        /// </summary>
        public virtual void Dispose()
        {
            if (this.UnderlyingStream != null)
            {
                this.UnderlyingStream.Dispose();
            }
        }

    }

    /// <summary>
    /// Storage stream wrapper interface.
    /// </summary>
    public interface IStorageStream : IDisposable
    {

        /// <summary>
        /// The item identifier.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// The underlying stream.
        /// </summary>
        Stream UnderlyingStream { get; }

    }

    /// <summary>
    /// Storage item meta data.
    /// </summary>
    public class StorageMetaData
    {

        /// <summary>
        /// The internal collection of meta data.
        /// </summary>
        public readonly Hashtable Data = new Hashtable();

        /// <summary>
        /// Gets or sets the specified meta data item.
        /// </summary>
        /// <param name="key">The meta data item key</param>
        /// <returns>The value of the meta data item</returns>
        public object this[string key]
        {
            get
            {
                return this.Data[key];
            }
            set
            {
                this.Data[key] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="StorageMetaData"/>
        /// </summary>
        public StorageMetaData()
        {
            this.Data = new Hashtable(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="StorageMetaData"/> with the provided predefined data.
        /// </summary>
        /// <param name="data">The predefined data</param>
        public StorageMetaData(Hashtable data)
        {
            this.Data = new Hashtable(data, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="StorageMetaData"/> with the provided predefined data.
        /// </summary>
        /// <param name="data">The predefined data</param>
        public StorageMetaData(IDictionary data)
        {
            this.Data = new Hashtable(data, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the specified meta data item and casts its value to the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The meta data item key</param>
        /// <returns>The value of the meta data item</returns>
        public T Get<T>(string key)
        {
            if (!this.Data.ContainsKey(key))
            {
                return default(T);
            }
            object value = this.Data[key];
            try
            {
                return (T)value;
            }
            catch (InvalidCastException ex)
            {
                if (value is JToken)
                {
                    return (value as JToken).ToObject<T>();
                }
                throw ex;
            }
        }

        /// <summary>
        /// Checkes whether the specified meta data item exists or not.
        /// </summary>
        /// <param name="key">The meta data item key</param>
        /// <returns>True if meta data item exists and false if not</returns>
        public bool Has(string key)
        {
            return this.Data.ContainsKey(key);
        }

    }

    /// <summary>
    /// Storage item backup information.
    /// </summary>
    public class StorageBackup
    {

        [JsonProperty]
        public readonly string Identifier;
        [JsonProperty]
        public readonly DateTime BackupTimeUtc;

        [JsonConstructor]
        public StorageBackup(string identifier, DateTime backupTimeUtc)
        {
            this.Identifier = identifier;
            this.BackupTimeUtc = backupTimeUtc;
        }

    }

}