using System.Collections;
using System.Collections.Generic;

using Bayat.SaveSystem.Demos;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bayat.SaveSystem.Examples
{

    public class ComplexRuntimeInstanitatedObjectsExampleController : DemoController
    {

        [SerializeField]
        protected InputField identifierField;
        [SerializeField]
        protected GameObject[] prefabs;
        [SerializeField]
        protected Material[] availableMaterials;
        [SerializeField]
        protected List<GameObject> spawnedInstances = new List<GameObject>();
        [SerializeField]
        protected Transform container;
        [SerializeField]
        protected Camera mainCamera;

        protected virtual void Start()
        {
            if (this.mainCamera == null)
            {
                this.mainCamera = Camera.main;
            }
        }

        protected virtual void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(this.mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo))
                {
                    if (hitInfo.collider.name == "Plane")
                    {
                        SpawnNewObject(hitInfo.point);
                    }
                    else if (hitInfo.collider.GetComponent<MeshRenderer>() != null)
                    {
                        Material sharedMaterial = hitInfo.collider.GetComponent<MeshRenderer>().sharedMaterial;
                        Material randomMaterial = GetRandomMaterial();
                        while (randomMaterial == sharedMaterial)
                        {
                            randomMaterial = GetRandomMaterial();
                        }
                        hitInfo.collider.GetComponent<MeshRenderer>().sharedMaterial = randomMaterial;
                    }
                }
            }
        }

        public override void Save()
        {
            SaveSystemAPI.SaveAsync(this.identifierField.text, this.spawnedInstances);
            Debug.Log("Data saved successfully");
        }

        public override async void Load()
        {
            if (!await SaveSystemAPI.ExistsAsync(this.identifierField.text))
            {
                Debug.Log("Data not found");
                return;
            }
            for (int i = 0; i < this.spawnedInstances.Count; i++)
            {
                DestroyImmediate(this.spawnedInstances[i]);
            }
            this.spawnedInstances.Clear();
            await SaveSystemAPI.LoadIntoAsync(this.identifierField.text, this.spawnedInstances);
            Debug.Log("Data loaded successfully");
        }

        public override void Delete()
        {
            SaveSystemAPI.DeleteAsync(this.identifierField.text);
            Debug.Log("Data deleted successfully");
        }

        public GameObject SpawnNewObject(Vector3 position)
        {
            GameObject instance = Instantiate<GameObject>(GetRandomPrefab(), position, Quaternion.identity, this.container);
            instance.GetComponent<Renderer>().sharedMaterial = GetRandomMaterial();
            this.spawnedInstances.Add(instance);
            return instance;
        }

        public void DestroySpawnedObjects()
        {
            for (int i = 0; i < this.spawnedInstances.Count; i++)
            {
                Destroy(this.spawnedInstances[i]);
            }
            this.spawnedInstances.Clear();
        }

        public GameObject GetRandomPrefab()
        {
            return this.prefabs[Random.Range(0, this.prefabs.Length)];
        }

        public Material GetRandomMaterial()
        {
            return this.availableMaterials[Random.Range(0, this.availableMaterials.Length)];
        }

    }

}