using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Bayat.SaveSystem.Storage
{

    /// <summary>
    /// PlayerPrefs storage implementation.
    /// </summary>
    public class PlayerPrefsStorage : StorageBase
    {

        protected bool useBase64 = true;

        public PlayerPrefsStorage(string encodingName, bool useBase64) : base(encodingName)
        {
            this.useBase64 = useBase64;
        }

        public override Task<IStorageStream> GetWriteStream(string identifier)
        {
            PlayerPrefsStorageStream storageStream = new PlayerPrefsStorageStream(identifier, new MemoryStream());
            return Task.FromResult<IStorageStream>(storageStream);
        }

        protected override Task CommitWriteStreamInternal(IStorageStream stream)
        {
            PlayerPrefsStorageStream memoryStream = (PlayerPrefsStorageStream)stream;
            string data = null;
            if (useBase64)
            {
                data = Convert.ToBase64String(((MemoryStream)memoryStream.UnderlyingStream).ToArray());
            }
            else
            {
                data = this.TextEncoding.GetString(((MemoryStream)memoryStream.UnderlyingStream).ToArray());
            }
            PlayerPrefs.SetString(memoryStream.Identifier, data);
            return Task.CompletedTask;
        }

        public override Task<IStorageStream> GetReadStream(string identifier)
        {
            MemoryStream memoryStream = null;
            if (useBase64)
            {
                memoryStream = new MemoryStream(Convert.FromBase64String(PlayerPrefs.GetString(identifier)));
            }
            else
            {
                memoryStream = new MemoryStream(this.TextEncoding.GetBytes(PlayerPrefs.GetString(identifier)));
            }
            PlayerPrefsStorageStream storageStream = new PlayerPrefsStorageStream(identifier, memoryStream);
            return Task.FromResult<IStorageStream>(storageStream);
        }

        protected override Task WriteAllTextInternal(string identifier, string data)
        {
            PlayerPrefs.SetString(identifier, data);
            return Task.CompletedTask;
        }

        public override Task<string> ReadAllText(string identifier)
        {
            return Task.FromResult(PlayerPrefs.GetString(identifier));
        }

        protected override Task WriteAllBytesInternal(string identifier, byte[] data)
        {
            PlayerPrefs.SetString(identifier, Convert.ToBase64String(data));
            return Task.CompletedTask;
        }

        public override Task<byte[]> ReadAllBytes(string identifier)
        {
            return Task.FromResult(Convert.FromBase64String(PlayerPrefs.GetString(identifier)));
        }

        public override async Task<StorageClearOperationResult> Clear()
        {
            bool result = true;
            List<string> catalog = await LoadCatalog();
            foreach (string identifier in catalog)
            {
                result &= (await Delete(identifier)).Succeed;
            }
            return new StorageClearOperationResult(result, catalog.ToArray());
        }

        protected override Task<StorageDeleteOperationResult> DeleteInternal(string identifier)
        {
            bool result = false;
            if (PlayerPrefs.HasKey(identifier))
            {
                PlayerPrefs.DeleteKey(identifier);
                result = true;
            }
            return Task.FromResult(new StorageDeleteOperationResult(result));
        }

        public override Task<bool> Exists(string identifier)
        {
            return Task.FromResult(PlayerPrefs.HasKey(identifier));
        }

        protected override async Task<StorageMoveOperationResult> MoveInternal(string oldIdentifier, string newIdentifier, bool replace)
        {
            bool result = false;
            if (PlayerPrefs.HasKey(oldIdentifier))
            {
                if (replace || !PlayerPrefs.HasKey(newIdentifier))
                {
                    PlayerPrefs.SetString(newIdentifier, PlayerPrefs.GetString(oldIdentifier));
                    await Delete(oldIdentifier);
                    result = true;
                }
            }
            return new StorageMoveOperationResult(result, newIdentifier);
        }

        protected override Task<StorageCopyOperationResult> CopyInternal(string fromIdentifier, string toIdentifier, bool replace)
        {
            bool result = false;
            if (PlayerPrefs.HasKey(fromIdentifier))
            {
                if (replace || !PlayerPrefs.HasKey(toIdentifier))
                {
                    PlayerPrefs.SetString(toIdentifier, PlayerPrefs.GetString(fromIdentifier));
                    result = true;
                }
            }
            return Task.FromResult(new StorageCopyOperationResult(result, toIdentifier));
        }

        public override async Task<string[]> List(string identifier, StorageListOptions options)
        {
            List<string> items = await LoadCatalog();
            if (options.MaxResults.HasValue)
            {
                items.Capacity = options.MaxResults.GetValueOrDefault();
            }
            return items.FindAll(item => item.Contains(identifier)).ToArray();
        }

        public override async Task<string[]> ListAll()
        {
            return (await LoadCatalog()).ToArray();
        }

    }

    /// <summary>
    /// The PlayerPrefs storage stream wrapper.
    /// </summary>
    public class PlayerPrefsStorageStream : StorageStream
    {

        public PlayerPrefsStorageStream(string identifier, Stream stream) : base(identifier, stream)
        {
        }

    }

}