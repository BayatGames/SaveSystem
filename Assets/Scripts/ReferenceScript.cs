using UnityEngine;

public class ReferenceScript : MonoBehaviour
{

    public GameObject TargetObject;
    public MeshFilter TargetMeshFilter;
    public Transform TargetTransform;

    public void Create()
    {
        GameObject created = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //TargetTransform = created.transform;
        TargetMeshFilter = created.GetComponent<MeshFilter>();
    }

}
