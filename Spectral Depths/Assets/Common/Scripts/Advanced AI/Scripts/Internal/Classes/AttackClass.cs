using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmeraldAI
{
    /// <summary>
    /// Holds all of an AI's attack attack information.
    /// </summary>
    [System.Serializable]
    public class AttackClass
    {
        public int AttackListIndex = 0;
        public AttackPickTypes AttackPickType = AttackPickTypes.Order;
        [SerializeField]
        public List<AttackData> AttackDataList = new List<AttackData>();

        [System.Serializable]
        public class AttackData
        {
            public EmeraldAbilityObject AbilityObject;
            public int AttackAnimation;
            public int AttackOdds = 25;
            public float AttackDistance = 3f;
            public float TooCloseDistance = 1f;
            public bool CooldownIgnored;
            public float CooldownTimeStamp;

            public bool Contains(List<AttackData> m_AttackDataList, AttackData m_AttackDataClass)
            {
                foreach (AttackData AttackInfo in m_AttackDataList)
                {
                    return (AttackInfo == m_AttackDataClass);
                }

                return false;
            }
        }
    }
}