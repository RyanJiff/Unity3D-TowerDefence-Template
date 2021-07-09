using UnityEngine;

public class DestroyAfter : MonoBehaviour
{

    public float destroyAfterTime = 1f;
    
    void Start()
    {
        Destroy(this.gameObject, destroyAfterTime);
    }

}
