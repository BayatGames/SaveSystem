using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bayat.SaveSystem.Demos
{

    public class ExistingObjectDemoController : DemoController
    {

        [SerializeField]
        protected InputField identifierField;
        [SerializeField]
        protected GameObject cube;
        [SerializeField]
        protected float speed = 10f;

        private void Update()
        {
            Vector3 position = this.cube.transform.position;
            position.x += Input.GetAxis("Horizontal") * this.speed * Time.deltaTime;
            position.y += Input.GetAxis("Vertical") * this.speed * Time.deltaTime;
            this.cube.transform.position = position;
        }

        public override void Save()
        {
            SaveSystemAPI.SaveAsync(this.identifierField.text, this.cube.transform);
        }

        public override async void Load()
        {
            await SaveSystemAPI.LoadIntoAsync(this.identifierField.text, this.cube.transform);
        }

        public override void Delete()
        {
            SaveSystemAPI.DeleteAsync(this.identifierField.text);
        }

    }

}