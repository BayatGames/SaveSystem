using Bayat.Core;
using Bayat.Json;

using UnityEngine;

namespace Bayat.SaveSystem
{

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    [AddComponentMenu("Bayat/Save System/Auto Save Prefab")]
    public class AutoSavePrefab : AutoSave, ISavablePrefab
    {

        [SerializeField]
        protected string prefabRef;

        public virtual string PrefabRef
        {
            get => this.prefabRef;
            set => this.prefabRef = value;
        }

    }

}