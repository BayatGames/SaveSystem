namespace Bayat.SaveSystem.Storage
{

    /// <summary>
    /// Storage operation result.
    /// </summary>
    public class StorageOperationResult
    {

        /// <summary>
        /// Whether the operation succeeed or not.
        /// </summary>
        public readonly bool Succeed;

        public StorageOperationResult(bool succeed)
        {
            this.Succeed = succeed;
        }

    }

    /// <summary>
    /// Storage delete operation result.
    /// </summary>
    public class StorageDeleteOperationResult : StorageOperationResult
    {

        public StorageDeleteOperationResult(bool succeed) : base(succeed)
        {
        }

    }

    /// <summary>
    /// Storage clear operation result.
    /// </summary>
    public class StorageClearOperationResult : StorageOperationResult
    {

        /// <summary>
        /// The deleted storage items.
        /// </summary>
        public readonly string[] DeletedItems;

        public StorageClearOperationResult(bool succeed, string[] deletedItems) : base(succeed)
        {
            this.DeletedItems = deletedItems;
        }

    }

    /// <summary>
    /// Storage move operation result.
    /// </summary>
    public class StorageMoveOperationResult : StorageOperationResult
    {

        /// <summary>
        /// The final result identifier of move operation.
        /// </summary>
        public readonly string ResultIdentifier;

        public StorageMoveOperationResult(bool succeed, string resultIdentifier) : base(succeed)
        {
            this.ResultIdentifier = resultIdentifier;
        }

    }

    /// <summary>
    /// Storage copy operation result.
    /// </summary>
    public class StorageCopyOperationResult : StorageOperationResult
    {

        /// <summary>
        /// The final result identifier of copy operation.
        /// </summary>
        public readonly string ResultIdentifier;

        public StorageCopyOperationResult(bool succeed, string resultIdentifier) : base(succeed)
        {
            this.ResultIdentifier = resultIdentifier;
        }

    }

}