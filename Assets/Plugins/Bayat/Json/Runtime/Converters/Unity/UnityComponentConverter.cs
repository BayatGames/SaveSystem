using System;

namespace Bayat.Json.Converters
{

    public abstract class UnityComponentConverter : UnityObjectConverter
    {

        public override bool IsGenericConverter
        {
            get
            {
                return true;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(UnityEngine.Component).IsAssignableFrom(objectType) && base.CanConvert(objectType);
        }

    }

}