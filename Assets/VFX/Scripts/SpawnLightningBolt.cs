using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpawnLightningBolt : MonoBehaviour
{
    [SerializeField] public GameObject LightningVFX;
    [SerializeField] public DecalProjector decalWarning;

    //[SerializeField] public float radius = 3.0f;
    public float waitduration = 0.5f;
    public float r = 3f;
    bool isSpawned = false;

    void Start()
    {

    }

    void Update()
    {
        //for debug
        if (!isSpawned && Input.GetKeyDown(KeyCode.E))
        {
            SpawnLightning(r, new Vector3(0,0,0), waitduration);
            isSpawned = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            isSpawned = false;
        }
    }

    public void SpawnLightning(float radius, Vector3 spawnPosition, float waitDuration = 0.1f)
    {
        StartCoroutine(Spawn(radius, spawnPosition, waitDuration));
    }

    IEnumerator Spawn(float radius, Vector3 spawnPosition, float waitDuration = 0.1f)
    {
        DecalProjector decal = Instantiate(decalWarning, spawnPosition, Quaternion.LookRotation(Vector3.down));
        decal.size = new Vector3(radius, radius, decal.size.y);

        yield return new WaitForSeconds(waitDuration);
        Destroy(decal);

        GameObject objLightning = Instantiate(LightningVFX, spawnPosition, Quaternion.identity);
        ParticleSystem ps = objLightning.GetComponentInChildren<ParticleSystem>();
        //var state = ps.GetPlaybackState();
        var main = ps.main;

        float lifetime = main.duration + main.startLifetime.constantMax;

        yield return new WaitForSeconds(lifetime);

        Destroy(objLightning);
    }

 
}
