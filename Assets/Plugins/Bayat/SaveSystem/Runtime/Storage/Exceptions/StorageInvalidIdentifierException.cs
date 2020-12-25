using System;

namespace Bayat.SaveSystem.Storage
{

    [Serializable]
    public class StorageInvalidIdentifierException : Exception
    {

        public StorageInvalidIdentifierException(string identifier) : base(string.Format("The identifier '{0}' is invalid for this storage.", identifier)) { }

        public StorageInvalidIdentifierException(string identifier, Exception inner) : base(string.Format("The identifier '{0}' is invalid for this storage.", identifier), inner) { }

    }

}
