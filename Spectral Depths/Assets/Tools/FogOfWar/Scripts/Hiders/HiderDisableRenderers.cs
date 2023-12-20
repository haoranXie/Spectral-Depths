using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FOW
{
    public class HiderDisableRenderers : HiderBehavior
    {
        public Renderer[] ObjectsToHide;

        protected override void OnHide()
        {
            foreach (Renderer renderer in ObjectsToHide)
                renderer.enabled = false;
        }

        protected override void OnReveal()
        {
            foreach (Renderer renderer in ObjectsToHide)
                renderer.enabled = true;
        }
    }
}