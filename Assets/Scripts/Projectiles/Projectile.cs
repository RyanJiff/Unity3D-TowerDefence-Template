using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /*
     * Base projectile class
     */
    [Header("Stats")]
    private float damage = 1;
    private float explosiveDamage = 1;
    private float explosiveRadius = 1;
    private bool explosive = false;

    float velocity = 10;

    [SerializeField] private GameObject destroyEffect = null;

    public void SetDamage(float d)
    {
        damage = d;
    }
    public void SetExplosiveDamage(float d)
    {
        explosiveDamage = d;
    }
    public void SetExplosiveRadius(float r)
    {
        explosiveRadius = r;
    }
    public void SetExplosiveProjectile(bool b)
    {
        explosive = b;
    }

    private void Update()
    {
        transform.Translate(transform.forward * velocity * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!explosive)
        {
            if (other.GetComponent<Enemy>())
            {
                other.GetComponent<Entity>().Damage(damage);
                Debug.Log("hit for:" + damage);
                if (destroyEffect)
                {
                    GameObject obj = Instantiate(destroyEffect, transform.position, Quaternion.identity);
                    obj.transform.localScale = Vector3.one * explosiveRadius / 2;
                    Destroy(obj, 0.5f);
                }
                Destroy(this.gameObject);
            }
        }
        else if (explosive)
        {
            if (other.GetComponent<Enemy>() || other.transform.tag == "Terrain")
            {
                Collider[] c;
                c = Physics.OverlapSphere(transform.position, explosiveRadius);
                for (int i = 0; i < c.Length; i++)
                {
                    if (c[i].GetComponent<Enemy>())
                    {
                        c[i].GetComponent<Entity>().Damage(explosiveDamage);
                    }
                }
                if (destroyEffect)
                {
                    GameObject obj = Instantiate(destroyEffect, transform.position, Quaternion.identity);
                    obj.transform.localScale = Vector3.one * explosiveRadius / 1.25f;
                }
                Destroy(this.gameObject);
            }
        }
    }
}
