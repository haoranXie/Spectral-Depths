using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.TopDown
{
    public class Order : MonoBehaviour
    {
        //Whether or not under this order, a character should hold position without moving
        public bool HoldPosition = false;
        //Whether or not under this order, a character should fight after getting hit
        public bool IgnoreGettingHit = false;
        //Whether or not under this order, a character should target nearby enemies
        public bool AttackNearbyEnemies = false;
		[Tooltip("whether or not the command is targetting a specific character")]
		public bool AttackTargetCharacter = false;
		[Tooltip("reference to the targetted character")]
		public Transform TargetCharacter;
    }
}