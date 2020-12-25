using System.Text.RegularExpressions;

namespace Bayat.Core.Text
{

    public static class StringUtility
    {

        public static readonly Regex GuidRegex = new Regex(@"[a-fA-F0-9]{8}(\-[a-fA-F0-9]{4}){3}\-[a-fA-F0-9]{12}");

    }

}