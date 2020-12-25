using System.Linq;

namespace Bayat.Core.Utilities
{

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [System.Serializable]
    public class ReferenceToGuidDictionary : SerializableDictionary<UnityEngine.Object, string>
    {

        protected override bool KeysAreEqual(UnityEngine.Object a, UnityEngine.Object b)
        {
            return a == b;
        }

        protected override bool ValuesAreEqual(string a, string b)
        {
            return a == b;
        }

        public int RemoveNullValues()
        {
            var nullKeys = this.Where(pair => pair.Value == null)
                .Select(pair => pair.Key)
                .ToList();
            foreach (var nullKey in nullKeys)
                Remove(nullKey);
            return nullKeys.Count;
        }

    }

}