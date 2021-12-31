using System.Collections.Generic;
using System.Linq;

using Bayat.Json;
using Bayat.SaveSystem;

using UnityEngine;

public class Test : MonoBehaviour
{

    [JsonIgnore]
    public TestScriptableObject test;
    [JsonProperty]
    protected TestContainer[] containers;

    protected Dictionary<Vector2Int, int> dic = new Dictionary<Vector2Int, int>();

    protected virtual void Awake()
    {
        this.containers = new TestContainer[] {
            new TestContainer(this.test),
            new TestContainer(this.test)
        };
        this.dic.Add(new Vector2Int(2, 3), 23);
        this.dic.Add(new Vector2Int(2, 4), 23);
        this.dic.Add(new Vector2Int(2, 5), 23);
        this.dic.Add(new Vector2Int(2, 6), 23);
    }

    void Start()
    {
        Save();
    }

    //public void Destroy()
    //{
    //    Object.Destroy(go);
    //}

    public async void Load()
    {
        //containers = await SaveSystemAPI.LoadAsync<TestContainer[]>("go.dat");
        var data = await SaveSystemAPI.LoadAsync<List<KeyValuePair<Vector2Int, int>>>("dic.dat");
        this.dic.Clear();
        this.dic = data.ToDictionary(x => x.Key, x => x.Value);
        foreach (var item in dic)
        {
            Debug.Log(item.Key);
            Debug.Log(item.Value);
        }
    }

    public async void Save()
    {
        //await SaveSystemAPI.SaveAsync("go.dat", containers);
        await SaveSystemAPI.SaveAsync("dic.dat", this.dic.ToList());
        Load();
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