using System.Collections.ObjectModel;

namespace Bayat.Core.Profiling
{

    public class ProfiledSegmentCollection : KeyedCollection<string, ProfiledSegment>
    {

        protected override string GetKeyForItem(ProfiledSegment item)
        {
            return item.name;
        }

    }

}