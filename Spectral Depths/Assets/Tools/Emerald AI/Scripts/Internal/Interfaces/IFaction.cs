using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFaction
{
    /// <summary>
    /// Returns the object's faction index (based on the Faction Data's Faction Name List).
    /// </summary>
    int GetFaction();
}
