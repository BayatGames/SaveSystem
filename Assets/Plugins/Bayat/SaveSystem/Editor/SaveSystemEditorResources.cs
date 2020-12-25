using UnityEngine;

namespace Bayat.SaveSystem
{

    public static class SaveSystemEditorResources
    {

        public static readonly Texture IsometricIcon;
        public static readonly Texture FlatIcon;
        public static readonly Texture LogoIcon;

        static SaveSystemEditorResources()
        {
            IsometricIcon = Resources.Load<Texture>("Bayat/SaveSystem/Editor/IsometricIcon");
            FlatIcon = Resources.Load<Texture>("Bayat/SaveSystem/Editor/FlatIcon");
            LogoIcon = Resources.Load<Texture>("Bayat/SaveSystem/Editor/LogoIcon");
        }

    }

}