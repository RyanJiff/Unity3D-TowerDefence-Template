using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class ResourceManager : MonoBehaviour
{

    /*
     * Manages resources mining and consumption
     */

    [SerializeField] private float resources = 10;

    public float GetResources()
    {
        return resources;
    }

    public void SetResources(float r)
    {
        resources = r;
    }

    /// <summary>
    /// Increases resources by amount.
    /// </summary>
    public void AddResources(float amount)
    {
        resources += amount;
    }

    /// <summary>
    /// Decreases resources by amount.
    /// </summary>
    public void UseResources(float amount)
    {
        resources -= amount;
    }

}
