using Bayat.SaveSystem;

using UnityEngine;

public class Test : MonoBehaviour
{

    public TestScriptableObject go;

    //public void Destroy()
    //{
    //    Object.Destroy(go);
    //}

    public async void Load()
    {
        go = await SaveSystemAPI.LoadAsync<TestScriptableObject>("go.dat");
    }

    public async void Save()
    {
        await SaveSystemAPI.SaveAsync("go.dat", go);
    }

    //public void CreateDummyObject()
    //{
    //    GameObject parent = new GameObject("Dummy objects");
    //    for (int i = 0; i < 1000; i++)
    //    {
    //        PrimitiveType primitiveType = (PrimitiveType)UnityEngine.Random.Range(0, 6);
    //        GameObject go = GameObject.CreatePrimitive(primitiveType);
    //        go.transform.parent = parent.transform;
    //    }
    //    parent.AddComponent<ReadonlyTest>();
    //    go = parent;
    //}

}