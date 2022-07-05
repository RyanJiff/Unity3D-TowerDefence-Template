using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTurretUpgradeHandler : UpgradeHandler
{

    /*
     * Simple turret upgrade increases damage.
     */

    [SerializeField] float upgradeCost = 1;
    [SerializeField] float upgradeDamageIncrease = 1;


    /// <summary>
    /// If we upgraded return the level we upgraded to, otherwise return -1
    /// </summary>
    public override int Upgrade()
    {


        if(GameObject.FindObjectOfType<ResourceManager>().GetResources() >= upgradeCost && level < maxLevel)
        {
            GameObject.FindObjectOfType<ResourceManager>().AddResources(upgradeCost * -1);
            GetComponent<SimpleTurret>().ChangeKineticDamage(upgradeDamageIncrease);
            level++;
            return level;
        }
        else
        {
            Debug.Log("Can't upgrade, not enough money or max level reached!");
            
            return -1;
        }
    }
}
