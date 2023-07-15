using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace Bayat.SaveSystem.Storage
{

    /// <summary>
    /// The connection string factory.
    /// </summary>
    public static class ConnectionStringFactory
    {

        private static readonly object @lock = new object();

        private const string TypeSeparator = "://";
        private static readonly List<IConnectionFactory> Factories = new List<IConnectionFactory>();

        static ConnectionStringFactory()
        {
            lock (@lock)
            {
                List<IConnectionFactory> factories = GetConnectionFactories();
                foreach (IConnectionFactory factory in factories)
                {
                    Register(factory);
                }
            }
        }

        /// <summary>
        /// Retrieves all connection factories using reflection.
        /// </summary>
        /// <returns></returns>
        public static List<IConnectionFactory> GetConnectionFactories()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<IConnectionFactory> factories = new List<IConnectionFactory>();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Assembly assembly = assemblies[i];
                Type[] types = assembly.GetLoadableTypes().ToArray();
                for (int j = 0; j < types.Length; j++)
                {
                    Type type = types[j];
                    if (type.IsAbstract || !type.IsVisible)
                    {
                        continue;
                    }
                    if (typeof(IConnectionFactory).IsAssignableFrom(type))
                    {
                        factories.Add((IConnectionFactory)Activator.CreateInstance(type));
                    }
                }
            }
            return factories;
        }

        private static Type[] GetLoadableTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null).ToArray();
            }
        }

        /// <summary>
        /// Registers a new connection string factory.
        /// </summary>
        /// <param name="factory">The connection string factory</param>
        public static void Register(IConnectionFactory factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            if (Factories.Contains(factory))
            {
                return;
            }

            Factories.Add(factory);
        }

        /// <summary>
        /// Creates a new instance of <see cref="IStorage"/> implementation using the connection string.
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <returns>A new instance of <see cref="IStorage"/> implementation if possible otherwise null</returns>
        public static IStorage CreateStorage(string connectionString)
        {
            return Create(connectionString, (factory, cs) => factory.CreateStorage(cs));
        }


        private static TInstance Create<TInstance>(string connectionString, Func<IConnectionFactory, StorageConnectionString, TInstance> createAction)
           where TInstance : class
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            connectionString = string.Format(connectionString,
                Application.persistentDataPath,
                Application.dataPath,
                Application.streamingAssetsPath,
                Application.temporaryCachePath,
                Application.absoluteURL,
                Application.buildGUID,
                Application.companyName,
                Application.productName,
                Application.identifier,
                Application.version,
                Application.unityVersion);
            var pcs = new StorageConnectionString(connectionString);

            TInstance instance = Factories
               .Select(f => createAction(f, pcs))
               .FirstOrDefault(b => b != null);

            if (instance == null)
            {
                throw new ArgumentException(
                   $"could not create any implementation based on the passed connection string (prefix: {pcs.Prefix}).",
                   nameof(connectionString));
            }

            return instance;
        }

    }

}