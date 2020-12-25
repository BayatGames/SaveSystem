using System.Linq;

namespace Bayat.Core.Utilities
{

    [System.Serializable]
    public class GuidToReferenceDictionary : SerializableDictionary<string, UnityEngine.Object>
    {

        protected override bool KeysAreEqual(string a, string b)
        {
            return a == b;
        }

        protected override bool ValuesAreEqual(UnityEngine.Object a, UnityEngine.Object b)
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