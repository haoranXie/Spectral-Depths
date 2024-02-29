using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmeraldAI.Utility;
using System.Linq;

namespace EmeraldAI
{
    [CreateAssetMenu(fileName = "Bullet Projectile Ability", menuName = "Emerald AI/Ability/Bullet Projectile Ability")]
    public class BulletProjectileAbility : EmeraldAbilityObject
    {
        public AbilityData.ChargeSettingsData ChargeSettings;
        public AbilityData.CreateSettingsData CreateSettings;
        public AbilityData.BulletProjectileData BulletProjectileSettings;
        public AbilityData.StunnedData StunnedSettings;
        public AbilityData.DamageData DamageSettings;

        public override void ChargeAbility(GameObject Owner, Transform AttackTransform = null)
        {
            ChargeSettings.SpawnChargeEffect(Owner, AttackTransform);
        }

        public override void InvokeAbility(GameObject Owner, Transform AttackTransform = null) 
        {
            MonoBehaviour OwnerMonoBehaviour = Owner.GetComponent<MonoBehaviour>();
            CreateSettings.SpawnCreateEffect(Owner, AttackTransform);
            OwnerMonoBehaviour.StartCoroutine(SpawnProjectiles(Owner, AttackTransform, BulletProjectileSettings.TimeBetweenBullets));
        }

        IEnumerator SpawnProjectiles (GameObject Owner, Transform AttackTransform, float Delay)
        {
            Transform Target = GetTarget(Owner, AbilityData.TargetTypes.CurrentTarget);

            for (int i = 0; i < BulletProjectileSettings.TotalBullets; i++)
            {
                EmeraldSystem EmeraldComponent = Owner.GetComponent<EmeraldSystem>();
                if (EmeraldComponent != null)
                {
                    if (EmeraldComponent.AnimationComponent.IsDodging || EmeraldComponent.AnimationComponent.IsGettingHit) yield break;
                }

                Vector3 SpawnPosition = AttackTransform.position;
                GameObject SpawnedProjectile = EmeraldObjectPool.Spawn(BulletProjectileSettings.BulletObject, SpawnPosition, BulletProjectileSettings.BulletObject.transform.rotation);
                SpawnedProjectile.transform.localScale = BulletProjectileSettings.BulletObject.transform.localScale;
                SpawnedProjectile.name = BulletProjectileSettings.BulletObject.name;

                AssignScript(SpawnedProjectile).Initialize(Owner, Target, this);

                if (Delay > 0) yield return new WaitForSeconds(Delay);
            }
        }

        /// <summary>
        /// Assign the ProjectileMovement script on the newly spawned projectile.
        /// </summary>
        public BulletProjectile AssignScript(GameObject SpawnedProjectile)
        {
            var bulletProjectile = SpawnedProjectile.GetComponent<BulletProjectile>();
            if (bulletProjectile == null) bulletProjectile = SpawnedProjectile.AddComponent<BulletProjectile>();
            bulletProjectile.enabled = true;
            return bulletProjectile;
        }
    }
}