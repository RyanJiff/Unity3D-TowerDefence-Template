using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHandler : MonoBehaviour
{
    /*
     * Base upgradable class
     */
    




    /// <summary>
    /// Upgrade tower
    /// </summary>
    public virtual void Upgrade()
    {
        Debug.Log("Base upgrade");
    }
}
