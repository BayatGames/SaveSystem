using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Bayat.Core
{

    /// <summary>
    /// Used to identify prefabs.
    /// </summary>
    public interface ISavablePrefab
    {

        string PrefabRef { get; set; }

    }

}