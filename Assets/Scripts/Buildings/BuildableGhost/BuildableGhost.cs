using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuildableGhost : MonoBehaviour
{
    /*
     * used to check if spot in is valid before placing a building
     */

    public LayerMask layerMask;
    [SerializeField] bool canBuild = false;

    private Renderer r;

    List<Collider> colliders;

    private void Awake()
    {
        r = GetComponent<Renderer>();
    }


    private void Update()
    {
        colliders = Physics.OverlapBox(transform.position, transform.localScale/2, Quaternion.identity, layerMask).ToList();
        if (colliders.Count > 0)
        {
            canBuild = false;
        }
        else
        {
            canBuild = true;
        }
        if (CanBuild())
        {
            SetColor(new Color(0, 1, 0, 0.5f));
        }
        else
        {
            SetColor(new Color(1, 0, 0, 0.5f));
        }
        
    }

    public bool CanBuild()
    {
        return canBuild;
    }
    public void SetSize(Vector3 size)
    {
        transform.localScale = size;
    }

    void SetColor(Color c)
    {
        r.material.color = c;
    }

}
