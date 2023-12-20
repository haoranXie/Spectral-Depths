using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FOW
{
    public class FogOfWarHider : MonoBehaviour
    {
        public Transform[] samplePoints;
        [HideInInspector] public float maxDistBetweenPoints;

        [HideInInspector] public int seenCount;
        [HideInInspector] public List<FogOfWarRevealer> seenBy = new List<FogOfWarRevealer>();

        private void OnEnable()
        {
            CalculateSamplePointData();
            RegisterHider();
        }
        private void OnDisable()
        {
            SetActive(true);
            DeregisterHider();
        }

        void CalculateSamplePointData()
        {
            if (samplePoints.Length == 0)
            {
                samplePoints = new Transform[1];
                samplePoints[0] = transform;
            }
            maxDistBetweenPoints = 0;
            for (int i = 0; i < samplePoints.Length; i++)
            {
                for (int j = i; j < samplePoints.Length; j++)
                {
                    maxDistBetweenPoints = Mathf.Max(maxDistBetweenPoints, Vector3.Distance(samplePoints[i].position, samplePoints[j].position));
                }
            }
        }

        public void RegisterHider()
        {
            //OnActiveChanged += callbackTest;
            if (!FogOfWarWorld.hiders.Contains(this))
            {
                FogOfWarWorld.hiders.Add(this);
                FogOfWarWorld.numHiders++;
                SetActive(false);
            }
        }

        public void DeregisterHider()
        {
            //OnActiveChanged -= callbackTest;
            if (FogOfWarWorld.hiders.Contains(this))
            {
                FogOfWarWorld.hiders.Remove(this);
                FogOfWarWorld.numHiders--;
                foreach (FogOfWarRevealer revealer in seenBy)
                {
                    revealer.hidersSeen.Remove(this);
                }
                seenCount = 0;
                seenBy.Clear();
            }
        }

        /// <summary>
        /// example method to show how to use the callback.
        /// </summary>
        void CallbackTest(bool isActive)
        {
            Debug.Log(isActive);
        }

        public void AddSeer(FogOfWarRevealer seer)   //see-er?
        {
            seenBy.Add(seer);
            if (seenCount == 0)
            {
                SetActive(true);
            }
            seenCount++;
        }
        public void RemoveSeer(FogOfWarRevealer seer)   //see-er?
        {
            seenBy.Remove(seer);
            seenCount--;
            if (seenCount == 0)
            {
                SetActive(false);
            }
        }

        public delegate void OnChangeActive(bool isActive);
        public event OnChangeActive OnActiveChanged;
        void SetActive(bool isActive)
        {
            OnActiveChanged?.Invoke(isActive);
        }
    }
}