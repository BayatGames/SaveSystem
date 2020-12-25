using System;
using System.Collections.Generic;
using System.Text;

namespace Bayat.SaveSystem.Storage
{

    /// <summary>
    /// Holds a parsed connection string to the storage
    /// </summary>
    public class StorageConnectionString
    {

        public static string NativeParameterName = "native";

        private const string PrefixSeparator = "://";
        private static readonly char[] PartsSeparators = new[] { ';' };
        private static readonly char[] PartSeparator = new[] { '=' };

        private readonly Dictionary<string, string> _parts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private string _nativeConnectionString;

        /// <summary>
        /// Creates a new instance of <see cref="StorageConnectionString"/>
        /// </summary>
        /// <param name="connectionString"></param>
        public StorageConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            ConnectionString = connectionString;

            Parse(connectionString);
        }

        /// <summary>
        /// Gets or sets connection string parameters
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                if (key == null)
                {
                    return null;
                }
                string value;
                _parts.TryGetValue(key, out value);
                return value;
            }
            set
            {
                if (key == null)
                    return;

                _parts[key] = value;
            }
        }

        /// <summary>
        /// Determines if this is a native connection string
        /// </summary>
        public bool IsNative => _nativeConnectionString != null;

        /// <summary>
        /// Returns native connection string, or null if connection string is not native
        /// </summary>
        public string Native => _nativeConnectionString;

        /// <summary>
        /// Original connection string
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Connection string parameters exposed as key-value pairs
        /// </summary>
        public Dictionary<string, string> Parameters => _parts;

        /// <summary>
        /// Prefix of this connection string, excluding prefix separator, i.e. for 'disk://something' the prefix is 'disk'
        /// </summary>
        public string Prefix { get; private set; }

        /// <summary>
        /// Gets the value of the parameter as when it's required. When parameter is not present, throws standard <see cref="ArgumentException"/>
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="requireNonEmptyValue">When true, checks that parameter value is not null or empty and throws <see cref="ArgumentException"/></param>
        /// <param name="value">Result value</param>
        public void GetRequired(string parameterName, bool requireNonEmptyValue, out string value, params string[] aliases)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            if (!_parts.TryGetValue(parameterName, out value))
            {
                bool found = false;
                if (aliases != null)
                {
                    foreach (string alias in aliases)
                    {
                        found = _parts.TryGetValue(alias, out value);
                        if (found)
                        {
                            break;
                        }
                    }
                }
                if (!found)
                {
                    {
                        throw new ArgumentException($"connection string requires '{parameterName}' parameter.");
                    }
                }

                if (requireNonEmptyValue && string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException($"parameter '{parameterName}' is present but value is not set.");
                }
            }
        }

        /// <summary>
        /// Gets the value of the parameter as when it's required. When parameter is not present, throws standard <see cref="ArgumentException"/>
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="requireNonEmptyValue">When true, checks that parameter value is not null or empty and throws <see cref="ArgumentException"/></param>
        /// <param name="value">Result value</param>
        public void GetRequiredBoolean(string parameterName, bool requireNonEmptyValue, out bool value, params string[] aliases)
        {
            string stringValue;
            GetRequired(parameterName, requireNonEmptyValue, out stringValue, aliases);
            value = bool.Parse(stringValue);
        }

        /// <summary>
        /// Gets the value of the parameter as when it's required. When parameter is not present, throws standard <see cref="ArgumentException"/>
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="requireNonEmptyValue">When true, checks that parameter value is not null or empty and throws <see cref="ArgumentException"/></param>
        /// <param name="value">Result value</param>
        public void GetRequiredInteger(string parameterName, bool requireNonEmptyValue, out int value, params string[] aliases)
        {
            string stringValue;
            GetRequired(parameterName, requireNonEmptyValue, out stringValue, aliases);
            value = int.Parse(stringValue);
        }

        /// <summary>
        /// Gets the value of the parameter as when it's required. When parameter is not present, throws standard <see cref="ArgumentException"/>
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="requireNonEmptyValue">When true, checks that parameter value is not null or empty and throws <see cref="ArgumentException"/></param>
        /// <param name="value">Result value</param>
        public void GetRequiredFloat(string parameterName, bool requireNonEmptyValue, out float value, params string[] aliases)
        {
            string stringValue;
            GetRequired(parameterName, requireNonEmptyValue, out stringValue, aliases);
            value = float.Parse(stringValue);
        }

        /// <summary>
        /// Get connection string parameter by name
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="defaultValue"></param>
        /// <returns>Parameter value. If parameter is not set returns an empty string</returns>
        public string Get(string parameterName, string defaultValue, params string[] aliases)
        {
            if (parameterName == null) return defaultValue;
            string value;
            if (!_parts.TryGetValue(parameterName, out value))
            {
                if (aliases != null)
                {
                    foreach (string alias in aliases)
                    {
                        if (_parts.TryGetValue(alias, out value))
                        {
                            return value;
                        }
                    }
                }
                return defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Get connection string parameter by name
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="defaultValue"></param>
        /// <returns>Parameter value. If parameter is not set returns an empty string</returns>
        public bool GetBoolean(string parameterName, bool defaultValue, params string[] aliases)
        {
            return bool.Parse(Get(parameterName, defaultValue.ToString(), aliases));
        }

        /// <summary>
        /// Get connection string parameter by name
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="defaultValue"></param>
        /// <returns>Parameter value. If parameter is not set returns an empty string</returns>
        public int GetInteger(string parameterName, int defaultValue, params string[] aliases)
        {
            return int.Parse(Get(parameterName, defaultValue.ToString(), aliases));
        }

        /// <summary>
        /// Get connection string parameter by name
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="defaultValue"></param>
        /// <returns>Parameter value. If parameter is not set returns an empty string</returns>
        public float GetFloat(string parameterName, float defaultValue, params string[] aliases)
        {
            return float.Parse(Get(parameterName, defaultValue.ToString(), aliases));
        }

        private void Parse(string connectionString)
        {
            int idx = connectionString.IndexOf(PrefixSeparator);

            if (idx == -1)
            {
                Prefix = connectionString;
                return;
            }

            Prefix = connectionString.Substring(0, idx);
            connectionString = connectionString.Substring(idx + PrefixSeparator.Length);

            // prefix extracted, now get the parts of the string

            //check if this is a native connection string
            if (connectionString.StartsWith(NativeParameterName + "="))
            {
                _nativeConnectionString = connectionString.Substring(NativeParameterName.Length + 1);
                _parts[NativeParameterName] = _nativeConnectionString;
            }
            else
            {
                string[] parts = connectionString.Split(PartsSeparators, StringSplitOptions.RemoveEmptyEntries);
                foreach (string part in parts)
                {
                    string[] kv = part.Split(PartSeparator, 2);

                    string key = kv[0];
                    string value = kv.Length == 1 ? string.Empty : kv[1];
                    _parts[key] = value;
                }
            }
        }

        /// <summary>
        /// Returns a string representation of the connection string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Prefix);
            sb.Append(PrefixSeparator);

            if (IsNative)
            {
                sb.Append(NativeParameterName);
                sb.Append(PartSeparator);
                sb.Append(Native);
            }
            else
            {

                bool first = true;
                foreach (KeyValuePair<string, string> pair in _parts)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(PartsSeparators);
                        first = false;
                    }

                    sb.Append(pair.Key);
                    sb.Append(PartSeparator);
                    sb.Append(pair.Value);
                }
            }

            return sb.ToString();
        }

    }

}