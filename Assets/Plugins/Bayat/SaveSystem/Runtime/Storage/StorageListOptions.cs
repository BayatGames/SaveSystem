namespace Bayat.SaveSystem.Storage
{

    /// <summary>
    /// Storage listing options.
    /// </summary>
    public class StorageListOptions
    {

        /// <summary>
        /// When true, operation will recursively navigate down the folders.
        /// </summary>
        public bool Recurse { get; set; }

        /// <summary>
        /// When set, limits the maximum amount of results. The count affects all object counts, including files and folders.
        /// </summary>
        public int? MaxResults { get; set; }

    }

}