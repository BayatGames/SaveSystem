using UnityEngine;

namespace Bayat.SaveSystem.Storage
{

    /// <summary>
    /// The storage factory.
    /// </summary>
    public sealed class StorageFactory
    {

        /// <summary>
        /// The singleton instance of factory.
        /// </summary>
        public static readonly StorageFactory Instance;

        /// <summary>
        /// The default storage implementation of this platform.
        /// </summary>
        public static IStorage DefaultStorage
        {
            get
            {
                if (string.IsNullOrEmpty(Application.persistentDataPath))
                {
                    return FromConnectionString("playerprefs://");
                }
                else
                {
                    return FromConnectionString("disk://path=" + Application.persistentDataPath);
                }
            }
        }

        static StorageFactory()
        {
            Instance = new StorageFactory();
        }

        private StorageFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="IStorage"/> implementation by using the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <returns>A new instance of <see cref="IStorage"/> implementation</returns>
        public static IStorage FromConnectionString(string connectionString)
        {
            return ConnectionStringFactory.CreateStorage(connectionString);
        }

    }

}