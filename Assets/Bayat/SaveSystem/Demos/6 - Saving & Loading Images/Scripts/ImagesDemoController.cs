using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bayat.SaveSystem.Demos
{

    public class ImagesDemoController : DemoController
    {

        [SerializeField]
        protected InputField identifierField;
        [SerializeField]
        protected RawImage image;
        [SerializeField]
        protected Texture2D[] images;

        public override void Save()
        {
            SaveSystemAPI.SaveImagePNGAsync(this.identifierField.text, (Texture2D)this.image.texture);
        }

        public override async void Load()
        {
            this.image.texture = await SaveSystemAPI.LoadImageAsync(this.identifierField.text);
        }

        public override void Delete()
        {
            SaveSystemAPI.DeleteAsync(this.identifierField.text);
        }

        public virtual void RandomizeImage()
        {
            this.image.texture = this.images[Random.Range(0, this.images.Length)];
        }

    }

}