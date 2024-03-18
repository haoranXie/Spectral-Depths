using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmeraldAI;
using EmeraldAI.Utility;
using SpectralDepths.Tools;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(AudioSource))]
public class DamageAIByExplosion : MonoBehaviour
{
    public LayerMask EmeraldAILayer;
    public int DamageAmount = 200;
    public int ExplosionRadius = 4;
    public int ExplosionForce = 400;
    public GameObject ExplosionEffect;
    public AudioClip ExplosionSound;
    GameObject ExplosionSoundObject;
    EmeraldGeneralTargetBridge _emeraldGeneralTargetBridge;
    PLHealthBar _healthBar;
    private void Start()
    {
        //Setup the health tracker to explode on death
        _emeraldGeneralTargetBridge = GetComponent<EmeraldGeneralTargetBridge>();
        if(_emeraldGeneralTargetBridge!=null)
        {
            _emeraldGeneralTargetBridge.OnDeath.AddListener(Explode);
        }

        //Setup the health bar to update health
        _healthBar = GetComponent<PLHealthBar>();
        if (_healthBar != null)
        {
            _healthBar.Initialization();
            _emeraldGeneralTargetBridge.OnTakeDamage.AddListener(UpdateHealthBar);
        }

        ExplosionSoundObject = Resources.Load("Emerald Sound") as GameObject;

    }

    /// <summary>
    /// Call this function when you want to damage surrounding AI based on the set public variables within this script. 
    /// </summary>
    public void Explode ()
    {
        EmeraldObjectPool.SpawnEffect(ExplosionEffect, transform.position, Quaternion.identity, 4);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, EmeraldAILayer);

        foreach (var hitCollider in hitColliders)
        {
            int DamageMitigation = Mathf.RoundToInt((1f - Vector3.Distance(hitCollider.transform.position, transform.position) / ExplosionRadius) * DamageAmount);
            int ForceMitigation = Mathf.RoundToInt((1f - Vector3.Distance(hitCollider.transform.position, transform.position) / ExplosionRadius) * ExplosionForce);
            
            if (hitCollider.GetComponent<IDamageable>() != null)
            {
                hitCollider.GetComponent<IDamageable>().Damage(DamageMitigation, transform, ForceMitigation);
            }
        }
        SpawnExplosionSound();
        gameObject.SetActive(false);
    }

    void SpawnExplosionSound ()
    {
        GameObject SpawnedExplosionSound = EmeraldObjectPool.SpawnEffect(ExplosionSoundObject, transform.position, Quaternion.identity, 3);
        SpawnedExplosionSound.transform.SetParent(EmeraldSystem.ObjectPool.transform);
        AudioSource ExplosionAudioSource = SpawnedExplosionSound.GetComponent<AudioSource>();
        if (ExplosionSound != null) ExplosionAudioSource.PlayOneShot(ExplosionSound);
    }

    void UpdateHealthBar()
    {
        if(_emeraldGeneralTargetBridge!=null)
        {
            _healthBar.UpdateBar(_emeraldGeneralTargetBridge.Health, 0f, _emeraldGeneralTargetBridge.StartHealth, true);
        }
    }
}
