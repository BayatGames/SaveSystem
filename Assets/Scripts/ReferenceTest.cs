using Bayat.SaveSystem;

using UnityEngine;

public class ReferenceTest : MonoBehaviour
{

    public string Tag = "Player";
    public GameObject TargetObject = null;

    private void OnEnable()
    {
        AutoSaveManager.Current.Loaded += AutoSaveManager_Loaded;
        Find();
    }

    private void OnDisable()
    {
        AutoSaveManager.Current.Loaded -= AutoSaveManager_Loaded;
    }

    private void AutoSaveManager_Loaded(object sender, System.EventArgs e)
    {
        Find();
    }

    public void Find()
    {
        TargetObject = GameObject.FindWithTag(Tag);
    }

    public void Destroy()
    {
        if (TargetObject == null) return;

        Destroy(TargetObject);
    }

}
