using Bayat.SaveSystem.Security;
using Bayat.SaveSystem.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Bayat.SaveSystem
{

    /// <summary>
    /// The Save System Unified Facade API class.
    /// </summary>
    public static class SaveSystemAPI
    {

        #region Events

        /// <summary>
        /// Fires when begun saving the data.
        /// </summary>
        public static event EventHandler<SaveEventArgs> Saving;

        /// <summary>
        /// Fires when saved the data.
        /// </summary>
        public static event EventHandler<SaveEventArgs> Saved;

        /// <summary>
        /// Fires when begun loading the data.
        /// </summary>
        public static event EventHandler<LoadEventArgs> Loading;

        /// <summary>
        /// Fires when loaded the data.
        /// </summary>
        public static event EventHandler<LoadEventArgs> Loaded;

        /// <summary>
        /// Fires when begun loading into the object.
        /// </summary>
        public static event EventHandler<LoadIntoEventArgs> LoadingInto;

        /// <summary>
        /// Fires when loaded into the object.
        /// </summary>
        public static event EventHandler<LoadIntoEventArgs> LoadedInto;

        #endregion

        #region Save Methods

        ///// <summary>
        ///// Serializes the value and saves the serialized data at the specified storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="value">The value</param>
        //public static void Save(string identifier, object value)
        //{
        //    SaveAsync(identifier, value, SaveSystemSettings.DefaultSettings).Wait();
        //}

        ///// <summary>
        ///// Serializes the value and saves the serialized data at the specified storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="value">The value</param>
        ///// <param name="settings">The settings</param>
        //public static void Save(string identifier, object value, SaveSystemSettings settings)
        //{
        //    SaveAsync(identifier, value, settings.Storage, settings.Serializer, settings.UseEncryption ? settings.EncryptionAlgorithm : null, settings.Password).Wait();
        //}

        ///// <summary>
        ///// Serializes the value and saves the serialized data at the specified storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="value">The value</param>
        ///// <param name="storage">The storage</param>
        ///// <param name="serializer">The serializer</param>
        //public static void Save(string identifier, object value, IStorage storage, SaveSystemJsonSerializer serializer, ISaveSystemEncryption encryption, string password)
        //{
        //    SaveAsync(identifier, value, storage, serializer, encryption, password).Wait();
        //}

        /// <summary>
        /// Serializes the value and saves the serialized data at the specified storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="value">The value</param>
        public static Task SaveAsync(string identifier, object value)
        {
            return SaveAsync(identifier, value, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Serializes the value and saves the serialized data at the specified storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="value">The value</param>
        /// <param name="settings">The settings</param>
        public static Task SaveAsync(string identifier, object value, SaveSystemSettings settings)
        {
            return SaveAsync(identifier, value, settings.Storage, settings.Serializer, settings.UseEncryption, settings.EncryptionAlgorithm, settings.Password);
        }

        /// <summary>
        /// Serializes the value and saves the serialized data at the specified storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="value">The value</param>
        /// <param name="storage">The storage</param>
        /// <param name="serializer">The serializer</param>
        public static async Task SaveAsync(string identifier, object value, IStorage storage, SaveSystemJsonSerializer serializer, bool useEncryption, ISaveSystemEncryption encryption, string password)
        {
            SaveEventArgs eventArgs = new SaveEventArgs(identifier, value, storage, serializer, encryption, password);
            Saving?.Invoke(null, eventArgs);
            using (IStorageStream stream = await storage.GetWriteStream(identifier))
            {
                Stream underlyingStream = stream.UnderlyingStream;
                SaveSystemCryptoStream cryptoStream = null;
                if (useEncryption)
                {
                    cryptoStream = encryption.GetWriteStream(stream, password);
                    underlyingStream = cryptoStream.UnderlyingCryptoStream;
                }
                serializer.Serialize(underlyingStream, value);
                if (cryptoStream != null)
                {
                    cryptoStream.Dispose();
                }
                await storage.CommitWriteStream(stream);
                await storage.UpdateMetaData(identifier, true, useEncryption);
            }
            Saved?.Invoke(null, eventArgs);
        }

        #endregion

        #region Load Methods

        ///// <summary>
        ///// Loads the serialized data from the storage item and deserializes the object using the default settings.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <returns>The deserialized object</returns>
        //public static T Load<T>(string identifier)
        //{
        //    return Load<T>(identifier, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Loads the serialized data from the storage item and deserializes the object.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>The deserialized object</returns>
        //public static T Load<T>(string identifier, SaveSystemSettings settings)
        //{
        //    return Load<T>(identifier, settings.Storage, settings.Serializer, settings.UseEncryption ? settings.EncryptionAlgorithm : null, settings.Password);
        //}

        ///// <summary>
        ///// Loads the serialized data from the storage item and deserializes the object.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="storage">The storage</param>
        ///// <param name="serializer">The serializer</param>
        ///// <returns>The deserialized object</returns>
        //public static T Load<T>(string identifier, IStorage storage, SaveSystemJsonSerializer serializer, ISaveSystemEncryption encryption, string password)
        //{
        //    Task<T> task = LoadAsync<T>(identifier, storage, serializer, encryption, password);
        //    task.Wait();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Loads the serialized data from the storage item and deserializes the object using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="objectType">The object type</param>
        ///// <returns>The deserialized object</returns>
        //public static object Load(string identifier, Type objectType)
        //{
        //    return Load(identifier, objectType, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Loads the serialized data from the storage item and deserializes the object.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="objectType">The object type</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>The deserialized object</returns>
        //public static object Load(string identifier, Type objectType, SaveSystemSettings settings)
        //{
        //    return Load(identifier, objectType, settings.Storage, settings.Serializer, settings.UseEncryption ? settings.EncryptionAlgorithm : null, settings.Password);
        //}

        ///// <summary>
        ///// Loads the serialized data from the storage item and deserializes the object.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="objectType">The object type</param>
        ///// <param name="storage">The storage</param>
        ///// <param name="serializer">The serializer</param>
        ///// <returns>The deserialized object</returns>
        //public static object Load(string identifier, Type objectType, IStorage storage, SaveSystemJsonSerializer serializer, ISaveSystemEncryption encryption, string password)
        //{
        //    Task<object> task = LoadAsync(identifier, objectType, storage, serializer, encryption, password);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Loads the serialized data from the storage item and deserializes the object using the default settings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifier">The storage item identifier</param>
        /// <returns>The deserialized object</returns>
        public static Task<T> LoadAsync<T>(string identifier)
        {
            return LoadAsync<T>(identifier, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Loads the serialized data from the storage item and deserializes the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="objectType">The object type</param>
        /// <param name="settings">The settings</param>
        /// <returns>The deserialized object</returns>
        public static Task<T> LoadAsync<T>(string identifier, SaveSystemSettings settings)
        {
            return LoadAsync<T>(identifier, settings.Storage, settings.Serializer, settings.UseEncryption, settings.EncryptionAlgorithm, settings.Password);
        }

        /// <summary>
        /// Loads the serialized data from the storage item and deserializes the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="objectType">The object type</param>
        /// <param name="storage">The storage</param>
        /// <param name="serializer">The serializer</param>
        /// <returns>The deserialized object</returns>
        public static async Task<T> LoadAsync<T>(string identifier, IStorage storage, SaveSystemJsonSerializer serializer, bool useEncryption, ISaveSystemEncryption encryption, string password)
        {
            object value = await LoadAsync(identifier, typeof(T), storage, serializer, useEncryption, encryption, password);
            if (value == null)
            {
                value = default(T);
            }
            return (T)value;
        }

        /// <summary>
        /// Loads the serialized data from the storage item and deserializes the object using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="objectType">The object type</param>
        /// <returns>The deserialized object</returns>
        public static Task<object> LoadAsync(string identifier, Type objectType)
        {
            return LoadAsync(identifier, objectType, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Loads the serialized data from the storage item and deserializes the object.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="objectType">The object type</param>
        /// <param name="settings">The settings</param>
        /// <returns>The deserialized object</returns>
        public static Task<object> LoadAsync(string identifier, Type objectType, SaveSystemSettings settings)
        {
            return LoadAsync(identifier, objectType, settings.Storage, settings.Serializer, settings.UseEncryption, settings.EncryptionAlgorithm, settings.Password);
        }

        /// <summary>
        /// Loads the serialized data from the storage item and deserializes the object.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="objectType">The object type</param>
        /// <param name="storage">The storage</param>
        /// <param name="serializer">The serializer</param>
        /// <returns>The deserialized object</returns>
        public static async Task<object> LoadAsync(string identifier, Type objectType, IStorage storage, SaveSystemJsonSerializer serializer, bool useEncryption, ISaveSystemEncryption encryption, string password)
        {
            LoadEventArgs eventArgs = new LoadEventArgs(identifier, objectType, storage, serializer, encryption, password);
            Loading?.Invoke(null, eventArgs);
            object value = null;
            using (IStorageStream stream = await storage.GetReadStream(identifier))
            {
                Stream underlyingStream = stream.UnderlyingStream;
                Stream normalStream = underlyingStream;
                SaveSystemCryptoStream cryptoStream = null;
                bool isEncrypted = useEncryption;
                if (await storage.HasMetaData(identifier))
                {
                    StorageMetaData metaData = await storage.LoadMetaData(identifier);
                    if (metaData.Has("Encrypted"))
                    {
                        isEncrypted = metaData.Get<bool>("Encrypted");
                    }
                }
                if (isEncrypted)
                {
                    cryptoStream = encryption.GetReadStream(stream, password);
                    underlyingStream = cryptoStream.UnderlyingCryptoStream;
                }
                if (isEncrypted)
                {
                    try
                    {
                        value = serializer.Deserialize(underlyingStream, objectType);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                        value = serializer.Deserialize(normalStream, objectType);
                    }
                }
                else
                {
                    value = serializer.Deserialize(normalStream, objectType);
                }
                if (cryptoStream != null)
                {
                    cryptoStream.Dispose();
                }
                await storage.UpdateMetaData(identifier, false, isEncrypted);
            }
            Loaded?.Invoke(null, eventArgs);
            return value;
        }

        #endregion

        #region LoadInto Methods

        ///// <summary>
        ///// Loads the serialized data from the storage item and deserializes it into (populates) the object using the default settings.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="target">The target object to load data into</param>
        ///// <returns>The populated target object</returns>
        //public static T LoadInto<T>(string identifier, object target)
        //{
        //    return LoadInto<T>(identifier, target, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Loads the serialized data from the storage item and deserializes it into (populates) the object.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="target">The target object to load data into</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>The populated target object</returns>
        //public static T LoadInto<T>(string identifier, object target, SaveSystemSettings settings)
        //{
        //    return LoadInto<T>(identifier, target, settings.Storage, settings.Serializer, settings.UseEncryption ? settings.EncryptionAlgorithm : null, settings.Password);
        //}

        ///// <summary>
        ///// Loads the serialized data from the storage item and deserializes it into (populates) the object.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="target">The target object to load data into</param>
        ///// <param name="storage">The storage</param>
        ///// <param name="serializer">The serializer</param>
        ///// <returns>The populated target object</returns>
        //public static T LoadInto<T>(string identifier, object target, IStorage storage, SaveSystemJsonSerializer serializer, ISaveSystemEncryption encryption, string password)
        //{
        //    Task<T> task = LoadIntoAsync<T>(identifier, target, storage, serializer, encryption, password);
        //    task.Wait();
        //    return task.Result;
        //}

        ///// <summary>
        ///// Loads the serialized data from the storage item and deserializes it into (populates) the object using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="target">The target object to load data into</param>
        ///// <returns>The populated target object</returns>
        //public static object LoadInto(string identifier, object target)
        //{
        //    return LoadInto(identifier, target, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Loads the serialized data from the storage item and deserializes it into (populates) the object.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="target">The target object to load data into</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>The populated target object</returns>
        //public static object LoadInto(string identifier, object target, SaveSystemSettings settings)
        //{
        //    return LoadInto(identifier, target, settings.Storage, settings.Serializer, settings.UseEncryption ? settings.EncryptionAlgorithm : null, settings.Password);
        //}

        ///// <summary>
        ///// Loads the serialized data from the storage item and deserializes it into (populates) the object.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="target">The target object to load data into</param>
        ///// <param name="storage">The storage</param>
        ///// <param name="serializer">The serializer</param>
        ///// <returns>The populated target object</returns>
        //public static object LoadInto(string identifier, object target, IStorage storage, SaveSystemJsonSerializer serializer, ISaveSystemEncryption encryption, string password)
        //{
        //    Task<object> task = LoadIntoAsync(identifier, target, storage, serializer, encryption, password);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Loads the serialized data from the storage item and deserializes it into (populates) the object using the default settings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="target">The target object to load data into</param>
        /// <returns>The populated target object</returns>
        public static Task<T> LoadIntoAsync<T>(string identifier, object target)
        {
            return LoadIntoAsync<T>(identifier, target, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Loads the serialized data from the storage item and deserializes it into (populates) the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="target">The target object to load data into</param>
        /// <param name="settings">The settings</param>
        /// <returns>The populated target object</returns>
        public static Task<T> LoadIntoAsync<T>(string identifier, object target, SaveSystemSettings settings)
        {
            return LoadIntoAsync<T>(identifier, target, settings.Storage, settings.Serializer, settings.UseEncryption, settings.EncryptionAlgorithm, settings.Password);
        }

        /// <summary>
        /// Loads the serialized data from the storage item and deserializes it into (populates) the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="target">The target object to load data into</param>
        /// <param name="storage">The storage</param>
        /// <param name="serializer">The serializer</param>
        /// <returns>The populated target object</returns>
        public static async Task<T> LoadIntoAsync<T>(string identifier, object target, IStorage storage, SaveSystemJsonSerializer serializer, bool useEncryption, ISaveSystemEncryption encryption, string password)
        {
            object value = await LoadIntoAsync(identifier, target, storage, serializer, useEncryption, encryption, password);
            if (value == null)
            {
                value = default(T);
            }
            return (T)value;
        }

        /// <summary>
        /// Loads the serialized data from the storage item and deserializes it into (populates) the object using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="target">The target object to load data into</param>
        /// <returns>The populated target object</returns>
        public static Task<object> LoadIntoAsync(string identifier, object target)
        {
            return LoadIntoAsync(identifier, target, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Loads the serialized data from the storage item and deserializes it into (populates) the object.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="target">The target object to load data into</param>
        /// <param name="settings">The settings</param>
        /// <returns>The populated target object</returns>
        public static Task<object> LoadIntoAsync(string identifier, object target, SaveSystemSettings settings)
        {
            return LoadIntoAsync(identifier, target, settings.Storage, settings.Serializer, settings.UseEncryption, settings.EncryptionAlgorithm, settings.Password);
        }

        /// <summary>
        /// Loads the serialized data from the storage item and deserializes it into (populates) the object.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="target">The target object to load data into</param>
        /// <param name="storage">The storage</param>
        /// <param name="serializer">The serializer</param>
        /// <returns>The populated target object</returns>
        public static async Task<object> LoadIntoAsync(string identifier, object target, IStorage storage, SaveSystemJsonSerializer serializer, bool useEncryption, ISaveSystemEncryption encryption, string password)
        {
            LoadIntoEventArgs eventArgs = new LoadIntoEventArgs(identifier, target, storage, serializer, encryption, password);
            LoadingInto?.Invoke(null, eventArgs);
            using (IStorageStream stream = await storage.GetReadStream(identifier))
            {
                Stream underlyingStream = stream.UnderlyingStream;
                Stream normalStream = underlyingStream;
                SaveSystemCryptoStream cryptoStream = null;
                bool isEncrypted = useEncryption;
                if (await storage.HasMetaData(identifier))
                {
                    StorageMetaData metaData = await storage.LoadMetaData(identifier);
                    if (metaData.Has("Encrypted"))
                    {
                        isEncrypted = metaData.Get<bool>("Encrypted");
                    }
                }
                if (isEncrypted)
                {
                    try
                    {
                        target = serializer.DeserializeInto(underlyingStream, target);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                        target = serializer.DeserializeInto(normalStream, target);
                    }
                }
                else
                {
                    target = serializer.DeserializeInto(normalStream, target);
                }
                if (cryptoStream != null)
                {
                    cryptoStream.Dispose();
                }
                await storage.UpdateMetaData(identifier, false, encryption != null);
            }
            LoadedInto?.Invoke(null, eventArgs);
            return target;
        }

        #endregion

        #region Exists Methods

        ///// <summary>
        ///// Checks whether the storage item exists or not using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <returns>True if storage item exists otherwise false</returns>
        //public static bool Exists(string identifier)
        //{
        //    return Exists(identifier, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Checks whether the storage item exists or not.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>True if storage item exists otherwise false</returns>
        //public static bool Exists(string identifier, SaveSystemSettings settings)
        //{
        //    return Exists(identifier, settings.Storage);
        //}

        ///// <summary>
        ///// Checks whether the storage item exists or not.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="storage">The storage</param>
        ///// <returns>True if storage item exists otherwise false</returns>
        //public static bool Exists(string identifier, IStorage storage)
        //{
        //    Task<bool> task = ExistsAsync(identifier, storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Checks whether the storage item exists or not using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <returns>True if storage item exists otherwise false</returns>
        public static Task<bool> ExistsAsync(string identifier)
        {
            return ExistsAsync(identifier, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Checks whether the storage item exists or not.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="settings">The settings</param>
        /// <returns>True if storage item exists otherwise false</returns>
        public static Task<bool> ExistsAsync(string identifier, SaveSystemSettings settings)
        {
            return ExistsAsync(identifier, settings.Storage);
        }

        /// <summary>
        /// Checks whether the storage item exists or not.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="storage">The storage</param>
        /// <returns>True if storage item exists otherwise false</returns>
        public static Task<bool> ExistsAsync(string identifier, IStorage storage)
        {
            return storage.Exists(identifier);
        }

        #endregion

        #region Delete Methods

        ///// <summary>
        ///// Deletes the storage item using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static StorageDeleteOperationResult Delete(string identifier)
        //{
        //    return Delete(identifier, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Deletes the storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static StorageDeleteOperationResult Delete(string identifier, SaveSystemSettings settings)
        //{
        //    return Delete(identifier, settings.Storage);
        //}

        ///// <summary>
        ///// Deletes the storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="storage">The storage</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static StorageDeleteOperationResult Delete(string identifier, IStorage storage)
        //{
        //    Task<StorageDeleteOperationResult> task = DeleteAsync(identifier, storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Deletes the storage item using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<StorageDeleteOperationResult> DeleteAsync(string identifier)
        {
            return DeleteAsync(identifier, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Deletes the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="settings">The settings</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<StorageDeleteOperationResult> DeleteAsync(string identifier, SaveSystemSettings settings)
        {
            return DeleteAsync(identifier, settings.Storage);
        }

        /// <summary>
        /// Deletes the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="storage">The storage</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<StorageDeleteOperationResult> DeleteAsync(string identifier, IStorage storage)
        {
            return storage.Delete(identifier);
        }

        #endregion

        #region Move Methods

        ///// <summary>
        ///// Moves the storage item to a new location using the default settings.
        ///// </summary>
        ///// <param name="fromIdentifier">The storage item source location</param>
        ///// <param name="toIdentifier">The storage item destination location</param>
        ///// <param name="replace">Replace the destination item if exists</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static StorageMoveOperationResult Move(string fromIdentifier, string toIdentifier, bool replace)
        //{
        //    return Move(fromIdentifier, toIdentifier, replace, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Moves the storage item to a new location.
        ///// </summary>
        ///// <param name="fromIdentifier">The storage item source location</param>
        ///// <param name="toIdentifier">The storage item destination location</param>
        ///// <param name="replace">Replace the destination item if exists</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static StorageMoveOperationResult Move(string fromIdentifier, string toIdentifier, bool replace, SaveSystemSettings settings)
        //{
        //    return Move(fromIdentifier, toIdentifier, replace, settings.Storage);
        //}

        ///// <summary>
        ///// Moves the storage item to a new location.
        ///// </summary>
        ///// <param name="fromIdentifier">The storage item source location</param>
        ///// <param name="toIdentifier">The storage item destination location</param>
        ///// <param name="replace">Replace the destination item if exists</param>
        ///// <param name="storage">The storage</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static StorageMoveOperationResult Move(string fromIdentifier, string toIdentifier, bool replace, IStorage storage)
        //{
        //    Task<StorageMoveOperationResult> task = MoveAsync(fromIdentifier, toIdentifier, replace, storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Moves the storage item to a new location using the default settings.
        /// </summary>
        /// <param name="fromIdentifier">The storage item source location</param>
        /// <param name="toIdentifier">The storage item destination location</param>
        /// <param name="replace">Replace the destination item if exists</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<StorageMoveOperationResult> MoveAsync(string fromIdentifier, string toIdentifier, bool replace)
        {
            return MoveAsync(fromIdentifier, toIdentifier, replace, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Moves the storage item to a new location.
        /// </summary>
        /// <param name="fromIdentifier">The storage item source location</param>
        /// <param name="toIdentifier">The storage item destination location</param>
        /// <param name="replace">Replace the destination item if exists</param>
        /// <param name="settings">The settings</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<StorageMoveOperationResult> MoveAsync(string fromIdentifier, string toIdentifier, bool replace, SaveSystemSettings settings)
        {
            return MoveAsync(fromIdentifier, toIdentifier, replace, settings.Storage);
        }

        /// <summary>
        /// Moves the storage item to a new location.
        /// </summary>
        /// <param name="fromIdentifier">The storage item source location</param>
        /// <param name="toIdentifier">The storage item destination location</param>
        /// <param name="replace">Replace the destination item if exists</param>
        /// <param name="storage">The storage</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<StorageMoveOperationResult> MoveAsync(string fromIdentifier, string toIdentifier, bool replace, IStorage storage)
        {
            return storage.Move(fromIdentifier, toIdentifier, replace);
        }

        #endregion

        #region Copy Methods

        ///// <summary>
        ///// Copies the storage item to a new location using the default settings.
        ///// </summary>
        ///// <param name="fromIdentifier">The storage item source location</param>
        ///// <param name="toIdentifier">The storage item destination location</param>
        ///// <param name="replace">Replace the destination item if exists</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static StorageCopyOperationResult Copy(string fromIdentifier, string toIdentifier, bool replace)
        //{
        //    return Copy(fromIdentifier, toIdentifier, replace, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Copies the storage item to a new location.
        ///// </summary>
        ///// <param name="fromIdentifier">The storage item source location</param>
        ///// <param name="toIdentifier">The storage item destination location</param>
        ///// <param name="replace">Replace the destination item if exists</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static StorageCopyOperationResult Copy(string fromIdentifier, string toIdentifier, bool replace, SaveSystemSettings settings)
        //{
        //    return Copy(fromIdentifier, toIdentifier, replace, settings.Storage);
        //}

        ///// <summary>
        ///// Copies the storage item to a new location.
        ///// </summary>
        ///// <param name="fromIdentifier">The storage item source location</param>
        ///// <param name="toIdentifier">The storage item destination location</param>
        ///// <param name="replace">Replace the destination item if exists</param>
        ///// <param name="storage">The storage</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static StorageCopyOperationResult Copy(string fromIdentifier, string toIdentifier, bool replace, IStorage storage)
        //{
        //    Task<StorageCopyOperationResult> task = CopyAsync(fromIdentifier, toIdentifier, replace, storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Copies the storage item to a new location using the default settings.
        /// </summary>
        /// <param name="fromIdentifier">The storage item source location</param>
        /// <param name="toIdentifier">The storage item destination location</param>
        /// <param name="replace">Replace the destination item if exists</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<StorageCopyOperationResult> CopyAsync(string fromIdentifier, string toIdentifier, bool replace)
        {
            return CopyAsync(fromIdentifier, toIdentifier, replace, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Copies the storage item to a new location.
        /// </summary>
        /// <param name="fromIdentifier">The storage item source location</param>
        /// <param name="toIdentifier">The storage item destination location</param>
        /// <param name="replace">Replace the destination item if exists</param>
        /// <param name="settings">The settings</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<StorageCopyOperationResult> CopyAsync(string fromIdentifier, string toIdentifier, bool replace, SaveSystemSettings settings)
        {
            return CopyAsync(fromIdentifier, toIdentifier, replace, settings.Storage);
        }

        /// <summary>
        /// Copies the storage item to a new location.
        /// </summary>
        /// <param name="fromIdentifier">The storage item source location</param>
        /// <param name="toIdentifier">The storage item destination location</param>
        /// <param name="replace">Replace the destination item if exists</param>
        /// <param name="storage">The storage</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<StorageCopyOperationResult> CopyAsync(string fromIdentifier, string toIdentifier, bool replace, IStorage storage)
        {
            return storage.Copy(fromIdentifier, toIdentifier, replace);
        }

        #endregion

        #region List Methods

        ///// <summary>
        ///// Lists storage items at the specified location using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage location</param>
        ///// <param name="listOptions">The listing options</param>
        ///// <returns>An array of items available at the location</returns>
        //public static string[] List(string identifier, StorageListOptions listOptions)
        //{
        //    return List(identifier, listOptions, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Lists storage items at the specified location.
        ///// </summary>
        ///// <param name="identifier">The storage location</param>
        ///// <param name="listOptions">The listing options</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>An array of items available at the location</returns>
        //public static string[] List(string identifier, StorageListOptions listOptions, SaveSystemSettings settings)
        //{
        //    return List(identifier, listOptions, settings.Storage);
        //}

        ///// <summary>
        ///// Lists storage items at the specified location.
        ///// </summary>
        ///// <param name="identifier">The storage location</param>
        ///// <param name="listOptions">The listing options</param>
        ///// <param name="storage">The storage</param>
        ///// <returns>An array of items available at the location</returns>
        //public static string[] List(string identifier, StorageListOptions listOptions, IStorage storage)
        //{
        //    Task<string[]> task = ListAsync(identifier, listOptions, storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Lists storage items at the specified location using the default settings.
        /// </summary>
        /// <param name="identifier">The storage location</param>
        /// <param name="listOptions">The listing options</param>
        /// <returns>An array of items available at the location</returns>
        public static Task<string[]> ListAsync(string identifier, StorageListOptions listOptions)
        {
            return ListAsync(identifier, listOptions, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Lists storage items at the specified location.
        /// </summary>
        /// <param name="identifier">The storage location</param>
        /// <param name="listOptions">The listing options</param>
        /// <param name="settings">The settings</param>
        /// <returns>An array of items available at the location</returns>
        public static Task<string[]> ListAsync(string identifier, StorageListOptions listOptions, SaveSystemSettings settings)
        {
            return ListAsync(identifier, listOptions, settings.Storage);
        }

        /// <summary>
        /// Lists storage items at the specified location.
        /// </summary>
        /// <param name="identifier">The storage location</param>
        /// <param name="listOptions">The listing options</param>
        /// <param name="storage">The storage</param>
        /// <returns>An array of items available at the location</returns>
        public static Task<string[]> ListAsync(string identifier, StorageListOptions listOptions, IStorage storage)
        {
            return storage.List(identifier, listOptions);
        }

        #endregion

        #region ListAll Methods

        ///// <summary>
        ///// Lists all items in the storage.
        ///// </summary>
        ///// <returns>An array of items available in storage</returns>
        //public static string[] ListAll()
        //{
        //    return ListAll(SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Lists all items in the storage.
        ///// </summary>
        ///// <param name="settings">The settings</param>
        ///// <returns>An array of items available in storage</returns>
        //public static string[] ListAll(SaveSystemSettings settings)
        //{
        //    return ListAll(settings.Storage);
        //}

        ///// <summary>
        ///// Lists all items in the storage.
        ///// </summary>
        ///// <param name="storage">The storage</param>
        ///// <returns>An array of items available in storage</returns>
        //public static string[] ListAll(IStorage storage)
        //{
        //    Task<string[]> task = ListAllAsync(storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Lists all items in the storage.
        /// </summary>
        /// <returns>An array of items available in storage</returns>
        public static Task<string[]> ListAllAsync()
        {
            return ListAllAsync(SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Lists all items in the storage.
        /// </summary>
        /// <param name="settings">The settings</param>
        /// <returns>An array of items available in storage</returns>
        public static Task<string[]> ListAllAsync(SaveSystemSettings settings)
        {
            return ListAllAsync(settings.Storage);
        }

        /// <summary>
        /// Lists all items in the storage.
        /// </summary>
        /// <param name="storage">The storage</param>
        /// <returns>An array of items available in storage</returns>
        public static Task<string[]> ListAllAsync(IStorage storage)
        {
            return storage.ListAll();
        }

        #endregion

        #region Clear Methods

        ///// <summary>
        ///// Clears items available in the storage using the default settings.
        ///// </summary>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static StorageClearOperationResult Clear()
        //{
        //    return Clear(SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Clears items available in the storage.
        ///// </summary>
        ///// <param name="settings">The settings</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static StorageClearOperationResult Clear(SaveSystemSettings settings)
        //{
        //    return Clear(settings.Storage);
        //}

        ///// <summary>
        ///// Clears items available in the storage.
        ///// </summary>
        ///// <param name="storage">The storage</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static StorageClearOperationResult Clear(IStorage storage)
        //{
        //    Task<bool> task = ClearAsync(storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Clears items available in the storage using the default settings.
        /// </summary>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<StorageClearOperationResult> ClearAsync()
        {
            return ClearAsync(SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Clears items available in the storage.
        /// </summary>
        /// <param name="settings">The settings</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<StorageClearOperationResult> ClearAsync(SaveSystemSettings settings)
        {
            return ClearAsync(settings.Storage);
        }

        /// <summary>
        /// Clears items available in the storage.
        /// </summary>
        /// <param name="storage">The storage</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<StorageClearOperationResult> ClearAsync(IStorage storage)
        {
            return storage.Clear();
        }

        #endregion

        #region Save Catalog Methods

        ///// <summary>
        ///// Saves the storage catalog using the default settings.
        ///// </summary>
        ///// <param name="catalog">The catalog list</param>
        //public static void SaveCatalog(List<string> catalog)
        //{
        //    SaveCatalog(catalog, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Saves the storage catalog.
        ///// </summary>
        ///// <param name="catalog">The catalog list</param>
        ///// <param name="settings">The settings</param>
        //public static void SaveCatalog(List<string> catalog, SaveSystemSettings settings)
        //{
        //    SaveCatalog(catalog, settings.Storage);
        //}

        ///// <summary>
        ///// Saves the storage catalog.
        ///// </summary>
        ///// <param name="catalog">The catalog list</param>
        ///// <param name="storage">The storage</param>
        //public static void SaveCatalog(List<string> catalog, IStorage storage)
        //{
        //    Task task = SaveCatalogAsync(catalog, storage);
        //    task.Wait();
        //}

        /// <summary>
        /// Saves the storage catalog using the default settings.
        /// </summary>
        /// <param name="catalog">The catalog list</param>
        public static Task SaveCatalogAsync(List<string> catalog)
        {
            return SaveCatalogAsync(catalog, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Saves the storage catalog.
        /// </summary>
        /// <param name="catalog">The catalog list</param>
        /// <param name="settings">The settings</param>
        public static Task SaveCatalogAsync(List<string> catalog, SaveSystemSettings settings)
        {
            return SaveCatalogAsync(catalog, settings.Storage);
        }

        /// <summary>
        /// Saves the storage catalog.
        /// </summary>
        /// <param name="catalog">The catalog list</param>
        /// <param name="storage">The storage</param>
        public static Task SaveCatalogAsync(List<string> catalog, IStorage storage)
        {
            return storage.SaveCatalog(catalog);
        }

        #endregion

        #region Load Catalog Methods

        ///// <summary>
        ///// Loads the storage catalog using the default settings.
        ///// </summary>
        ///// <returns>The catalog list</returns>
        //public static List<string> LoadCatalog()
        //{
        //    return LoadCatalog(SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Loads the storage catalog.
        ///// </summary>
        ///// <param name="settings">The settings</param>
        ///// <returns>The catalog list</returns>
        //public static List<string> LoadCatalog(SaveSystemSettings settings)
        //{
        //    return LoadCatalog(settings.Storage);
        //}

        ///// <summary>
        ///// Loads the storage catalog.
        ///// </summary>
        ///// <param name="storage">The storage</param>
        ///// <returns>The catalog list</returns>
        //public static List<string> LoadCatalog(IStorage storage)
        //{
        //    Task<List<string>> task = LoadCatalogAsync(storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Loads the storage catalog using the default settings.
        /// </summary>
        /// <returns>The catalog list</returns>
        public static Task<List<string>> LoadCatalogAsync()
        {
            return LoadCatalogAsync(SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Loads the storage catalog.
        /// </summary>
        /// <param name="settings">The settings</param>
        /// <returns>The catalog list</returns>
        public static Task<List<string>> LoadCatalogAsync(SaveSystemSettings settings)
        {
            return LoadCatalogAsync(settings.Storage);
        }

        /// <summary>
        /// Loads the storage catalog.
        /// </summary>
        /// <param name="storage">The storage</param>
        /// <returns>The catalog list</returns>
        public static Task<List<string>> LoadCatalogAsync(IStorage storage)
        {
            return storage.LoadCatalog();
        }

        #endregion

        #region Save Meta Data Methods

        ///// <summary>
        ///// Saves the storage item meta data using the default settings.
        ///// </summary>
        ///// <param name="catalog">The catalog list</param>
        //public static void SaveMetaData(string identifier, StorageMetaData metaData)
        //{
        //    SaveMetaData(identifier, metaData, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Saves the storage item meta data.
        ///// </summary>
        ///// <param name="catalog">The catalog list</param>
        ///// <param name="settings">The settings</param>
        //public static void SaveMetaData(string identifier, StorageMetaData metaData, SaveSystemSettings settings)
        //{
        //    SaveMetaData(identifier, metaData, settings.Storage);
        //}

        ///// <summary>
        ///// Saves the storage item meta data.
        ///// </summary>
        ///// <param name="catalog">The catalog list</param>
        ///// <param name="storage">The storage</param>
        //public static void SaveMetaData(string identifier, StorageMetaData metaData, IStorage storage)
        //{
        //    Task task = SaveMetaDataAsync(identifier, metaData, storage);
        //    task.Wait();
        //}

        /// <summary>
        /// Saves the storage item meta data using the default settings.
        /// </summary>
        /// <param name="catalog">The catalog list</param>
        public static Task SaveMetaDataAsync(string identifier, StorageMetaData metaData)
        {
            return SaveMetaDataAsync(identifier, metaData, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Saves the storage item meta data.
        /// </summary>
        /// <param name="catalog">The catalog list</param>
        /// <param name="settings">The settings</param>
        public static Task SaveMetaDataAsync(string identifier, StorageMetaData metaData, SaveSystemSettings settings)
        {
            return SaveMetaDataAsync(identifier, metaData, settings.Storage);
        }

        /// <summary>
        /// Saves the storage item meta data.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="metaData">The storage item meta data</param>
        /// <param name="storage">The storage</param>
        public static Task SaveMetaDataAsync(string identifier, StorageMetaData metaData, IStorage storage)
        {
            return storage.SaveMetaData(identifier, metaData);
        }

        #endregion

        #region Load Meta Data Methods

        ///// <summary>
        ///// Loads the storage item meta data using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <returns>The storage item meta data</returns>
        //public static StorageMetaData LoadMetaData(string identifier)
        //{
        //    return LoadMetaData(identifier, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Loads the storage item meta data.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>The storage item meta data</returns>
        //public static StorageMetaData LoadMetaData(string identifier, SaveSystemSettings settings)
        //{
        //    return LoadMetaData(identifier, settings.Storage);
        //}

        ///// <summary>
        ///// Loads the storage item meta data.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="storage">The storage</param>
        ///// <returns>The storage item meta data</returns>
        //public static StorageMetaData LoadMetaData(string identifier, IStorage storage)
        //{
        //    Task<StorageMetaData> task = LoadMetaDataAsync(identifier, storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Loads the storage item meta data using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <returns>The storage item meta data</returns>
        public static Task<StorageMetaData> LoadMetaDataAsync(string identifier)
        {
            return LoadMetaDataAsync(identifier, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Loads the storage item meta data.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="settings">The settings</param>
        /// <returns>The storage item meta data</returns>
        public static Task<StorageMetaData> LoadMetaDataAsync(string identifier, SaveSystemSettings settings)
        {
            return LoadMetaDataAsync(identifier, settings.Storage);
        }

        /// <summary>
        /// Loads the storage item meta data.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="storage">The storage</param>
        /// <returns>The storage item meta data</returns>
        public static Task<StorageMetaData> LoadMetaDataAsync(string identifier, IStorage storage)
        {
            return storage.LoadMetaData(identifier);
        }

        #endregion

        #region Has Meta Data Methods

        ///// <summary>
        ///// Checks whether the storage item meta data exists or not using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <returns>True if storage item meta data exists otherwise false</returns>
        //public static bool HasMetaData(string identifier)
        //{
        //    return HasMetaData(identifier, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Checks whether the storage item meta data exists or not.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>True if storage item meta data exists otherwise false</returns>
        //public static bool HasMetaData(string identifier, SaveSystemSettings settings)
        //{
        //    return HasMetaData(identifier, settings.Storage);
        //}

        ///// <summary>
        ///// Checks whether the storage item meta data exists or not.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="storage">The storage</param>
        ///// <returns>True if storage item meta data exists otherwise false</returns>
        //public static bool HasMetaData(string identifier, IStorage storage)
        //{
        //    Task<bool> task = HasMetaDataAsync(identifier, storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Checks whether the storage item meta data exists or not using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <returns>True if storage item meta data exists otherwise false</returns>
        public static Task<bool> HasMetaDataAsync(string identifier)
        {
            return HasMetaDataAsync(identifier, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Checks whether the storage item meta data exists or not.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="settings">The settings</param>
        /// <returns>True if storage item meta data exists otherwise false</returns>
        public static Task<bool> HasMetaDataAsync(string identifier, SaveSystemSettings settings)
        {
            return HasMetaDataAsync(identifier, settings.Storage);
        }

        /// <summary>
        /// Checks whether the storage item meta data exists or not.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="storage">The storage</param>
        /// <returns>True if storage item meta data exists otherwise false</returns>
        public static Task<bool> HasMetaDataAsync(string identifier, IStorage storage)
        {
            return storage.HasMetaData(identifier);
        }

        #endregion

        #region Write All Text Methods

        ///// <summary>
        ///// Writes all the text data to the storage item using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="data">The text data</param>
        //public static void WriteAllText(string identifier, string data)
        //{
        //    WriteAllText(identifier, data, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Writes all the text data to the storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="data">The text data</param>
        ///// <param name="settings">The settings</param>
        //public static void WriteAllText(string identifier, string data, SaveSystemSettings settings)
        //{
        //    WriteAllText(identifier, data, settings.Storage);
        //}

        ///// <summary>
        ///// Writes all the text data to the storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="data">The text data</param>
        ///// <param name="storage">The storage</param>
        //public static void WriteAllText(string identifier, string data, IStorage storage)
        //{
        //    Task task = WriteAllTextAsync(identifier, data, storage);
        //    task.Wait();
        //}

        /// <summary>
        /// Writes all the text data to the storage item using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="data">The text data</param>
        public static Task WriteAllTextAsync(string identifier, string data)
        {
            return WriteAllTextAsync(identifier, data, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Writes all the text data to the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="data">The text data</param>
        /// <param name="settings">The settings</param>
        public static Task WriteAllTextAsync(string identifier, string data, SaveSystemSettings settings)
        {
            return WriteAllTextAsync(identifier, data, settings.Storage);
        }

        /// <summary>
        /// Writes all the text data to the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="data">The text data</param>
        /// <param name="storage">The storage</param>
        public static Task WriteAllTextAsync(string identifier, string data, IStorage storage)
        {
            return storage.WriteAllText(identifier, data);
        }

        #endregion

        #region Read All Text Methods

        //public static string ReadAllText(string identifier)
        //{
        //    return ReadAllText(identifier, SaveSystemSettings.DefaultSettings);
        //}

        //public static string ReadAllText(string identifier, SaveSystemSettings settings)
        //{
        //    return ReadAllText(identifier, settings.Storage);
        //}

        //public static string ReadAllText(string identifier, IStorage storage)
        //{
        //    Task<string> task = ReadAllTextAsync(identifier, storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Reads all the text data from the storage item using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <returns>The text data</returns>
        public static Task<string> ReadAllTextAsync(string identifier)
        {
            return ReadAllTextAsync(identifier, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Reads all the text data from the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="settings">The settings</param>
        /// <returns>The text data</returns>
        public static Task<string> ReadAllTextAsync(string identifier, SaveSystemSettings settings)
        {
            return ReadAllTextAsync(identifier, settings.Storage);
        }

        /// <summary>
        /// Reads all the text data from the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="storage">The storage</param>
        /// <returns>The text data</returns>
        public static Task<string> ReadAllTextAsync(string identifier, IStorage storage)
        {
            return storage.ReadAllText(identifier);
        }

        #endregion

        #region Write All Bytes Methods

        ///// <summary>
        ///// Writes all the binary data in the storage item using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item</param>
        ///// <param name="data">The binary data</param>
        //public static void WriteAllBytes(string identifier, byte[] data)
        //{
        //    WriteAllBytes(identifier, data, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Writes all the binary data in the storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item</param>
        ///// <param name="data">The binary data</param>
        ///// <param name="settings">The settings</param>
        //public static void WriteAllBytes(string identifier, byte[] data, SaveSystemSettings settings)
        //{
        //    WriteAllBytes(identifier, data, settings.Storage);
        //}

        ///// <summary>
        ///// Writes all the binary data in the storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item</param>
        ///// <param name="data">The binary data</param>
        ///// <param name="storage">The storage</param>
        //public static void WriteAllBytes(string identifier, byte[] data, IStorage storage)
        //{
        //    Task task = WriteAllBytesAsync(identifier, data, storage);
        //    task.Wait();
        //}

        /// <summary>
        /// Writes all the binary data in the storage item using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item</param>
        /// <param name="data">The binary data</param>
        public static Task WriteAllBytesAsync(string identifier, byte[] data)
        {
            return WriteAllBytesAsync(identifier, data, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Writes all the binary data in the storage item.
        /// </summary>
        /// <param name="identifier">The storage item</param>
        /// <param name="data">The binary data</param>
        /// <param name="settings">The settings</param>
        public static Task WriteAllBytesAsync(string identifier, byte[] data, SaveSystemSettings settings)
        {
            return WriteAllBytesAsync(identifier, data, settings.Storage);
        }

        /// <summary>
        /// Writes all the binary data in the storage item.
        /// </summary>
        /// <param name="identifier">The storage item</param>
        /// <param name="data">The binary data</param>
        /// <param name="storage">The storage</param>
        public static Task WriteAllBytesAsync(string identifier, byte[] data, IStorage storage)
        {
            return storage.WriteAllBytes(identifier, data);
        }

        #endregion

        #region Read All Bytes Methods

        ///// <summary>
        ///// Reads all the binary data from the storage item using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <returns>The binary data</returns>
        //public static byte[] ReadAllBytes(string identifier)
        //{
        //    return ReadAllBytes(identifier, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Reads all the binary data from the storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>The binary data</returns>
        //public static byte[] ReadAllBytes(string identifier, SaveSystemSettings settings)
        //{
        //    return ReadAllBytes(identifier, settings.Storage);
        //}

        ///// <summary>
        ///// Reads all the binary data from the storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="storage">The storage</param>
        ///// <returns>The binary data</returns>
        //public static byte[] ReadAllBytes(string identifier, IStorage storage)
        //{
        //    Task<byte[]> task = ReadAllBytesAsync(identifier, storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Reads all the binary data from the storage item using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <returns>The binary data</returns>
        public static Task<byte[]> ReadAllBytesAsync(string identifier)
        {
            return ReadAllBytesAsync(identifier, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Reads all the binary data from the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="settings">The settings</param>
        /// <returns>The binary data</returns>
        public static Task<byte[]> ReadAllBytesAsync(string identifier, SaveSystemSettings settings)
        {
            return ReadAllBytesAsync(identifier, settings.Storage);
        }

        /// <summary>
        /// Reads all the binary data from the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="storage">The storage</param>
        /// <returns>The binary data</returns>
        public static Task<byte[]> ReadAllBytesAsync(string identifier, IStorage storage)
        {
            return storage.ReadAllBytes(identifier);
        }

        #endregion

        #region Save Image PNG Methods

        ///// <summary>
        ///// Saves the texture by encoding it to PNG format using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="image">The texture</param>
        //public static void SaveImagePNG(string identifier, Texture2D image)
        //{
        //    SaveImagePNG(identifier, image, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Saves the texture by encoding it to PNG format.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="image">The texture</param>
        ///// <param name="settings">The settings</param>
        //public static void SaveImagePNG(string identifier, Texture2D image, SaveSystemSettings settings)
        //{
        //    SaveImagePNG(identifier, image, settings.Storage);
        //}

        ///// <summary>
        ///// Saves the texture by encoding it to PNG format.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="image">The texture</param>
        ///// <param name="storage">The storage</param>
        //public static void SaveImagePNG(string identifier, Texture2D image, IStorage storage)
        //{
        //    Task task = SaveImagePNGAsync(identifier, image, storage);
        //    task.Wait();
        //}

        /// <summary>
        /// Saves the texture by encoding it to PNG format using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="image">The texture</param>
        public static Task SaveImagePNGAsync(string identifier, Texture2D image)
        {
            return SaveImagePNGAsync(identifier, image, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Saves the texture by encoding it to PNG format.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="image">The texture</param>
        /// <param name="settings">The settings</param>
        public static Task SaveImagePNGAsync(string identifier, Texture2D image, SaveSystemSettings settings)
        {
            return SaveImagePNGAsync(identifier, image, settings.Storage);
        }

        /// <summary>
        /// Saves the texture by encoding it to PNG format.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="image">The texture</param>
        /// <param name="storage">The storage</param>
        public static Task SaveImagePNGAsync(string identifier, Texture2D image, IStorage storage)
        {
            return WriteAllBytesAsync(identifier, image.EncodeToPNG(), storage);
        }

        #endregion

        #region Save Image JPG Methods

        ///// <summary>
        ///// Saves the texture by encoding it to JPG format using the default settings
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="image">The texture</param>
        ///// <param name="quality">The quality</param>
        //public static void SaveImageJPG(string identifier, Texture2D image, int quality)
        //{
        //    SaveImageJPG(identifier, image, quality, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Saves the texture by encoding it to JPG format.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="image">The texture</param>
        ///// <param name="quality">The quality</param>
        ///// <param name="settings">The settings</param>
        //public static void SaveImageJPG(string identifier, Texture2D image, int quality, SaveSystemSettings settings)
        //{
        //    SaveImageJPG(identifier, image, quality, settings.Storage);
        //}

        ///// <summary>
        ///// Saves the texture by encoding it to JPG format.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="image">The texture</param>
        ///// <param name="quality">The quality</param>
        ///// <param name="storage">The storage</param>
        //public static void SaveImageJPG(string identifier, Texture2D image, int quality, IStorage storage)
        //{
        //    Task task = SaveImageJPGAsync(identifier, image, quality, storage);
        //    task.Wait();
        //}

        /// <summary>
        /// Saves the texture by encoding it to JPG format using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="image">The texture</param>
        /// <param name="quality">The quality</param>
        public static Task SaveImageJPGAsync(string identifier, Texture2D image, int quality)
        {
            return SaveImageJPGAsync(identifier, image, quality, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Saves the texture by encoding it to JPG format.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="image">The texture</param>
        /// <param name="quality">The quality</param>
        /// <param name="settings">The settings</param>
        public static Task SaveImageJPGAsync(string identifier, Texture2D image, int quality, SaveSystemSettings settings)
        {
            return SaveImageJPGAsync(identifier, image, quality, settings.Storage);
        }

        /// <summary>
        /// Saves the texture by encoding it to JPG format.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="image">The texture</param>
        /// <param name="quality">The quality</param>
        /// <param name="storage">The storage</param>
        public static Task SaveImageJPGAsync(string identifier, Texture2D image, int quality, IStorage storage)
        {
            return WriteAllBytesAsync(identifier, image.EncodeToJPG(quality), storage);
        }

        #endregion

        #region Save Image EXR Methods

        ///// <summary>
        ///// Saves the texture by encoding it to EXR format using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="image">The texture</param>
        ///// <param name="flags">The EXR flags</param>
        //public static void SaveImageEXR(string identifier, Texture2D image, Texture2D.EXRFlags flags)
        //{
        //    SaveImageEXR(identifier, image, flags, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Saves the texture by encoding it to EXR format.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="image">The texture</param>
        ///// <param name="flags">The EXR flags</param>
        ///// <param name="settings">The settings</param>
        //public static void SaveImageEXR(string identifier, Texture2D image, Texture2D.EXRFlags flags, SaveSystemSettings settings)
        //{
        //    SaveImageEXR(identifier, image, flags, settings.Storage);
        //}

        ///// <summary>
        ///// Saves the texture by encoding it to EXR format.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="image">The texture</param>
        ///// <param name="flags">The EXR flags</param>
        ///// <param name="storage">The storage</param>
        //public static void SaveImageEXR(string identifier, Texture2D image, Texture2D.EXRFlags flags, IStorage storage)
        //{
        //    Task task = SaveImageEXRAsync(identifier, image, flags, storage);
        //    task.Wait();
        //}

        /// <summary>
        /// Saves the texture by encoding it to EXR format using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="image">The texture</param>
        /// <param name="flags">The EXR flags</param>
        public static Task SaveImageEXRAsync(string identifier, Texture2D image, Texture2D.EXRFlags flags)
        {
            return SaveImageEXRAsync(identifier, image, flags, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Saves the texture by encoding it to EXR format.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="image">The texture</param>
        /// <param name="flags">The EXR flags</param>
        /// <param name="settings">The settings</param>
        public static Task SaveImageEXRAsync(string identifier, Texture2D image, Texture2D.EXRFlags flags, SaveSystemSettings settings)
        {
            return SaveImageEXRAsync(identifier, image, flags, settings.Storage);
        }

        /// <summary>
        /// Saves the texture by encoding it to EXR format.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="image">The texture</param>
        /// <param name="flags">The EXR flags</param>
        /// <param name="storage">The storage</param>
        public static Task SaveImageEXRAsync(string identifier, Texture2D image, Texture2D.EXRFlags flags, IStorage storage)
        {
            return WriteAllBytesAsync(identifier, image.EncodeToEXR(flags), storage);
        }

        #endregion

        #region Save Image TGA Methods

        ///// <summary>
        ///// Saves the texture by encoding it to TGA format using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="image">The texture</param>
        //public static void SaveImageTGA(string identifier, Texture2D image)
        //{
        //    SaveImageTGA(identifier, image, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Saves the texture by encoding it to TGA format.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="image">The texture</param>
        ///// <param name="settings">The settings</param>
        //public static void SaveImageTGA(string identifier, Texture2D image, SaveSystemSettings settings)
        //{
        //    SaveImageTGA(identifier, image, settings.Storage);
        //}

        ///// <summary>
        ///// Saves the texture by encoding it to TGA format.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="image">The texture</param>
        ///// <param name="storage">The storage</param>
        //public static void SaveImageTGA(string identifier, Texture2D image, IStorage storage)
        //{
        //    Task task = SaveImageTGAAsync(identifier, image, storage);
        //    task.Wait();
        //}

#if UNITY_2018_3_OR_NEWER

        /// <summary>
        /// Saves the texture by encoding it to TGA format using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="image">The texture</param>
        public static Task SaveImageTGAAsync(string identifier, Texture2D image)
        {
            return SaveImageTGAAsync(identifier, image, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Saves the texture by encoding it to TGA format.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="image">The texture</param>
        /// <param name="settings">The settings</param>
        public static Task SaveImageTGAAsync(string identifier, Texture2D image, SaveSystemSettings settings)
        {
            return SaveImageTGAAsync(identifier, image, settings.Storage);
        }

        /// <summary>
        /// Saves the texture by encoding it to TGA format.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="image">The texture</param>
        /// <param name="storage">The storage</param>
        public static Task SaveImageTGAAsync(string identifier, Texture2D image, IStorage storage)
        {
            return WriteAllBytesAsync(identifier, image.EncodeToTGA(), storage);
        }

#endif

        #endregion

        #region Load Image Methods

        ///// <summary>
        ///// Loads the texture from the image encoded data using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <returns>The texture loaded from image data</returns>
        //public static Texture2D LoadImage(string identifier)
        //{
        //    return LoadImage(identifier, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Loads the texture from the image encoded data.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>The texture loaded from image data</returns>
        //public static Texture2D LoadImage(string identifier, SaveSystemSettings settings)
        //{
        //    return LoadImage(identifier, settings.Storage);
        //}

        ///// <summary>
        ///// Loads the texture from the image encoded data.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="storage">The storage</param>
        ///// <returns>The texture loaded from image data</returns>
        //public static Texture2D LoadImage(string identifier, IStorage storage)
        //{
        //    Task<Texture2D> task = LoadImageAsync(identifier, storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Loads the texture from the image encoded data using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <returns>The texture loaded from image data</returns>
        public static Task<Texture2D> LoadImageAsync(string identifier)
        {
            return LoadImageAsync(identifier, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Loads the texture from the image encoded data.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="settings">The settings</param>
        /// <returns>The texture loaded from image data</returns>
        public static Task<Texture2D> LoadImageAsync(string identifier, SaveSystemSettings settings)
        {
            return LoadImageAsync(identifier, settings.Storage);
        }

        /// <summary>
        /// Loads the texture from the image encoded data.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="storage">The storage</param>
        /// <returns>The texture loaded from image data</returns>
        public static async Task<Texture2D> LoadImageAsync(string identifier, IStorage storage)
        {
            byte[] data = await ReadAllBytesAsync(identifier, storage);
            Texture2D image = new Texture2D(0, 0);
            image.LoadImage(data);
            return image;
        }

        #endregion

        #region Create Backup Methods

        ///// <summary>
        ///// Creates a new backup for the storage item using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <returns>The created backup information <see cref="StorageBackup"/></returns>
        //public static StorageBackup CreateBackup(string identifier)
        //{
        //    return CreateBackup(identifier, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Creates a new backup for the storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>The created backup information <see cref="StorageBackup"/></returns>
        //public static StorageBackup CreateBackup(string identifier, SaveSystemSettings settings)
        //{
        //    return CreateBackup(identifier, settings.Storage);
        //}

        ///// <summary>
        ///// Creates a new backup for the storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="storage">The storage</param>
        ///// <returns>The created backup information <see cref="StorageBackup"/></returns>
        //public static StorageBackup CreateBackup(string identifier, IStorage storage)
        //{
        //    Task<StorageBackup> task = CreateBackupAsync(identifier, storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Creates a new backup for the storage item using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <returns>The created backup information <see cref="StorageBackup"/></returns>
        public static Task<StorageBackup> CreateBackupAsync(string identifier)
        {
            return CreateBackupAsync(identifier, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Creates a new backup for the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="settings">The settings</param>
        /// <returns>The created backup information <see cref="StorageBackup"/></returns>
        public static Task<StorageBackup> CreateBackupAsync(string identifier, SaveSystemSettings settings)
        {
            return CreateBackupAsync(identifier, settings.Storage);
        }

        /// <summary>
        /// Creates a new backup for the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="storage">The storage</param>
        /// <returns>The created backup information <see cref="StorageBackup"/></returns>
        public static Task<StorageBackup> CreateBackupAsync(string identifier, IStorage storage)
        {
            return storage.CreateBackup(identifier);
        }

        #endregion

        #region Get Latest Backup Methods

        /// <summary>
        /// Gets the latest backup for the storage item using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <returns>The created backup information <see cref="StorageBackup"/></returns>
        public static Task<StorageBackup> GetLatestBackupAsync(string identifier)
        {
            return GetLatestBackupAsync(identifier, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Gets the latest backup for the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="settings">The settings</param>
        /// <returns>The created backup information <see cref="StorageBackup"/></returns>
        public static Task<StorageBackup> GetLatestBackupAsync(string identifier, SaveSystemSettings settings)
        {
            return GetLatestBackupAsync(identifier, settings.Storage);
        }

        /// <summary>
        /// Gets the latest backup for the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="storage">The storage</param>
        /// <returns>The latest backup information <see cref="StorageBackup"/></returns>
        public static Task<StorageBackup> GetLatestBackupAsync(string identifier, IStorage storage)
        {
            return storage.GetLatestBackup(identifier);
        }

        #endregion

        #region Get Backups Methods

        /// <summary>
        /// Gets the backups for the storage item using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <returns>The backups information <see cref="StorageBackup"/></returns>
        public static Task<List<StorageBackup>> GetBackupsAsync(string identifier)
        {
            return GetBackupsAsync(identifier, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Gets the backups for the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="settings">The settings</param>
        /// <returns>The backups information <see cref="StorageBackup"/></returns>
        public static Task<List<StorageBackup>> GetBackupsAsync(string identifier, SaveSystemSettings settings)
        {
            return GetBackupsAsync(identifier, settings.Storage);
        }

        /// <summary>
        /// Gets the backups for the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="storage">The storage</param>
        /// <returns>The backups information <see cref="StorageBackup"/></returns>
        public static Task<List<StorageBackup>> GetBackupsAsync(string identifier, IStorage storage)
        {
            return storage.GetBackups(identifier);
        }

        #endregion

        #region Restore Latest Backup Methods

        ///// <summary>
        ///// Restores the latest backup for the storage item using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static bool RestoreLatestBackup(string identifier)
        //{
        //    return RestoreLatestBackup(identifier, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Restores the latest backup for the storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static bool RestoreLatestBackup(string identifier, SaveSystemSettings settings)
        //{
        //    return RestoreLatestBackup(identifier, settings.Storage);
        //}

        ///// <summary>
        ///// Restores the latest backup for the storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="storage">The storage</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static bool RestoreLatestBackup(string identifier, IStorage storage)
        //{
        //    Task<bool> task = RestoreLatestBackupAsync(identifier, storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Restores the latest backup for the storage item using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<bool> RestoreLatestBackupAsync(string identifier)
        {
            return RestoreLatestBackupAsync(identifier, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Restores the latest backup for the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="settings">The settings</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<bool> RestoreLatestBackupAsync(string identifier, SaveSystemSettings settings)
        {
            return RestoreLatestBackupAsync(identifier, settings.Storage);
        }

        /// <summary>
        /// Restores the latest backup for the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="storage">The storage</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<bool> RestoreLatestBackupAsync(string identifier, IStorage storage)
        {
            return storage.RestoreLatestBackup(identifier);
        }

        #endregion

        #region Restore Backup Methods

        ///// <summary>
        ///// Restores the backup for the storage item using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="backup">The backup to restore</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static bool RestoreBackup(string identifier, StorageBackup backup)
        //{
        //    return RestoreBackup(identifier, backup, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Restores the backup for the storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="backup">The backup to restore</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static bool RestoreBackup(string identifier, StorageBackup backup, SaveSystemSettings settings)
        //{
        //    return RestoreBackup(identifier, backup, settings.Storage);
        //}

        ///// <summary>
        ///// Restores the backup for the storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="backup">The backup to restore</param>
        ///// <param name="storage">The storage</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static bool RestoreBackup(string identifier, StorageBackup backup, IStorage storage)
        //{
        //    Task<bool> task = RestoreBackupAsync(identifier, backup, storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Restores the backup for the storage item using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="backup">The backup to restore</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<bool> RestoreBackupAsync(string identifier, StorageBackup backup)
        {
            return RestoreBackupAsync(identifier, backup, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Restores the backup for the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="backup">The backup to restore</param>
        /// <param name="settings">The settings</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<bool> RestoreBackupAsync(string identifier, StorageBackup backup, SaveSystemSettings settings)
        {
            return RestoreBackupAsync(identifier, backup, settings.Storage);
        }

        /// <summary>
        /// Restores the backup for the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="backup">The backup to restore</param>
        /// <param name="storage">The storage</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<bool> RestoreBackupAsync(string identifier, StorageBackup backup, IStorage storage)
        {
            return storage.RestoreBackup(identifier, backup);
        }

        #endregion

        #region Delete Backup Methods

        ///// <summary>
        ///// Deletes the backup for the stroage item using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="backup">The storage backup information</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static StorageDeleteOperationResult DeleteBackup(string identifier, StorageBackup backup)
        //{
        //    return DeleteBackup(identifier, backup, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Deletes the backup for the stroage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="backup">The storage backup information</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static StorageDeleteOperationResult DeleteBackup(string identifier, StorageBackup backup, SaveSystemSettings settings)
        //{
        //    return DeleteBackup(identifier, backup, settings.Storage);
        //}

        ///// <summary>
        ///// Deletes the backup for the stroage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier</param>
        ///// <param name="backup">The storage backup information</param>
        ///// <param name="storage">The storage</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static StorageDeleteOperationResult DeleteBackup(string identifier, StorageBackup backup, IStorage storage)
        //{
        //    Task<StorageDeleteOperationResult> task = DeleteBackupAsync(identifier, backup, storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Deletes the backup for the stroage item using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="backup">The storage backup information</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<StorageDeleteOperationResult> DeleteBackupAsync(string identifier, StorageBackup backup)
        {
            return DeleteBackupAsync(identifier, backup, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Deletes the backup for the stroage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="backup">The storage backup information</param>
        /// <param name="settings">The settings</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<StorageDeleteOperationResult> DeleteBackupAsync(string identifier, StorageBackup backup, SaveSystemSettings settings)
        {
            return DeleteBackupAsync(identifier, backup, settings.Storage);
        }

        /// <summary>
        /// Deletes the backup for the stroage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier</param>
        /// <param name="backup">The storage backup information</param>
        /// <param name="storage">The storage</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<StorageDeleteOperationResult> DeleteBackupAsync(string identifier, StorageBackup backup, IStorage storage)
        {
            return storage.DeleteBackup(identifier, backup);
        }

        #endregion

        #region Delete Backups Methods

        ///// <summary>
        ///// Deletes all the backups for the storage item using the default settings.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier to delete backups</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static bool DeleteBackups(string identifier)
        //{
        //    return DeleteBackups(identifier, SaveSystemSettings.DefaultSettings);
        //}

        ///// <summary>
        ///// Deletes all the backups for the storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier to delete backups</param>
        ///// <param name="settings">The settings</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static bool DeleteBackups(string identifier, SaveSystemSettings settings)
        //{
        //    return DeleteBackups(identifier, settings.Storage);
        //}

        ///// <summary>
        ///// Deletes all the backups for the storage item.
        ///// </summary>
        ///// <param name="identifier">The storage item identifier to delete backups</param>
        ///// <param name="storage">The storage</param>
        ///// <returns>True if operation was successful otherwise false</returns>
        //public static bool DeleteBackups(string identifier, IStorage storage)
        //{
        //    Task<bool> task = DeleteBackupsAsync(identifier, storage);
        //    task.Wait();
        //    return task.Result;
        //}

        /// <summary>
        /// Deletes all the backups for the storage item using the default settings.
        /// </summary>
        /// <param name="identifier">The storage item identifier to delete backups</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<bool> DeleteBackupsAsync(string identifier)
        {
            return DeleteBackupsAsync(identifier, SaveSystemSettings.DefaultSettings);
        }

        /// <summary>
        /// Deletes all the backups for the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier to delete backups</param>
        /// <param name="settings">The settings</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<bool> DeleteBackupsAsync(string identifier, SaveSystemSettings settings)
        {
            return DeleteBackupsAsync(identifier, settings.Storage);
        }

        /// <summary>
        /// Deletes all the backups for the storage item.
        /// </summary>
        /// <param name="identifier">The storage item identifier to delete backups</param>
        /// <param name="storage">The storage</param>
        /// <returns>True if operation was successful otherwise false</returns>
        public static Task<bool> DeleteBackupsAsync(string identifier, IStorage storage)
        {
            return storage.DeleteBackups(identifier);
        }

        #endregion

    }

    /// <summary>
    /// Save event args.
    /// </summary>
    public class SaveEventArgs : EventArgs
    {

        public readonly string Identifier;
        public readonly object Value;
        public readonly IStorage Storage;
        public readonly SaveSystemJsonSerializer Serializer;
        public readonly ISaveSystemEncryption Encryption;
        public readonly string Password;

        public SaveEventArgs(string identifier, object value, IStorage storage, SaveSystemJsonSerializer serializer, ISaveSystemEncryption encryption, string password)
        {
            this.Identifier = identifier;
            this.Value = value;
            this.Storage = storage;
            this.Serializer = serializer;
            this.Encryption = encryption;
            this.Password = password;
        }

    }

    /// <summary>
    /// Load event args.
    /// </summary>
    public class LoadEventArgs : EventArgs
    {

        public readonly string Identifier;
        public readonly Type ObjectType;
        public readonly IStorage Storage;
        public readonly SaveSystemJsonSerializer Serializer;
        public readonly ISaveSystemEncryption Encryption;
        public readonly string Password;

        public LoadEventArgs(string identifier, Type objectType, IStorage storage, SaveSystemJsonSerializer serializer, ISaveSystemEncryption encryption, string password)
        {
            this.Identifier = identifier;
            this.ObjectType = objectType;
            this.Storage = storage;
            this.Serializer = serializer;
            this.Encryption = encryption;
            this.Password = password;
        }

    }

    /// <summary>
    /// LoadInto event args.
    /// </summary>
    public class LoadIntoEventArgs : EventArgs
    {

        public readonly string Identifier;
        public readonly object Target;
        public readonly IStorage Storage;
        public readonly SaveSystemJsonSerializer Serializer;
        public readonly ISaveSystemEncryption Encryption;
        public readonly string Password;

        public LoadIntoEventArgs(string identifier, object target, IStorage storage, SaveSystemJsonSerializer serializer, ISaveSystemEncryption encryption, string password)
        {
            this.Identifier = identifier;
            this.Target = target;
            this.Storage = storage;
            this.Serializer = serializer;
            this.Encryption = encryption;
            this.Password = password;
        }

    }

}