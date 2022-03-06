using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using UnityEngine;

namespace Bayat.SaveSystem.Storage
{

    /// <summary>
    /// Memory storage implementation.
    /// </summary>
    public class MemoryStorage : StorageBase
    {

        protected static Dictionary<string, string> storedData = new Dictionary<string, string>();
        protected bool useBase64 = true;

        public static void PurgeAllStoredData()
        {
            storedData.Clear();
        }

        public MemoryStorage(string encodingName, bool useBase64) : base(encodingName)
        {
            this.useBase64 = useBase64;
        }

        public override Task<IStorageStream> GetWriteStream(string identifier)
        {
            var storageStream = new MemoryStorageStream(identifier, new MemoryStream());
            return Task.FromResult<IStorageStream>(storageStream);
        }

        protected override Task CommitWriteStreamInternal(IStorageStream stream)
        {
            var storageStream = (MemoryStorageStream)stream;
            var memoryStream = (MemoryStream)storageStream.UnderlyingStream;
            string data = null;
            if (useBase64)
            {
                data = Convert.ToBase64String((memoryStream).ToArray());
            }
            else
            {
                data = this.TextEncoding.GetString((memoryStream).ToArray());
            }
            storedData[storageStream.Identifier] = data;
            return Task.CompletedTask;
        }

        public override Task<IStorageStream> GetReadStream(string identifier)
        {
            MemoryStream memoryStream = null;
            string data = storedData[identifier];
            if (useBase64)
            {
                memoryStream = new MemoryStream(Convert.FromBase64String(data));
            }
            else
            {
                memoryStream = new MemoryStream(this.TextEncoding.GetBytes(data));
            }
            var storageStream = new MemoryStorageStream(identifier, memoryStream);
            return Task.FromResult<IStorageStream>(storageStream);
        }

        protected override Task WriteAllTextInternal(string identifier, string data)
        {
            storedData[identifier] = data;
            return Task.CompletedTask;
        }

        public override Task<string> ReadAllText(string identifier)
        {
            return Task.FromResult(storedData[identifier]);
        }

        protected override Task WriteAllBytesInternal(string identifier, byte[] data)
        {
            storedData[identifier] = Convert.ToBase64String(data);
            return Task.CompletedTask;
        }

        public override Task<byte[]> ReadAllBytes(string identifier)
        {
            return Task.FromResult(Convert.FromBase64String(storedData[identifier]));
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
            if (storedData.ContainsKey(identifier))
            {
                result = storedData.Remove(identifier);
            }
            return Task.FromResult(new StorageDeleteOperationResult(result));
        }

        public override Task<bool> Exists(string identifier)
        {
            return Task.FromResult(storedData.ContainsKey(identifier));
        }

        protected override async Task<StorageMoveOperationResult> MoveInternal(string oldIdentifier, string newIdentifier, bool replace)
        {
            bool result = false;
            if (storedData.ContainsKey(oldIdentifier))
            {
                if (replace || !storedData.ContainsKey(newIdentifier))
                {
                    storedData[newIdentifier] = storedData[oldIdentifier];
                    await Delete(oldIdentifier);
                    result = true;
                }
            }
            return new StorageMoveOperationResult(result, newIdentifier);
        }

        protected override Task<StorageCopyOperationResult> CopyInternal(string fromIdentifier, string toIdentifier, bool replace)
        {
            bool result = false;
            if (storedData.ContainsKey(fromIdentifier))
            {
                if (replace || !storedData.ContainsKey(toIdentifier))
                {
                    storedData[toIdentifier] = storedData[fromIdentifier];
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
    /// The Memory storage stream wrapper.
    /// </summary>
    public class MemoryStorageStream : StorageStream
    {

        public MemoryStorageStream(string identifier, Stream stream) : base(identifier, stream)
        {
        }

    }

}