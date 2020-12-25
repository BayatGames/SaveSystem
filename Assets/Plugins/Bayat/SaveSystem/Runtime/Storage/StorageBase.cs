using Bayat.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bayat.SaveSystem.Storage
{

    /// <summary>
    /// The base implementation of <see cref="IStorage"/>.
    /// </summary>
    public abstract class StorageBase : IStorage
    {

        public const string DefaultTextEncodingName = "utf-8";

        protected Encoding textEncoding;
        protected bool useMetaData = true;
        protected bool useCatalog = true;

        public virtual Encoding TextEncoding
        {
            get
            {
                return this.textEncoding ?? Encoding.UTF8;
            }
            set
            {
                this.textEncoding = value;
            }
        }

        public virtual bool UseMetaData
        {
            get
            {
                return this.useMetaData;
            }
            set
            {
                this.useMetaData = value;
            }
        }

        public virtual bool UseCatalog
        {
            get
            {
                return this.useCatalog;
            }
            set
            {
                this.useCatalog = value;
            }
        }

        public virtual string BackupSuffix
        {
            get
            {
                return ".backup";
            }
        }

        public virtual string MetaSuffix
        {
            get
            {
                return ".meta";
            }
        }

        public virtual string CatalogIdentifier
        {
            get
            {
                return ".catalog";
            }
        }

        public StorageBase() : this(Encoding.UTF8)
        {

        }

        public StorageBase(string encodingName) : this(Encoding.GetEncoding(encodingName))
        {

        }

        public StorageBase(Encoding textEncoding)
        {
            this.textEncoding = textEncoding ?? Encoding.UTF8;
        }

        #region Storage Interface Methods

        public abstract Task<IStorageStream> GetWriteStream(string identifier);

        public virtual async Task CommitWriteStream(IStorageStream stream)
        {
            await CommitWriteStreamInternal(stream);
            if (this.UseCatalog)
            {
                await AddToCatalog(stream.Identifier);
            }
        }

        /// <summary>
        /// Commit the write stream internally without doing any other actions.
        /// </summary>
        /// <param name="stream">The stream</param>
        protected abstract Task CommitWriteStreamInternal(IStorageStream stream);

        public abstract Task<IStorageStream> GetReadStream(string identifier);

        public virtual async Task WriteAllText(string identifier, string data)
        {
            await WriteAllTextInternal(identifier, data);
            if (this.UseCatalog)
            {
                await AddToCatalog(identifier);
            }
        }

        /// <summary>
        /// Write all the text data internally without doing any other actions.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <param name="data">The text data</param>
        protected abstract Task WriteAllTextInternal(string identifier, string data);

        public abstract Task<string> ReadAllText(string identifier);

        public virtual async Task WriteAllBytes(string identifier, byte[] data)
        {
            await WriteAllBytesInternal(identifier, data);
            if (this.UseCatalog)
            {
                await AddToCatalog(identifier);
            }
        }

        /// <summary>
        /// Write all binary data internally without doing any other actions.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <param name="data">The binary data</param>
        protected abstract Task WriteAllBytesInternal(string identifier, byte[] data);

        public abstract Task<byte[]> ReadAllBytes(string identifier);

        public abstract Task<StorageClearOperationResult> Clear();

        public virtual async Task<StorageDeleteOperationResult> Delete(string identifier)
        {
            StorageDeleteOperationResult result = await DeleteInternal(identifier);
            if (this.UseCatalog)
            {
                await RemoveFromCatalog(identifier);
            }

            // Delete meta file
            if (!IsMetaIdentifier(identifier))
            {
                await DeleteInternal(GetMetaIdentifier(identifier));
            }
            return result;
        }

        /// <summary>
        /// Delete the specified item internally without doing any other actions.
        /// </summary>
        /// <param name="identifier">The itme identifier</param>
        /// <returns>True if operation was successful otherwise false</returns>
        protected abstract Task<StorageDeleteOperationResult> DeleteInternal(string identifier);

        public abstract Task<bool> Exists(string identifier);

        public virtual async Task<StorageMoveOperationResult> Move(string oldIdentifier, string newIdentifier, bool replace)
        {
            StorageMoveOperationResult result = await MoveInternal(oldIdentifier, newIdentifier, replace);
            if (result.Succeed)
            {
                if (this.UseCatalog)
                {
                    await RenameInCatalog(oldIdentifier, result.ResultIdentifier);
                }

                // Move meta file
                if (!IsMetaIdentifier(oldIdentifier) && !IsMetaIdentifier(result.ResultIdentifier))
                {
                    await MoveInternal(GetMetaIdentifier(oldIdentifier), GetMetaIdentifier(result.ResultIdentifier), replace);
                }
            }
            return result;
        }

        /// <summary>
        /// Move the item internally without doing any other actions.
        /// </summary>
        /// <param name="oldIdentifier">The old item identifier</param>
        /// <param name="newIdentifier">The new item identifier</param>
        /// <param name="resultIdentifier">The item final result identifier after applying the operation</param>
        /// <param name="replace">Replace the target item if exists or not</param>
        /// <returns>True if operation was successful otherwise false</returns>
        protected abstract Task<StorageMoveOperationResult> MoveInternal(string oldIdentifier, string newIdentifier, bool replace);

        public virtual async Task<StorageCopyOperationResult> Copy(string fromIdentifier, string toIdentifier, bool replace)
        {
            StorageCopyOperationResult result = await CopyInternal(fromIdentifier, toIdentifier, replace);
            if (result.Succeed)
            {
                if (this.UseCatalog)
                {
                    await AddToCatalog(result.ResultIdentifier);
                }

                // Copy meta file
                if (!IsMetaIdentifier(fromIdentifier) && !IsMetaIdentifier(result.ResultIdentifier))
                {
                    await CopyInternal(GetMetaIdentifier(fromIdentifier), GetMetaIdentifier(result.ResultIdentifier), replace);
                }
            }
            return result;
        }

        /// <summary>
        /// Copy the item internally without doing any other actions.
        /// </summary>
        /// <param name="fromIdentifier">The item source identifier</param>
        /// <param name="toIdentifier">The item destination identifier</param>
        /// <param name="replace">Replace the target item if exists or not</param>
        /// <returns>True if operation was successful otherwise false</returns>
        protected abstract Task<StorageCopyOperationResult> CopyInternal(string fromIdentifier, string toIdentifier, bool replace);

        public abstract Task<string[]> List(string identifier, StorageListOptions options);

        public abstract Task<string[]> ListAll();

        #endregion

        #region Meta Data Methods

        public virtual async Task UpdateMetaData(string identifier, bool isWrite, bool encrypted)
        {
            if (!this.UseMetaData)
            {
                return;
            }
            StorageMetaData metaData = await LoadMetaData(identifier);
            if (isWrite)
            {
                DateTime creationTime = metaData.Get<DateTime>("CreationTimeUtc");
                if (creationTime == default(DateTime))
                {
                    metaData["CreationTimeUtc"] = DateTime.UtcNow;
                }
                metaData["LastModificationTimeUtc"] = DateTime.UtcNow;
                metaData["Encrypted"] = encrypted;
            }
            else
            {
                metaData["LastAccessTimeUtc"] = DateTime.UtcNow;
            }
            metaData["ApplicationVersion"] = Application.version;
            await SaveMetaData(identifier, metaData);
        }

        public virtual Task SaveMetaData(string identifier, StorageMetaData metaData)
        {
            string metaIdentifier = GetMetaIdentifier(identifier);
            return WriteAllTextInternal(metaIdentifier, JsonConvert.SerializeObject(metaData.Data));
        }

        public virtual async Task<StorageMetaData> LoadMetaData(string identifier)
        {
            string metaIdentifier = GetMetaIdentifier(identifier);
            if (!await Exists(metaIdentifier))
            {
                return new StorageMetaData();
            }
            string data = await ReadAllText(metaIdentifier);
            return new StorageMetaData(JsonConvert.DeserializeObject<Hashtable>(data));
        }

        public virtual async Task<StorageDeleteOperationResult> DeleteMetaData(string identifier)
        {
            string metaIdentifier = GetMetaIdentifier(identifier);
            if (await HasMetaData(metaIdentifier))
            {
                return await DeleteInternal(metaIdentifier);
            }
            return new StorageDeleteOperationResult(false);
        }

        public virtual Task<bool> HasMetaData(string identifier)
        {
            string metaIdentifier = GetMetaIdentifier(identifier);
            return Exists(metaIdentifier);
        }

        /// <summary>
        /// Gets the meta identifier using the specified item identifier or returns the identifier itself if it is a meta identifier already.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <returns>The meta identifier of the specified item identifier</returns>
        protected virtual string GetMetaIdentifier(string identifier)
        {
            if (IsMetaIdentifier(identifier))
            {
                return identifier;
            }
            return identifier + this.MetaSuffix;
        }

        /// <summary>
        /// Checks whether the specified identifier is meta or not.
        /// </summary>
        /// <param name="identifier">The identifier</param>
        /// <returns>True if the identifier is meta otherwise false</returns>
        protected virtual bool IsMetaIdentifier(string identifier)
        {
            return identifier.EndsWith(this.MetaSuffix);
        }

        #endregion

        #region Catalog Methods

        public virtual Task SaveCatalog(List<string> catalog)
        {
            return WriteAllTextInternal(this.CatalogIdentifier, JsonConvert.SerializeObject(catalog));
        }

        public virtual async Task<List<string>> LoadCatalog()
        {
            if (!await Exists(this.CatalogIdentifier))
            {
                return new List<string>();
            }
            string data = await ReadAllText(this.CatalogIdentifier);
            return JsonConvert.DeserializeObject<List<string>>(data);
        }

        /// <summary>
        /// Adds the new item to the catalog.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        protected virtual async Task AddToCatalog(string identifier)
        {
            if (identifier == this.CatalogIdentifier)
            {
                return;
            }
            if (IsMetaIdentifier(identifier))
            {
                return;
            }
            List<string> catalog = await LoadCatalog();
            if (catalog.Contains(identifier))
            {
                return;
            }
            catalog.Add(identifier);
            await SaveCatalog(catalog);
        }

        /// <summary>
        /// Removes the item from the catalog.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        protected virtual async Task RemoveFromCatalog(string identifier)
        {
            if (IsMetaIdentifier(identifier))
            {
                return;
            }
            List<string> catalog = await LoadCatalog();
            if (!catalog.Contains(identifier))
            {
                return;
            }
            catalog.Remove(identifier);
            await SaveCatalog(catalog);
        }

        /// <summary>
        /// Renames the item in the catalog.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        protected virtual async Task RenameInCatalog(string oldIdentifier, string newIdentifier)
        {
            if (oldIdentifier == this.CatalogIdentifier || newIdentifier == this.CatalogIdentifier)
            {
                return;
            }
            if (IsMetaIdentifier(oldIdentifier) || IsMetaIdentifier(newIdentifier))
            {
                return;
            }
            List<string> catalog = await LoadCatalog();
            if (catalog.Contains(oldIdentifier))
            {
                catalog[catalog.IndexOf(oldIdentifier)] = newIdentifier;
                await SaveCatalog(catalog);
            }
        }

        /// <summary>
        /// Checks whether the specified identifier is catalog identifier or not.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <returns>True if the identifier is catalog identifier otherwise false</returns>
        protected virtual bool IsCatalogIdentifier(string identifier)
        {
            return this.CatalogIdentifier == identifier;
        }

        #endregion

        #region Backup Methods

        public virtual async Task<StorageBackup> CreateBackup(string identifier)
        {
            DateTime backupTimeUtc = DateTime.UtcNow;
            string backupIdentifier = identifier + backupTimeUtc.Ticks + this.BackupSuffix;
            StorageBackup backup = new StorageBackup(backupIdentifier, backupTimeUtc);
            await Copy(identifier, backupIdentifier, false);

            // Update source item meta data after creating backup
            StorageMetaData metaData = await LoadMetaData(identifier);
            List<StorageBackup> backups = new List<StorageBackup>();
            if (metaData.Has("Backups"))
            {
                backups = metaData.Get<List<StorageBackup>>("Backups");
            }
            backups.Add(backup);
            metaData["Backups"] = backups;
            await SaveMetaData(identifier, metaData);

            return backup;
        }

        public virtual async Task<bool> RestoreLatestBackup(string identifier)
        {
            StorageBackup latestBackup = await GetLatestBackup(identifier);
            if (latestBackup != null)
            {
                return await RestoreBackup(identifier, latestBackup);
            }
            return false;
        }

        public virtual async Task<StorageBackup> GetLatestBackup(string identifier)
        {
            StorageMetaData metaData = await LoadMetaData(identifier);
            if (metaData.Has("Backups"))
            {
                List<StorageBackup> backups = metaData.Get<List<StorageBackup>>("Backups");
                StorageBackup latestBackup = null;
                foreach (StorageBackup backup in backups)
                {
                    if (latestBackup == null)
                    {
                        latestBackup = backup;
                    }
                    else if (backup.BackupTimeUtc > latestBackup.BackupTimeUtc)
                    {
                        if (await Exists(backup.Identifier))
                        {
                            latestBackup = backup;
                        }
                    }
                }
                return latestBackup;
            }
            return null;
        }

        public virtual async Task<bool> RestoreBackup(string identifier, StorageBackup backup)
        {
            if (backup == null)
            {
                throw new ArgumentNullException(nameof(backup));
            }
            if (!await Exists(backup.Identifier))
            {
                return false;
            }
            return (await Move(backup.Identifier, identifier, true)).Succeed;
        }

        public virtual async Task<List<StorageBackup>> GetBackups(string identifier)
        {
            StorageMetaData metaData = await LoadMetaData(identifier);
            if (metaData.Has("Backups"))
            {
                return metaData.Get<List<StorageBackup>>("Backups");
            }
            return null;
        }

        public virtual async Task<StorageDeleteOperationResult> DeleteBackup(string identifier, StorageBackup backup)
        {
            if (backup == null)
            {
                throw new ArgumentNullException(nameof(backup));
            }
            if (!await Exists(backup.Identifier))
            {
                return new StorageDeleteOperationResult(false);
            }
            StorageMetaData metaData = await LoadMetaData(identifier);
            if (metaData.Has("Backups"))
            {
                List<StorageBackup> backups = metaData.Get<List<StorageBackup>>("Backups");
                StorageBackup backupToRemove = backup;
                foreach (StorageBackup currentBackup in backups)
                {
                    if (currentBackup.Identifier == backup.Identifier)
                    {
                        backupToRemove = currentBackup;
                    }
                }
                backups.Remove(backupToRemove);
                metaData["Backups"] = backups;
                await SaveMetaData(identifier, metaData);
            }
            return await Delete(backup.Identifier);
        }

        public virtual async Task<bool> DeleteBackups(string identifier)
        {
            StorageMetaData metaData = await LoadMetaData(identifier);
            if (metaData.Has("Backups"))
            {
                bool allSucceed = true;
                List<StorageBackup> backups = metaData.Get<List<StorageBackup>>("Backups");
                foreach (StorageBackup backup in backups)
                {
                    if (await Exists(backup.Identifier))
                    {
                        allSucceed &= (await Delete(backup.Identifier)).Succeed;
                    }
                }
                backups.Clear();
                metaData["Backups"] = backups;
                await SaveMetaData(identifier, metaData);
                return allSucceed;
            }
            return false;
        }

        #endregion

    }

}