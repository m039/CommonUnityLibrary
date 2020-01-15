using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common 
{
    [ExecuteInEditMode]
	public class RoundedRectangle: MonoBehaviour 
	{
        private static readonly int BorderColorID = Shader.PropertyToID("_BorderColor");

        private static readonly int BorderSizeID = Shader.PropertyToID("_BorderSize");

        private static readonly int BorderRadiusID = Shader.PropertyToID("_BorderRadius");

        #region Inspector

        public Color borderColor = Color.black;

        public float borderSize = 0;

        public float borderRadius = 0;

        [SerializeField]
        Material _material;

        #endregion

        void OnRectTransformDimensionsChange()
        {
            Refresh();
        }

        void OnValidate()
        {
            Refresh();
        }

        void Refresh()
        {
            if (_material != null)
            {
                _material.SetColor(BorderColorID, borderColor);
                _material.SetFloat(BorderSizeID, borderSize);
                _material.SetFloat(BorderRadiusID, borderRadius);
            }
        }

	}

}