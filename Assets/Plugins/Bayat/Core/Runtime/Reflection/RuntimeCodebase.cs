using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bayat.Core.Reflection
{

    public static class RuntimeCodebase
    {

        private static readonly object @lock = new object();

        private static readonly List<Type> _types = new List<Type>();

        public static IEnumerable<Type> Types => _types;

        private static readonly List<Assembly> _assemblies = new List<Assembly>();

        public static IEnumerable<Assembly> Assemblies => _assemblies;

        static RuntimeCodebase()
        {
            lock (@lock)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    _assemblies.Add(assembly);

                    foreach (var assemblyType in assembly.GetTypesSafely())
                    {
                        _types.Add(assemblyType);
                    }
                }
            }
        }

    }

}
