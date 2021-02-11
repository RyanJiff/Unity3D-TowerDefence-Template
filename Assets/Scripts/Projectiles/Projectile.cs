using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /*
     * Base projectile class
     */

    protected float damage = 1;
    protected Transform target;


    public void SetDamage(float d)
    {
        damage = d;
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Enemy>())
        {
            other.GetComponent<Entity>().Damage(damage);
            Debug.Log("hit for:" + damage);
            Destroy(this.gameObject);
        }
    }

}
