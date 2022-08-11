using UnityEngine;

public class SimpleTurret : Turret
{
    /*
     * Simple turret, single projectile and look at target
    */

    [SerializeField] float reloadTime = 1f;
    [SerializeField] float kineticDamage = 1f;
    [SerializeField] float explosiveDamage = 0f;
    [SerializeField] float explosiveRadius = 0f;
    [SerializeField] bool explosive = false;
    [SerializeField] GameObject projectilePrefab = null;
    
    private float reloadTimer = 0f;


    [SerializeField] Transform turret = null;
    [SerializeField] Transform[] muzzles = null;
    [SerializeField] GameObject effectPrefab = null;

    int currentMuzzle = 0;
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
                Shoot(projectilePrefab, muzzles[currentMuzzle]);
                currentMuzzle = (currentMuzzle + 1) % (muzzles.Length);
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
    void Shoot(GameObject projectile,Transform location)
    {
        Projectile p = GameObject.Instantiate(projectile, location.position, Quaternion.identity).GetComponent<Projectile>();
        p.transform.LookAt(currentTarget.transform.position,Vector3.up);
        p.SetDamage(kineticDamage);
        p.SetExplosiveDamage(explosiveDamage);
        p.SetExplosiveRadius(explosiveRadius);
        p.SetExplosiveProjectile(explosive);
        
        Destroy(p.gameObject, 5f);
        
        if (effectPrefab)
        {
            Instantiate(effectPrefab, location.position, Quaternion.identity);
        }

    }


    /// <summary>
    /// used by the upgrade handler to change the kinetic damage
    /// </summary>
    public void ChangeKineticDamage(float delta)
    {
        kineticDamage += delta;
    }


}
