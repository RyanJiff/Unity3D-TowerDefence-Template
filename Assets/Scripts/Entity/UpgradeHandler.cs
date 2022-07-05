using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHandler : MonoBehaviour
{
    /*
     * Base upgradable class
     */

    [SerializeField] protected int level = 1;
    [SerializeField] protected int maxLevel = 3;


    /// <summary>
    /// If we upgraded return the level we upgraded to, otherwise return -1
    /// </summary>
    public virtual int Upgrade()
    {
        return -1;
    }
}
