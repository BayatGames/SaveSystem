namespace Bayat.SaveSystem.Storage
{

    /// <summary>
    /// Connection factory is responsible for creating storage instances from connection strings.
    /// </summary>
    public interface IConnectionFactory
    {

        /// <summary>
        /// Creates a storage instance from connection string if possible. When this factory does not support this connection
        /// string it returns null.
        /// </summary>
        IStorage CreateStorage(StorageConnectionString connectionString);

    }

}