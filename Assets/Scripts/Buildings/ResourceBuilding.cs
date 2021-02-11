using UnityEngine;
[RequireComponent(typeof(Entity))]
public class ResourceBuilding : MonoBehaviour
{
    /*
     *Resource buildings contain this script, it adds to the resource pool once every X time
     */

    ResourceManager resourceManager = null;
    WaveManager waveManager = null;

    [SerializeField] [Tooltip("How long in seconds before we add resource to pool")] float resourceTimer = 1.0f;
    [SerializeField] [Tooltip("amount of resource to add to pool")] float amount = 1.0f;

    private float timer = 0;

    private void Start()
    {
        resourceManager = GameObject.FindObjectOfType<ResourceManager>().GetComponent<ResourceManager>();
        waveManager = GameObject.FindObjectOfType<WaveManager>().GetComponent<WaveManager>();
        timer = resourceTimer;
    }

    private void Update()
    {
        //only allow resource gathering during wave
        if (waveManager.IsWaveInProgress())
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer = resourceTimer;
                resourceManager.AddResources(amount);
            }
        }
    }

}
