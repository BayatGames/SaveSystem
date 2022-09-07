using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Bayat.SaveSystem.Examples
{

    [CreateAssetMenu(menuName = "Bayat/Save System/Examples/Complex Component Definition")]
    public class ComplexComponentDefinition : ScriptableObject
    {

        public string id;
        public float maxHealth = 100;

    }

}