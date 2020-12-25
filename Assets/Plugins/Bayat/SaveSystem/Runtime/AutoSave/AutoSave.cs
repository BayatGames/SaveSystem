using Bayat.Json;
using UnityEngine;

namespace Bayat.SaveSystem
{

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    [AddComponentMenu("Bayat/Save System/Auto Save")]
    public class AutoSave : GameObjectSerializationHandler
    {

        [SerializeField]
        protected bool removeOnDisable = true;

        protected virtual void OnEnable()
        {
            AutoSaveManager.Current.AddAutoSave(this);
        }

        protected virtual void OnDisable()
        {
            if (this.removeOnDisable)
            {
                AutoSaveManager.Current.RemoveAutoSave(this);
            }
        }

        protected virtual void OnDestroy()
        {
            AutoSaveManager.Current.RemoveAutoSave(this);
        }

    }

}