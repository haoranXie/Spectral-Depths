using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmeraldAI;
using EmeraldAI.Utility;

namespace EmeraldAI.Example
{
    /// <summary>
    /// Spawns random blood splatter GameObjects. None are included with Emerald AI, but an open source asset can be found at the following link which was used in Emerald AI's 3.0's demo videos.
    /// Dynamic Decals: https://github.com/EricFreeman/DynamicDecals
    /// </summary>
    [RequireComponent(typeof(EmeraldEvents))]
    public class BloodSplatterManager : MonoBehaviour
    {
        public List<GameObject> BloodEffects = new List<GameObject>();
        public float BloodSpawnDelay = 0;
        public float BloodSpawnRadius = 0.6f;
        public int BloodDespawnTime = 16;
        public int OddsForBlood = 100;
        EmeraldEvents EmeraldEventsComponent;
        EmeraldSystem EmeraldComponent;

        void Start()
        {
            EmeraldEventsComponent = GetComponent<EmeraldEvents>();
            EmeraldComponent = GetComponent<EmeraldSystem>();
            EmeraldEventsComponent.OnTakeDamageEvent.AddListener(() => { CreateBloodSplatter(); });
        }

        public void CreateBloodSplatter()
        {
            Invoke("DelayCreateBloodSplatter", BloodSpawnDelay);
        }

        void DelayCreateBloodSplatter()
        {
            var Odds = Random.Range(0, 101);

            if (Odds <= OddsForBlood && EmeraldComponent != null && !EmeraldComponent.AnimationComponent.IsBlocking)
            {
                GameObject BloodEffect = EmeraldObjectPool.SpawnEffect(BloodEffects[Random.Range(0, BloodEffects.Count)], transform.position + Random.insideUnitSphere * BloodSpawnRadius, Quaternion.identity, BloodDespawnTime);
                BloodEffect.transform.position = new Vector3(BloodEffect.transform.position.x, transform.position.y, BloodEffect.transform.position.z);
                BloodEffect.transform.rotation = Quaternion.AngleAxis(Random.Range(55, 125), Vector3.right) * Quaternion.AngleAxis(Random.Range(10, 350), Vector3.forward);
                BloodEffect.transform.localScale = Vector3.one * Random.Range(0.8f, 1f);
            }
        }
    }
}