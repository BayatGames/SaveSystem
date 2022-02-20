using System.Collections.Generic;
using System.Linq;

using Bayat.Json;
using Bayat.SaveSystem;

using TMPro;

using UnityEngine;

public class Test : MonoBehaviour
{

    void Start()
    {
        var text = GetComponent<TextMeshPro>();
        var meshRenderer = GetComponent<MeshRenderer>();
        if (text != null)
        {

            Debug.Log(text.fontMaterial);
        }
    }

}