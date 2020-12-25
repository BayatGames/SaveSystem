using System;

namespace Bayat.SaveSystem.Storage
{

    [Serializable]
    public class StorageItemNotFoundException : Exception
    {

        public StorageItemNotFoundException(string identifier) : base(string.Format("The storage item '{0}' not found.", identifier)) { }

        public StorageItemNotFoundException(string identifier, Exception inner) : base(string.Format("The storage item '{0}' not found.", identifier), inner) { }

    }

}
