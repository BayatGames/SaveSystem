using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// This script will be attached to every track instance
public class TrackInstance : MonoBehaviour
{


    public int prefabId;

    public void ApplyData(TrackInstanceData data)
    {
        transform.position = data.position;
        transform.rotation = data.rotation;
        this.prefabId = data.prefabId;
    }

    public void GetData(TrackInstanceData data)
    {
        data.position = transform.position;
        data.rotation = transform.rotation;
        data.prefabId = this.prefabId;
    }

}