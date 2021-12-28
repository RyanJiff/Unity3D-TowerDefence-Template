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

    public override void Upgrade()
    {
        base.Upgrade();
        Debug.Log("Simple turret upgrade");
        if(GameObject.FindObjectOfType<ResourceManager>().GetResources() >= upgradeCost)
        {
            GameObject.FindObjectOfType<ResourceManager>().AddResources(upgradeCost * -1);
            GetComponent<SimpleTurret>().ChangeKineticDamage(upgradeDamageIncrease);
        }
    }
}
