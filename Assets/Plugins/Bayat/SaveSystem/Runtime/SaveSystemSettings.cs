
using Bayat.Json;
using Bayat.SaveSystem.Security;
using Bayat.SaveSystem.Storage;

namespace Bayat.SaveSystem
{

    /// <summary>
    /// The SaveSystemAPI settings.
    /// </summary>
    public class SaveSystemSettings
    {

        private static readonly object @lock = new object();

        /// <summary>
        /// The default settings.
        /// </summary>
        public static readonly SaveSystemSettings DefaultSettings;

        /// <summary>
        /// The default storage.
        /// </summary>
        public static readonly IStorage DefaultStorage;

        /// <summary>
        /// The default serializer.
        /// </summary>
        public static readonly SaveSystemJsonSerializer DefaultSerializer;

        /// <summary>
        /// The default use encryption.
        /// </summary>
        public static readonly bool DefaultUseEncryption;

        /// <summary>
        /// The default encryption algorithm.
        /// </summary>
        public static readonly ISaveSystemEncryption DefaultEncryptionAlgorithm;

        /// <summary>
        /// The default password.
        /// </summary>
        public static readonly string DefaultPassword;

        /// <summary>
        /// The default json serializer settings.
        /// </summary>
        public static readonly JsonSerializerSettings DefaultJsonSerializerSettings;

        protected IStorage storage = DefaultStorage;
        protected bool useCatalog = true;
        protected bool useMetaData = true;
        protected SaveSystemJsonSerializer serializer = DefaultSerializer;
        protected bool useEncryption = DefaultUseEncryption;
        protected ISaveSystemEncryption encryptionAlgorithm = DefaultEncryptionAlgorithm;
        protected string password = DefaultPassword;

        /// <summary>
        /// Gets the default storage if not set or sets the storage.
        /// </summary>
        public virtual IStorage Storage
        {
            get
            {
                return this.storage ?? DefaultStorage;
            }
            set
            {
                this.storage = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to use meta data or not.
        /// </summary>
        public virtual bool UseMetaData
        {
            get
            {
                return this.useMetaData;
            }
            set
            {
                this.useMetaData = value;
                this.Storage.UseMetaData = this.useMetaData;
            }
        }

        /// <summary>
        /// Gets or sets whether to use catalog or not.
        /// </summary>
        public virtual bool UseCatalog
        {
            get
            {
                return this.useCatalog;
            }
            set
            {
                this.useCatalog = value;
                this.Storage.UseCatalog = this.useCatalog;
            }
        }

        /// <summary>
        /// Gets the default storage if not set or sets the serializer.
        /// </summary>
        public virtual SaveSystemJsonSerializer Serializer
        {
            get
            {
                return this.serializer ?? DefaultSerializer;
            }
            set
            {
                this.serializer = value;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether to use encryption while saving and loading or not.
        /// </summary>
        public virtual bool UseEncryption
        {
            get
            {
                return this.useEncryption;
            }
            set
            {
                this.useEncryption = value;
            }
        }

        /// <summary>
        /// Gets the default encryption algorithm if not set or sets the encryption algorithm.
        /// </summary>
        public virtual ISaveSystemEncryption EncryptionAlgorithm
        {
            get
            {
                return this.encryptionAlgorithm ?? DefaultEncryptionAlgorithm;
            }
            set
            {
                this.encryptionAlgorithm = value;
            }
        }

        /// <summary>
        /// Gets the default password if not set or sets the password.
        /// </summary>
        public virtual string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
            }
        }

        static SaveSystemSettings()
        {
            lock (@lock)
            {
                DefaultStorage = StorageFactory.DefaultStorage;
                DefaultJsonSerializerSettings = JsonSerializerSettingsPreset.DefaultPreset.NewSettings;
                DefaultSerializer = new SaveSystemJsonSerializer(DefaultJsonSerializerSettings);
                DefaultUseEncryption = false;
                DefaultEncryptionAlgorithm = new SaveSystemSymmetricEncryption();
                DefaultPassword = "s@veg@me!12:59";
                if (SaveSystemSettingsPreset.DefaultPreset != null)
                {
                    DefaultSettings = SaveSystemSettingsPreset.DefaultPreset.CustomSettings;
                }
                else
                {
                    DefaultSettings = new SaveSystemSettings()
                    {
                        Storage = DefaultStorage,
                        Serializer = DefaultSerializer,
                        UseEncryption = DefaultUseEncryption,
                        EncryptionAlgorithm = DefaultEncryptionAlgorithm,
                        Password = DefaultPassword
                    };
                }
            }
        }

        public virtual SaveSystemSettings Clone()
        {
            return (SaveSystemSettings)this.MemberwiseClone();
            //SaveSystemSettings newSettings = new SaveSystemSettings();
            //newSettings.EncryptionAlgorithm = this.EncryptionAlgorithm;
            //newSettings.Password = this.Password;
            //newSettings.Serializer = this.Serializer;
            //newSettings.Storage = this.Storage;
            //newSettings.UseCatalog = this.UseCatalog;
            //newSettings.UseEncryption = this.UseEncryption;
            //newSettings.UseMetaData = this.UseMetaData;
            //return newSettings;
        }

    }

}