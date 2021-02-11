using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "wave set data", menuName = "Waves/WaveSet", order = 1)]
public class WaveSet : ScriptableObject
{
    /*
    *set of waves data object
    */

    public List<Wave> waves = new List<Wave>();

    /// <summary>
    /// returns number of waves in the set
    /// </summary>
    public int GetTotalWaveCount()
    {
        return waves.Count;
    }
}
