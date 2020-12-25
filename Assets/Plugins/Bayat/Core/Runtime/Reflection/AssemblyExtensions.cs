using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Bayat.Core.Reflection
{

    public static class AssemblyExtensions
    {

        public static IEnumerable<Type> GetTypesSafely(this Assembly assembly)
        {
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to load types in assembly '{assembly}'.\n{ex}");

                yield break;
            }

            foreach (var type in types)
            {
                if (type == typeof(void))
                {
                    continue;
                }

                yield return type;
            }
        }

    }

}