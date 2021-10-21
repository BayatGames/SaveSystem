using Bayat.Json;
using Bayat.SaveSystem;

using UnityEngine;

public class Test : MonoBehaviour
{

    [JsonIgnore]
    public TestScriptableObject test;
    [JsonProperty]
    protected TestContainer[] containers;

    protected virtual void Awake()
    {
        this.containers = new TestContainer[] {
            new TestContainer(this.test),
            new TestContainer(this.test)
        };
    }

    //public void Destroy()
    //{
    //    Object.Destroy(go);
    //}

    public async void Load()
    {
        containers = await SaveSystemAPI.LoadAsync<TestContainer[]>("go.dat");
    }

    public async void Save()
    {
        await SaveSystemAPI.SaveAsync("go.dat", containers);
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

public class TestContainer
{

    [JsonProperty]
    protected TestScriptableObject testScriptable;

    public TestContainer(TestScriptableObject testScriptable)
    {
        this.testScriptable = testScriptable;
    }

}