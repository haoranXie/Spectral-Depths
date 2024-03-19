using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FOW
{

    //you found an easter egg! this functionality is coming next update!

    public class PartialHider : MonoBehaviour
    {
        public Material HiderMaterial;

        private void Awake()
        {
            HiderMaterial = GetComponent<Renderer>().sharedMaterial;
        }
        private void OnEnable()
        {
            FogOfWarWorld.PartialHiders.Add(this);
        }
        private void OnDisable()
        {
            FogOfWarWorld.PartialHiders.Remove(this);
        }
        void Start()
        {
            FogOfWarWorld.instance.InitializeFogProperties(HiderMaterial);
            FogOfWarWorld.instance.UpdateMaterialProperties(HiderMaterial);
        }
    }

}