using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Entity))]
public class Turret : MonoBehaviour
{
    /*
     * turret superclass, all turret types should inherit from this class.
    */

    [SerializeField] protected Entity currentTarget;

    [SerializeField] protected float range = 5.0f;

    [SerializeField] Transform enemiesParent;

    private void Awake()
    {
        enemiesParent = GameObject.FindGameObjectWithTag("Enemies").transform;
    }

    /// <summary>
    /// call this to find next enemy using tags if we dont have one already.
    /// </summary>
    public void FindNextEnemy()
    {
        if (currentTarget)
        {
            return;
        }
        foreach(Transform t in enemiesParent)
        {
            if(Vector3.Distance(transform.position, t.position) < range)
            {
                currentTarget = t.GetComponent<Entity>();
                return;
            }
        }
    }
}
