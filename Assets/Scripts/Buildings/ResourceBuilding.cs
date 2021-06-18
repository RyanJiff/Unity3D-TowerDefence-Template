using UnityEngine;
[RequireComponent(typeof(Entity))]
public class ResourceBuilding : MonoBehaviour
{
    /*
     *Resource buildings contain this script, it adds to the resource pool once start of round
     */

    ResourceManager resourceManager = null;
    WaveManager waveManager = null;

    [SerializeField] [Tooltip("amount of resource to add to pool when round begins")] float amount = 1.0f;

    private bool resAdded = false;

    private void Start()
    {
        resourceManager = GameObject.FindObjectOfType<ResourceManager>().GetComponent<ResourceManager>();
        waveManager = GameObject.FindObjectOfType<WaveManager>().GetComponent<WaveManager>();
    }

    private void Update()
    {
        //only allow resource gathering during wave
        if (waveManager.IsWaveInProgress())
        {
            if (!resAdded)
            {
                resourceManager.AddResources(amount);
                resAdded = true;
            }

        }
        else
        {
            resAdded = false;
        }
    }

}
