using System.Collections.Generic;

using UnityEngine;

namespace Bayat.Core.Utilities
{

    [System.Serializable]
    public abstract class SerializableDictionary<TKey, TVal> : Dictionary<TKey, TVal>, ISerializationCallbackReceiver
    {

        [SerializeField]
        private List<TKey> _Keys;
        [SerializeField]
        private List<TVal> _Values;

        protected abstract bool KeysAreEqual(TKey a, TKey b);
        protected abstract bool ValuesAreEqual(TVal a, TVal b);

        public void OnBeforeSerialize()
        {
            _Keys = new List<TKey>();
            _Values = new List<TVal>();
            foreach (KeyValuePair<TKey, TVal> pair in this)
            {
                _Keys.Add(pair.Key);
                _Values.Add(pair.Value);
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            Clear();

            if (_Keys.Count != _Values.Count)
                throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable.", _Keys.Count, _Values.Count));

            for (int i = 0; i < _Keys.Count; i++)
            {
                if (_Keys[i] != null)
                {
                    try
                    {
                        Add(_Keys[i], _Values[i]);
                    }
                    catch
                    {
                        // Skip null occurrences in the GetHashCode
                    }
                }
            }

            _Keys = null;
            _Values = null;
        }

        // Changes the key of a value without changing it's position in the underlying Lists.
        // Mainly used in the Editor where position might otherwise change while the user is editing it.
        // Returns true if a change was made.
        public bool ChangeKey(TKey oldKey, TKey newKey)
        {
            if (KeysAreEqual(oldKey, newKey))
                return false;

            var val = this[oldKey];
            Remove(oldKey);
            this[newKey] = val;
            return true;
        }

    }

}