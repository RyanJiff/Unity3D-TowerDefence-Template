using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    /*
     * simple enemy script that will pathfind to HQ, if it gets blocked it will take a direct path destroy obstacles.
     */

    //bool to check if we are blocked from HQ
    [SerializeField] float speed = 1;
    [SerializeField] float damage = 1;
    NavMeshAgent agent;
    NavMeshPath path;

    GameObject target;

    Entity entity;

    private void Awake()
    {
        //make sure we have the correct parent object
        transform.parent = GameObject.FindGameObjectWithTag("Enemies").transform;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        path = new NavMeshPath();
        entity = GetComponent<Entity>();
        
        if (!target)
        {
            target = FindTarget();
        }

    }
    private void FixedUpdate()
    {
        if (target && !agent.hasPath)
        {
            agent.SetDestination(target.transform.position);
        }
    }

    public bool CalculateNewPath(Vector3 targetPos)
    {
        agent.CalculatePath(targetPos, path);
        if (path.status != NavMeshPathStatus.PathComplete)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    GameObject FindTarget()
    {
        GameObject t = GameObject.FindObjectOfType<Headquarters>().gameObject;
        if (t)
        {
            return t;
        }
        return null;
    }

    private void OnTriggerStay(Collider col)
    {
        Headquarters hq = col.gameObject.GetComponent<Headquarters>();
        if (hq)
        {
            Debug.Log("got to HQ!");
            hq.GetComponent<Entity>().Damage(damage);
            Destroy(this.gameObject);
        }
    }

}
