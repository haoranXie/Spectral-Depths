using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Tools
{
    [ExecuteInEditMode]
    public class ReassignBoneWeightsToNewMesh : MonoBehaviour
    {
        public Transform newArmature;
        public string rootBoneName = "Hips";
        public bool PressToReassign;

        void Update()
        {
            if (PressToReassign)
                Reassign();
            PressToReassign = false;
        }

        // [ContextMenu("Reassign Bones")]
        public void Reassign()
        {
            if (newArmature == null)
            {
                Debug.Log("No new armature assigned");
                return;
            }

            if (newArmature.Find(rootBoneName) == null)
            {
                Debug.Log("Root bone not found");
                return;
            }

            Debug.Log("Reassigning bones");
            SkinnedMeshRenderer rend = gameObject.GetComponent<SkinnedMeshRenderer>();
            Transform[] bones = new Transform[rend.bones.Length];

            for (int i = 0; i < bones.Length; i++)
            {
                for (int a = 0; a < newArmature.childCount; a++)
                {
                    if (rend.bones[i].name == newArmature.GetChild(a).name)
                    {
                        bones[i] = newArmature.GetChild(a);
                        break;
                    }
                }
            }

            rend.rootBone = newArmature.Find(rootBoneName);
            rend.bones = bones;
        }
    }
}
