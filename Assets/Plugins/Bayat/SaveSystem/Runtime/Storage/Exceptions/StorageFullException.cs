using System;

namespace Bayat.SaveSystem.Storage
{

    [Serializable]
    public class StorageFullException : Exception
    {

        public StorageFullException() : base("The storage is full.") { }

        public StorageFullException(Exception inner) : base("The storage  is full.", inner) { }

    }

}
