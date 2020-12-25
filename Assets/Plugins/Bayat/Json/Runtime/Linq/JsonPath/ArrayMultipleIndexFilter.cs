using Bayat.Json.Shims;
using System.Collections.Generic;

namespace Bayat.Json.Linq.JsonPath
{
    [Preserve]
    public class ArrayMultipleIndexFilter : PathFilter
    {
        public List<int> Indexes { get; set; }

        public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
        {
            foreach (JToken t in current)
            {
                foreach (int i in Indexes)
                {
                    JToken v = GetTokenIndex(t, errorWhenNoMatch, i);

                    if (v != null)
                    {
                        yield return v;
                    }
                }
            }
        }
    }
}
