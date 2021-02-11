using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : Projectile
{
    /*
     * basic projectile that goes towards a point
     */

    [SerializeField] float velocity = 10;

    private void Update()
    {
        transform.Translate(transform.forward * velocity * Time.deltaTime,Space.World);
    }

}
