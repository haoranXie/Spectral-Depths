using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmeraldAI
{
    public class LocationBasedDamage : MonoBehaviour
    {
        public List<Collider> IgnoreLineOfSight = new List<Collider>();
        public int LBDComponentsLayer;
        public string LBDComponentsTag = "Untagged";
        EmeraldSystem EmeraldComponent;
        public bool LBDSettingsFoldout = true;
        public bool HideSettingsFoldout;
        [SerializeField]
        public List<LocationBasedDamageClass> ColliderList = new List<LocationBasedDamageClass>();
        [System.Serializable]
        public class LocationBasedDamageClass
        {
            public Collider ColliderObject;
            public float DamageMultiplier = 1;
            public Vector3 BonePosition;
            public Quaternion BoneRotation;

            public LocationBasedDamageClass(Collider m_ColliderObject, int m_DamageMultiplier)
            {
                ColliderObject = m_ColliderObject;
                DamageMultiplier = m_DamageMultiplier;
            }

            public static bool Contains(List<LocationBasedDamageClass> m_LocationBasedDamageList, LocationBasedDamageClass m_LocationBasedDamageClass)
            {
                foreach (LocationBasedDamageClass lbdc in m_LocationBasedDamageList)
                {
                    return (lbdc.ColliderObject == m_LocationBasedDamageClass.ColliderObject);
                }

                return false;
            }
        }

        private void Start()
        {
            InitializeLocationBasedDamage();
        }

        public void InitializeLocationBasedDamage()
        {
            EmeraldComponent = GetComponent<EmeraldSystem>();
            EmeraldComponent.LBDComponent = this;
            EmeraldComponent.AIBoxCollider.size = new Vector3(0.015f, 0.015f, 0.015f);
            EmeraldComponent.AIBoxCollider.center = Vector3.zero;
            EmeraldComponent.AIBoxCollider.center = Vector3.up * transform.localScale.y;
            EmeraldComponent.AIBoxCollider.isTrigger = true;
            EmeraldDetection.LBDLayers |= (1 << LBDComponentsLayer);

            for (int i = 0; i < ColliderList.Count; i++)
            {
                if (ColliderList[i].ColliderObject.GetComponent<Rigidbody>() != null)
                {
                    Rigidbody ColliderRigidbody = ColliderList[i].ColliderObject.GetComponent<Rigidbody>();
                    ColliderRigidbody.useGravity = true;
                    ColliderRigidbody.isKinematic = true;

                    //Cache the position and rotation of each collider. These are used later when reusing an already killed AI.
                    ColliderList[i].BonePosition = ColliderRigidbody.position;
                    ColliderList[i].BoneRotation = ColliderRigidbody.rotation;

                    LocationBasedDamageArea DamageComponent = ColliderList[i].ColliderObject.gameObject.AddComponent<LocationBasedDamageArea>();
                    DamageComponent.EmeraldComponent = EmeraldComponent;
                    DamageComponent.DamageMultiplier = ColliderList[i].DamageMultiplier;

                    EmeraldComponent.DetectionComponent.IgnoredColliders.Add(ColliderList[i].ColliderObject);
                }

                ColliderList[i].ColliderObject.gameObject.layer = LBDComponentsLayer;
                ColliderList[i].ColliderObject.gameObject.tag = LBDComponentsTag;
            }

            for (int i = 0; i < IgnoreLineOfSight.Count; i++)
            {
                IgnoreLineOfSight[i].gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        }

        /// <summary>
        /// Resets the LDB Components (called when an AI is reset).
        /// </summary>
        public void ResetLBDComponents ()
        {
            for (int i = 0; i < ColliderList.Count; i++)
            {
                if (ColliderList[i].ColliderObject.GetComponent<Rigidbody>() != null)
                {
                    StartCoroutine(Reset(ColliderList[i]));
                }
            }
        }

        /// <summary>
        /// Resets the rigidbody and joint components. The helps prevent the ragdoll from becoming unstable after being reused in a different location.
        /// </summary>
        IEnumerator Reset (LocationBasedDamageClass LBDC)
        {
            Rigidbody ColliderRigidbody = LBDC.ColliderObject.GetComponent<Rigidbody>();
            ColliderRigidbody.useGravity = true;
            ColliderRigidbody.isKinematic = true;

            yield return new WaitForSeconds(0.05f);
            ColliderRigidbody.position = LBDC.BonePosition;
            yield return new WaitForSeconds(0.05f);
            ColliderRigidbody.rotation = LBDC.BoneRotation;

            yield return new WaitForSeconds(0.05f);
            Joint ColliderJoint = LBDC.ColliderObject.GetComponent<Joint>();
            ColliderJoint.autoConfigureConnectedAnchor = false;
            yield return new WaitForSeconds(0.05f);
            ColliderJoint.autoConfigureConnectedAnchor = true;

            yield return new WaitForSeconds(0.05f);
            LBDC.ColliderObject.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            LBDC.ColliderObject.gameObject.SetActive(true);
        }
    }
}