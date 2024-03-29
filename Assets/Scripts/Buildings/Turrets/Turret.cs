﻿using System.Linq;
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
    [SerializeField] protected List<Entity> targetsInRadius = new List<Entity>();
    [SerializeField] protected float range = 5.0f;

    Transform enemiesParent;

    private void Awake()
    {
        enemiesParent = GameObject.FindGameObjectWithTag("Enemies").transform;
    }

    /// <summary>
    /// call this to find next enemy using the enemies parent game object if we dont have one already. 
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

    /// <summary>
    /// call this to get a list of enemies within radius who are not on our team
    /// </summary>
    public List<Enemy> FindTargetsInRadius(float radius)
    {

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        List<Enemy> enemies = new List<Enemy>();

        for(int i = 0; i < colliders.Length; i++)
        {
            //check if we are on the same team or not
            if (colliders[i].GetComponent<Enemy>())
            {
                enemies.Add(colliders[i].GetComponent<Enemy>());
            }
        }

        return enemies;
    }

}
