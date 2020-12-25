using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.Storage.Streams;
#endif

namespace Bayat.SaveSystem.Storage
{

    /// <summary>
    /// Local disk file storage implementation.
    /// </summary>
    public class LocalDiskStorage : StorageBase
    {

        protected string basePath;

        /// <summary>
        /// The base path.
        /// </summary>
        public virtual string BasePath
        {
            get
            {
                return this.basePath;
            }
            set
            {
                this.basePath = value;
            }
        }

        public virtual string TemporarySuffix
        {
            get
            {
                return ".temp";
            }
        }

        public LocalDiskStorage(string basePath)
        {
            this.basePath = basePath;
        }

        public override
#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
                        async
#endif
            Task<IStorageStream> GetWriteStream(string identifier)
        {
            string fullPath = GetAbsolutePath(identifier);

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            identifier = identifier.Replace('/', '\\');
            fullPath = fullPath.Replace('/', '\\');
#endif

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(this.BasePath.Replace('/', '\\'));

            // Ensure directory exists
            string folderName = Path.GetDirectoryName(identifier);
            if (!string.IsNullOrEmpty(folderName))
            {
                folder = await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            }
            Stream file = await folder.OpenStreamForWriteAsync(Path.GetFileName(identifier) + this.TemporarySuffix, CreationCollisionOption.OpenIfExists);
            FileStorageStream storageStream = new FileStorageStream(fullPath, identifier, file);
            return storageStream;
#else
            try
            {
                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                FileStream file = File.Open(fullPath + this.TemporarySuffix, FileMode.Create);
                FileStorageStream storageStream = new FileStorageStream(fullPath, identifier, file);
                return Task.FromResult<IStorageStream>(storageStream);
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    throw new StorageItemNotFoundException(identifier, ex);
                }
                if (ex is NotSupportedException || ex is PathTooLongException || ex is ArgumentException || ex is ArgumentNullException)
                {
                    throw new StorageInvalidIdentifierException(identifier, ex);
                }
                throw;
            }
#endif
        }

        protected override
#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
                        async
#endif
            Task CommitWriteStreamInternal(IStorageStream stream)
        {
            FileStorageStream fileStream = (FileStorageStream)stream;

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            stream.UnderlyingStream.Close();
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(this.BasePath.Replace('/', '\\'));
            string folderName = Path.GetDirectoryName(stream.Identifier);
            if (!string.IsNullOrEmpty(folderName))
            {
                folder = await folder.GetFolderAsync(folderName);
            }
            StorageFile tempFile = await folder.GetFileAsync(Path.GetFileName(stream.Identifier) + this.TemporarySuffix);
            await tempFile.RenameAsync(Path.GetFileName(stream.Identifier), NameCollisionOption.ReplaceExisting);
#else
            File.Delete(fileStream.FullPath);
            File.Move(fileStream.FullPath + this.TemporarySuffix, fileStream.FullPath);
            return Task.CompletedTask;
#endif
        }

        public override
#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
                        async
#endif
            Task<IStorageStream> GetReadStream(string identifier)
        {
            string fullPath = GetAbsolutePath(identifier);

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            identifier = identifier.Replace('/', '\\');
            fullPath = fullPath.Replace('/', '\\');
#endif

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(this.BasePath.Replace('/', '\\'));

            // Ensure directory exists
            string folderName = Path.GetDirectoryName(identifier);
            if (!string.IsNullOrEmpty(folderName))
            {
                folder = await folder.GetFolderAsync(folderName);
            }
            Stream file = await folder.OpenStreamForReadAsync(Path.GetFileName(identifier));
            FileStorageStream storageStream = new FileStorageStream(fullPath, identifier, file);
            return storageStream;
#else
            try
            {
                FileStream file = File.OpenRead(fullPath);
                FileStorageStream storageStream = new FileStorageStream(fullPath, identifier, file);
                return Task.FromResult<IStorageStream>(storageStream);
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    throw new StorageItemNotFoundException(identifier, ex);
                }
                if (ex is NotSupportedException || ex is PathTooLongException || ex is ArgumentException || ex is ArgumentNullException)
                {
                    throw new StorageInvalidIdentifierException(identifier, ex);
                }
                throw;
            }
#endif
        }

        protected override
#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
                        async
#endif
            Task WriteAllTextInternal(string identifier, string data)
        {
            string fullPath = GetAbsolutePath(identifier);

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            identifier = identifier.Replace('/', '\\');
            fullPath = fullPath.Replace('/', '\\');
#endif

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(this.BasePath.Replace('/', '\\'));
            string folderName = Path.GetDirectoryName(identifier);
            if (!string.IsNullOrEmpty(folderName))
            {
                folder = await folder.CreateFolderAsync(folderName);
            }
            StorageFile file = await folder.CreateFileAsync(Path.GetFileName(identifier), CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, data);
#else
            try
            {
                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                using (FileStream stream = File.Open(fullPath, FileMode.OpenOrCreate))
                {
                    if (stream.Length > 0)
                    {
                        stream.SetLength(0);
                    }
                    using (TextWriter writer = new StreamWriter(stream, this.TextEncoding))
                    {
                        writer.Write(data);
                        writer.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    throw new StorageItemNotFoundException(identifier, ex);
                }
                if (ex is NotSupportedException || ex is PathTooLongException || ex is ArgumentException || ex is ArgumentNullException)
                {
                    throw new StorageInvalidIdentifierException(identifier, ex);
                }
                throw;
            }
            if (IsMetaIdentifier(identifier))
            {
                File.SetAttributes(fullPath, FileAttributes.Hidden);
            }
            return Task.CompletedTask;
#endif
        }

        public override
#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
                        async
#endif
            Task<string> ReadAllText(string identifier)
        {
            string fullPath = GetAbsolutePath(identifier);

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            identifier = identifier.Replace('/', '\\');
            fullPath = fullPath.Replace('/', '\\');
#endif

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(this.BasePath.Replace('/', '\\'));
            string folderName = Path.GetDirectoryName(identifier);
            if (!string.IsNullOrEmpty(folderName))
            {
                folder = await folder.GetFolderAsync(folderName);
            }
            StorageFile file = await folder.GetFileAsync(Path.GetFileName(identifier));
            return await FileIO.ReadTextAsync(file);
#else
            try
            {
                return Task.FromResult(File.ReadAllText(fullPath));
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    throw new StorageItemNotFoundException(identifier, ex);
                }
                if (ex is NotSupportedException || ex is PathTooLongException || ex is ArgumentException || ex is ArgumentNullException)
                {
                    throw new StorageInvalidIdentifierException(identifier, ex);
                }
                throw;
            }
#endif
        }

        protected override
#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
                        async
#endif
            Task WriteAllBytesInternal(string identifier, byte[] data)
        {
            string fullPath = GetAbsolutePath(identifier);

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            identifier = identifier.Replace('/', '\\');
            fullPath = fullPath.Replace('/', '\\');
#endif

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(this.BasePath.Replace('/', '\\'));
            string folderName = Path.GetDirectoryName(identifier);
            if (!string.IsNullOrEmpty(folderName))
            {
                folder = await folder.GetFolderAsync(folderName);
            }
            StorageFile file = await folder.CreateFileAsync(Path.GetFileName(identifier), CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(file, data);
#else
            try
            {
                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                using (FileStream stream = File.Open(fullPath, FileMode.OpenOrCreate))
                {
                    if (stream.Length > 0)
                    {
                        stream.SetLength(0);
                    }
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    throw new StorageItemNotFoundException(identifier, ex);
                }
                if (ex is NotSupportedException || ex is PathTooLongException || ex is ArgumentException || ex is ArgumentNullException)
                {
                    throw new StorageInvalidIdentifierException(identifier, ex);
                }
                throw;
            }
            if (IsMetaIdentifier(identifier))
            {
                File.SetAttributes(fullPath, FileAttributes.Hidden);
            }
            return Task.CompletedTask;
#endif
        }

        public override
#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
                        async
#endif
            Task<byte[]> ReadAllBytes(string identifier)
        {
            string fullPath = GetAbsolutePath(identifier);

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            identifier = identifier.Replace('/', '\\');
            fullPath = fullPath.Replace('/', '\\');
#endif

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(this.BasePath.Replace('/', '\\'));
            string folderName = Path.GetDirectoryName(identifier);
            if (!string.IsNullOrEmpty(folderName))
            {
                folder = await folder.GetFolderAsync(folderName);
            }
            StorageFile file = await folder.GetFileAsync(Path.GetFileName(identifier));
            return (await FileIO.ReadBufferAsync(file)).ToArray();
#else
            try
            {
                return Task.FromResult(File.ReadAllBytes(fullPath));
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    throw new StorageItemNotFoundException(identifier, ex);
                }
                if (ex is NotSupportedException || ex is PathTooLongException || ex is ArgumentException || ex is ArgumentNullException)
                {
                    throw new StorageInvalidIdentifierException(identifier, ex);
                }
                throw;
            }
#endif
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

        protected override
#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
                        async
#endif
            Task<StorageDeleteOperationResult> DeleteInternal(string identifier)
        {
            bool result = false;
            string fullPath = GetAbsolutePath(identifier);

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            identifier = identifier.Replace('/', '\\');
            fullPath = fullPath.Replace('/', '\\');
#endif

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(this.BasePath.Replace('/', '\\'));
            try
            {
                folder = await folder.GetFolderAsync(identifier);
                await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                result = true;
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException || ex is ArgumentException)
                {
                    string folderName = Path.GetDirectoryName(identifier);
                    try
                    {
                        if (!string.IsNullOrEmpty(folderName))
                        {
                            folder = await folder.GetFolderAsync(folderName);
                        }
                        StorageFile file = await folder.GetFileAsync(Path.GetFileName(identifier));
                        await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        result = true;
                    }
                    catch (Exception ex2)
                    {
                        if (ex2 is FileNotFoundException || ex2 is ArgumentException)
                        {
                            result = false;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    throw;
                }
            }
            return new StorageDeleteOperationResult(result);
#else
            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    result = true;
                }
                else if (Directory.Exists(fullPath))
                {
                    Directory.Delete(fullPath, true);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    throw new StorageItemNotFoundException(identifier, ex);
                }
                if (ex is NotSupportedException || ex is PathTooLongException || ex is ArgumentException || ex is ArgumentNullException)
                {
                    throw new StorageInvalidIdentifierException(identifier, ex);
                }
                throw;
            }
            return Task.FromResult(new StorageDeleteOperationResult(result));
#endif
        }

        public override
#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
                        async
#endif
            Task<bool> Exists(string identifier)
        {
            bool result = false;
            string fullPath = GetAbsolutePath(identifier);

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            identifier = identifier.Replace('/', '\\');
            fullPath = fullPath.Replace('/', '\\');
#endif

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(this.BasePath.Replace('/', '\\'));
            result = await folder.TryGetItemAsync(identifier) != null;
            return result;
#else
            result = File.Exists(fullPath) || Directory.Exists(fullPath);
            return Task.FromResult(result);
#endif
        }

        protected override async Task<StorageMoveOperationResult> MoveInternal(string oldIdentifier, string newIdentifier, bool replace)
        {
            bool result = false;
            string resultIdentifier = string.Empty;
            string oldFullPath = GetAbsolutePath(oldIdentifier);
            string newFullPath = GetAbsolutePath(newIdentifier);

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            oldIdentifier = oldIdentifier.Replace('/', '\\');
            newIdentifier = newIdentifier.Replace('/', '\\');
            oldFullPath = oldFullPath.Replace('/', '\\');
            newFullPath = newFullPath.Replace('/', '\\');
#endif
            try
            {
                if (File.Exists(oldFullPath))
                {

                    // Moving file to a directory
                    if (Directory.Exists(newFullPath))
                    {
                        newFullPath = Path.Combine(newFullPath, Path.GetFileName(oldFullPath));
                        resultIdentifier = newIdentifier + "/" + oldIdentifier;
                    }
                    else
                    {
                        resultIdentifier = newIdentifier;
                    }
                    if (replace)
                    {
                        if (await Exists(newFullPath))
                        {
                            await Delete(newFullPath);
                        }
                    }
                    File.Move(oldFullPath, newFullPath);
                    result = true;
                }
                else if (Directory.Exists(oldFullPath))
                {

                    // Moving directory to a directory
                    if (Directory.Exists(newFullPath))
                    {
                        newFullPath = Path.Combine(newFullPath, Path.GetFileName(oldFullPath));
                        resultIdentifier = newIdentifier + "/" + oldIdentifier;
                    }
                    else
                    {
                        resultIdentifier = newIdentifier;
                    }
                    if (replace)
                    {
                        if (await Exists(newFullPath))
                        {
                            await Delete(newFullPath);
                        }
                    }
                    Directory.Move(oldFullPath, newFullPath);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    throw new StorageItemNotFoundException(oldIdentifier, ex);
                }
                if (ex is NotSupportedException || ex is PathTooLongException || ex is ArgumentException || ex is ArgumentNullException)
                {
                    throw new StorageInvalidIdentifierException(oldIdentifier, ex);
                }
                throw;
            }
            return new StorageMoveOperationResult(result, resultIdentifier);
        }

        protected override async Task<StorageCopyOperationResult> CopyInternal(string fromIdentifier, string toIdentifier, bool replace)
        {
            bool result = false;
            string resultIdentifier = string.Empty;
            string fromFullPath = GetAbsolutePath(fromIdentifier);
            string toFullPath = GetAbsolutePath(toIdentifier);

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            fromIdentifier = fromIdentifier.Replace('/', '\\');
            toIdentifier = toIdentifier.Replace('/', '\\');
            fromFullPath = fromFullPath.Replace('/', '\\');
            toFullPath = toFullPath.Replace('/', '\\');
#endif
            try
            {
                if (File.Exists(fromFullPath))
                {

                    // Moving file to a directory
                    if (Directory.Exists(toFullPath))
                    {
                        toFullPath = Path.Combine(toFullPath, Path.GetFileName(fromFullPath));
                        resultIdentifier = toIdentifier + "/" + fromIdentifier;
                    }
                    else
                    {
                        resultIdentifier = toIdentifier;
                    }
                    if (replace)
                    {
                        if (await Exists(toFullPath))
                        {
                            await Delete(toFullPath);
                        }
                    }
                    File.Copy(fromFullPath, toFullPath);
                    result = true;
                }
                else if (Directory.Exists(fromFullPath))
                {

                    // Moving directory to a directory
                    if (Directory.Exists(toFullPath))
                    {
                        toFullPath = Path.Combine(toFullPath, Path.GetFileName(fromFullPath));
                        resultIdentifier = toIdentifier + "/" + fromIdentifier;
                    }
                    else
                    {
                        resultIdentifier = toIdentifier;
                    }
                    if (replace)
                    {
                        if (await Exists(toFullPath))
                        {
                            await Delete(toFullPath);
                        }
                    }

                    // Create all of the directories in the new directory
                    foreach (string dirPath in Directory.GetDirectories(fromFullPath, "*", SearchOption.AllDirectories))
                    {
                        Directory.CreateDirectory(dirPath.Replace(fromFullPath, toFullPath));
                    }

                    // Copy all the files & replaces any files with the same name
                    foreach (string newPath in Directory.GetFiles(fromFullPath, "*.*", SearchOption.AllDirectories))
                    {
                        File.Copy(newPath, newPath.Replace(fromFullPath, toFullPath), replace);
                    }
                    resultIdentifier = toFullPath;
                    result = true;
                }
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    throw new StorageItemNotFoundException(fromIdentifier, ex);
                }
                if (ex is NotSupportedException || ex is PathTooLongException || ex is ArgumentException || ex is ArgumentNullException)
                {
                    throw new StorageInvalidIdentifierException(fromIdentifier, ex);
                }
                throw;
            }
            return new StorageCopyOperationResult(result, resultIdentifier);
        }

        public override Task<string[]> List(string identifier, StorageListOptions options)
        {
            List<string> items = new List<string>();
            string fullPath = GetAbsolutePath(identifier);

#if !UNITY_EDITOR && (UNITY_WSA || UNITY_WINRT)
            identifier = identifier.Replace('/', '\\');
            fullPath = fullPath.Replace('/', '\\');
#endif

            if (options.Recurse)
            {
                items.AddRange(Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories));
                items.AddRange(Directory.GetDirectories(fullPath, "*", SearchOption.AllDirectories));
            }
            else if (Directory.Exists(fullPath))
            {
                items.AddRange(Directory.GetDirectories(fullPath));
                items.AddRange(Directory.GetFiles(fullPath));
            }
            if (options.MaxResults.HasValue)
            {
                items.Capacity = options.MaxResults.GetValueOrDefault();
            }
            return Task.FromResult(items.ToArray());
        }

        public override Task<string[]> ListAll()
        {
            return List(this.BasePath, new StorageListOptions()
            {
                Recurse = true
            });
        }

        /// <summary>
        /// Gets the absolute path for the specified identifier using the base path.
        /// </summary>
        /// <param name="identifier">The item identifier</param>
        /// <returns>The absolute path to the file</returns>
        public virtual string GetAbsolutePath(string identifier)
        {
            if (Path.IsPathRooted(identifier))
            {
                return identifier;
            }
            return Path.Combine(this.BasePath, identifier);
        }

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static string GetRelativePath(string fromPath, string toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

    }

    /// <summary>
    /// The file storage stream wrapper.
    /// </summary>
    public class FileStorageStream : StorageStream
    {

        protected readonly string fullPath;

        /// <summary>
        /// The full path to the file.
        /// </summary>
        public virtual string FullPath
        {
            get
            {
                return this.fullPath;
            }
        }

        public FileStorageStream(string fullPath, string identifier, Stream stream) : base(identifier, stream)
        {
            this.fullPath = fullPath;
        }

    }

}