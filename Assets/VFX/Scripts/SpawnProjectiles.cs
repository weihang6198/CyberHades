using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectiles : MonoBehaviour
{
    public Transform firePoint;
    public List<GameObject> vfx = new List<GameObject>();
    public RotateToMouse rotateToMouse;

    private GameObject effectToSpawn;

    // Start is called before the first frame update
    void Awake()
    {
        effectToSpawn = vfx[0];
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetMouseButton(0) && Time.time >= timeToFire)
        //{
        //    timeToFire = Time.time + 1 / effectToSpawn.GetComponent<ProjectileMove>().fireRate;
        //    SpawnVFX();
        //}
    }

    public void SpawnVFX(Vector3 direction,FighterBase owner)
    {
        Vector3 pos = firePoint.transform.position;
        pos.y = 1f;
        firePoint.transform.position = pos;
        if (firePoint == null)
        {
            Debug.LogWarning("FirePoint is null");
            return;
        }
       
        Quaternion rot = Quaternion.LookRotation(direction);
        GameObject projectileObj = Instantiate(effectToSpawn, firePoint.position, rot);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.owner = owner;
        }
    }
}
