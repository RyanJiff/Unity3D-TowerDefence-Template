using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class Entity : MonoBehaviour
{
    /*
     * all spawnable or killable objects must contain an entity component.
     */

    [SerializeField] private string _name  = "Default"; 
    [SerializeField] private string _abrev = "DFLT";
    [SerializeField] private string _description = "This is an entities description.";

    [SerializeField] private float cost = 0;
    [SerializeField] private float health = 1;
    [SerializeField] private int team = 0;

    //DO NOT CHANGE UNLESS MESH IS PROPERLY SET FOR 2x2 GRID AND OFFSET
    [SerializeField] private float placementSize = 0.85f;
    
    /// <summary>
    /// Decreases health by damage value.
    /// </summary>
    public virtual void Damage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Kill();
        }
    }

    /// <summary>
    /// Kills entity
    /// </summary>
    public virtual void Kill()
    {
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Demolishes building and returns 75% of resources.
    /// </summary>
    public virtual void Demolish(ResourceManager rM)
    {
        rM.AddResources(cost * 0.75f);
        Destroy(this.gameObject);
    }


    #region Getters_Setters
    public float GetHealth()
    {
        return health;
    }

    public string GetName()
    {
        return _name;
    }

    public string GetAbreviation()
    {
        return _abrev;
    }

    public string GetDescription()
    {
        return _description;
    }

    public float GetCost()
    {
        return cost;
    }

    public float GetPlacementSize()
    {
        return placementSize;
    }

    public int GetTeam()
    {
        return team;
    }

    public void SetTeam(int t)
    {
        team = t;
    }
    #endregion

}
