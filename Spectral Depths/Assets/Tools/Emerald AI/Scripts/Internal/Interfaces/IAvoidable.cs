using UnityEngine;

namespace EmeraldAI
{
    /// <summary>
    /// Used to allow AI to detect which objects/abilities they should avoid. 
    /// The AbilityTarget is used to determine which target the ability is intended for.
    /// </summary>
    public interface IAvoidable
    {
        Transform AbilityTarget { get; set; }
    }
}