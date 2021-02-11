using UnityEngine;

public class SimpleTurret : Turret
{
    /*
     * Simple turret
    */

    [SerializeField] float reloadTime = 1f;
    [SerializeField] float damage = 1f;
    [SerializeField] GameObject projectilePrefab = null;
    
    private float reloadTimer = 0f;


    [SerializeField] Transform turret = null;
    [SerializeField] Transform muzzle = null;
    private void Update()
    {
        if(currentTarget == null)
        {
            FindNextEnemy();
        }
        else if(Vector3.Distance(currentTarget.transform.position, transform.position) > range)
        {
            currentTarget = null;
        }
        else
        {
            turret.LookAt(currentTarget.transform.position + Vector3.up * 0.05f, Vector3.up);

            if(reloadTimer <= 0)
            {
                Shoot(projectilePrefab);
                reloadTimer = reloadTime;
            }
            else
            {
                reloadTimer -= Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// shoot projectile at target, also make sure to rotate it towards target
    /// </summary>
    void Shoot(GameObject projectile)
    {
        Projectile p = GameObject.Instantiate(projectilePrefab, muzzle.position, Quaternion.identity).GetComponent<Projectile>();
        p.transform.LookAt(currentTarget.transform.position,Vector3.up);
        p.SetDamage(damage);
        p.SetTarget(currentTarget.transform);
        
        Destroy(p.gameObject, 5f);
        GetComponent<AudioSource>().Play();

    }


}
