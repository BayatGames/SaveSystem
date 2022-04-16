using System.Collections;
using System.Collections.Generic;

using Bayat.SaveSystem;

using UnityEngine;
using UnityEngine.EventSystems;

public class TrackBuilder : MonoBehaviour
{

    public new Camera camera;
    public List<TrackInstance> tracksPrefabs = new List<TrackInstance>();
    public List<TrackInstance> tracks = new List<TrackInstance>();

    public string identifier = "tracks.dat";
    public TrackData data = new TrackData();

    public int selectedPrefabId = 0;

    public int SelectedPrefabId
    {
        get
        {
            return this.selectedPrefabId;
        }
        set
        {
            this.selectedPrefabId = value;
        }
    }

    public void SetSelectedPrefabId(float id)
    {
        this.selectedPrefabId = (int)id;
    }

    void Start()
    {

    }

    void Reset()
    {
        this.camera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            var ray = this.camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                var instance = CreateNewTrack(this.selectedPrefabId, hitInfo.point, Quaternion.identity);
                this.tracks.Add(instance);
            }
        }
    }

    public TrackInstance CreateTrack(int id, Vector3 position, Quaternion rotation)
    {
        var instance = CreateNewTrack(id, position, rotation);
        this.tracks.Add(instance);

        // Add the data to the tracks data list
        // Note that if you want the user to be able to change the tracks position after creating it, you might want to update the position associated with it before saving
        var instanceData = new TrackInstanceData();
        instance.GetData(instanceData);
        this.data.tracks.Add(instanceData);

        return instance;
    }

    public TrackInstance CreateNewTrack(int id, Vector3 position, Quaternion rotation)
    {
        if (this.tracksPrefabs.Count <= id)
        {

            // You can use a fallback approach here instead of throwing an exception if you change the amount of prefabs constantly
            throw new System.ArgumentException("The prefab id is smaller than the currently available prefabs");
        }
        var prefab = this.tracksPrefabs[id];
        var instance = Instantiate(prefab, position, rotation);
        instance.prefabId = id;

        return instance;
    }

    public async void Save()
    {
        for (int i = 0; i < this.tracks.Count; i++)
        {
            var trackInstance = this.tracks[i];
            if (trackInstance == null)
            {
                if (this.data.tracks.Count > i)
                {
                    this.data.tracks[i] = null;
                }
                continue;
            }
            TrackInstanceData trackInstanceData;
            if (this.data.tracks.Count <= i)
            {
                trackInstanceData = new TrackInstanceData();
                this.data.tracks.Add(trackInstanceData);
            }
            else
            {
                trackInstanceData = this.data.tracks[i];
            }
            trackInstanceData.callback = (a) =>
            {
                Debug.Log(a);
            };
            trackInstance.GetData(trackInstanceData);
        }
        await SaveSystemAPI.SaveAsync(this.identifier, this.data);
    }

    public async void Load()
    {
        if (await SaveSystemAPI.ExistsAsync(this.identifier))
        {
            var loadedData = await SaveSystemAPI.LoadAsync<TrackData>(this.identifier);

            // Loop through the loaded data tracks and merge the loaded data with the existing tracks or create new ones and apply the data
            for (int i = 0; i < loadedData.tracks.Count; i++)
            {
                var trackInstanceData = loadedData.tracks[i];
                TrackInstance trackInstance;

                // Check whether there are existing tracks in the scene, if not then create a new track for every data entry
                if (this.tracks.Count <= i)
                {

                    // Create a new track instance if there are not enough tracks available
                    trackInstance = CreateTrack(trackInstanceData.prefabId, trackInstanceData.position, trackInstanceData.rotation);
                }
                else if (this.tracks[i] == null)
                {
                    trackInstance = CreateNewTrack(trackInstanceData.prefabId, trackInstanceData.position, trackInstanceData.rotation);
                    this.tracks.Insert(i, trackInstance);
                }
                else
                {
                    trackInstance = this.tracks[i];
                }
                trackInstance.ApplyData(trackInstanceData);
            }
        }
    }

}

public class TrackData
{

    public List<TrackInstanceData> tracks = new List<TrackInstanceData>();

}

public class TrackInstanceData
{

    public delegate void TestHandler(int amonut);

    public TestHandler callback;

    // You can use a prefab identifier if you have multiple prefabs for tracks
    public int prefabId;
    public Vector3 position = Vector3.zero;
    public Quaternion rotation = Quaternion.identity;

}