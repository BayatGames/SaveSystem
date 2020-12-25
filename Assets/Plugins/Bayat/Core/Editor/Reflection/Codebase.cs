using Bayat.Core.Profiling;
using Bayat.Core.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using UnityEngine;

namespace Bayat.Core.Reflection
{

    public static class Codebase
    {

        static Codebase()
        {
            using (ProfilingUtility.SampleBlock("Bayat - Codebase initialization"))
            {
                _assemblies = new List<Assembly>();
                _runtimeAssemblies = new List<Assembly>();
                _editorAssemblies = new List<Assembly>();

                _types = new List<Type>();
                _runtimeTypes = new List<Type>();
                _editorTypes = new List<Type>();

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    _assemblies.Add(assembly);

                    var isRuntimeAssembly = IsRuntimeAssembly(assembly);
                    var isEditorAssembly = IsEditorDependentAssembly(assembly);

                    if (isRuntimeAssembly)
                    {
                        _runtimeAssemblies.Add(assembly);
                    }

                    if (isEditorAssembly)
                    {
                        _editorAssemblies.Add(assembly);
                    }

                    foreach (var type in assembly.GetTypesSafely())
                    {
                        _types.Add(type);

                        if (isRuntimeAssembly)
                        {
                            _runtimeTypes.Add(type);
                        }

                        if (isEditorAssembly)
                        {
                            _editorTypes.Add(type);
                        }
                    }
                }

                Assemblies = _assemblies.AsReadOnly();
                RuntimeAssemblies = _runtimeAssemblies.AsReadOnly();
                EditorAssemblies = _editorAssemblies.AsReadOnly();

                Types = _types.AsReadOnly();
                RuntimeTypes = _runtimeTypes.AsReadOnly();
                EditorTypes = _editorTypes.AsReadOnly();
            }
        }

        private static readonly List<Assembly> _assemblies;
        private static readonly List<Assembly> _runtimeAssemblies;
        private static readonly List<Assembly> _editorAssemblies;
        private static readonly List<Type> _types;
        private static readonly List<Type> _runtimeTypes;
        private static readonly List<Type> _editorTypes;

        // NETUP: IReadOnlyCollection

        public static ReadOnlyCollection<Assembly> Assemblies { get; private set; }

        public static ReadOnlyCollection<Assembly> RuntimeAssemblies { get; private set; }

        public static ReadOnlyCollection<Assembly> EditorAssemblies { get; private set; }

        public static ReadOnlyCollection<Type> Types { get; private set; }

        public static ReadOnlyCollection<Type> RuntimeTypes { get; private set; }

        public static ReadOnlyCollection<Type> EditorTypes { get; private set; }

        private static bool IsEditorAssembly(AssemblyName assemblyName)
        {
            var name = assemblyName.Name;

            return
                name == "Assembly-CSharp-Editor" ||
                name == "Assembly-CSharp-Editor-firstpass" ||
                name == "UnityEditor";
        }

        private static bool IsUserAssembly(AssemblyName assemblyName)
        {
            var name = assemblyName.Name;

            return
                name == "Assembly-CSharp" ||
                name == "Assembly-CSharp-firstpass";
        }

        private static bool IsUserAssembly(Assembly assembly)
        {
            return IsUserAssembly(assembly.GetName());
        }

        private static bool IsEditorAssembly(Assembly assembly)
        {
            if (Attribute.IsDefined(assembly, typeof(AssemblyIsEditorAssembly)))
            {
                return true;
            }

            return IsEditorAssembly(assembly.GetName());
        }

        private static bool IsRuntimeAssembly(Assembly assembly)
        {
            // User assemblies refer to the editor when they include
            // a using UnityEditor / #if UNITY_EDITOR, but they should still
            // be considered runtime.
            return IsUserAssembly(assembly) || !IsEditorDependentAssembly(assembly);
        }

        private static bool IsEditorDependentAssembly(Assembly assembly)
        {
            if (IsEditorAssembly(assembly))
            {
                return true;
            }

            foreach (var dependency in assembly.GetReferencedAssemblies())
            {
                if (IsEditorAssembly(dependency))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsEditorType(Type type)
        {
            var rootNamespace = type.RootNamespace();

            return IsEditorAssembly(type.Assembly) ||
                   rootNamespace == "UnityEditor" ||
                   rootNamespace == "UnityEditorInternal";
        }

        public static bool IsInternalType(Type type)
        {
            var rootNamespace = type.RootNamespace();

            return rootNamespace == "UnityEngineInternal" ||
                   rootNamespace == "UnityEditorInternal";
        }

        public static bool IsRuntimeType(Type type)
        {
            return !IsEditorType(type) && !IsInternalType(type);
        }

        private static string RootNamespace(this Type type)
        {
            return type.Namespace?.PartBefore('.');
        }

    }

}