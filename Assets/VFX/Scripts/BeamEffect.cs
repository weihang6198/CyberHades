using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BeamEffect : MonoBehaviour
{
    [SerializeField] public GameObject BeamChargeVFX;
    [SerializeField] public GameObject BeamLaserVFX;
    Transform SpawnTransform;
    bool isSpawned = false;


    private void Awake()
    {
        SpawnTransform = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Chest);
    }
    // Update is called once per frame
    void Update()
    {

        if (!isSpawned && Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(StartBeam());
            isSpawned = true;
        }

        if (Input.GetKeyDown(KeyCode.R)) 
        {
            isSpawned = false;
        }
    }

    IEnumerator StartBeam()
    {
        // spawn first vfx
        GameObject obj = Instantiate(BeamChargeVFX, SpawnTransform.position, SpawnTransform.rotation);

        // get particle inside the spawned object
        ParticleSystem ps = obj.GetComponentInChildren<ParticleSystem>();

        var main = ps.main;
        float lifetime = main.startLifetime.constant; // or main.startLifetime.constantMin for random range

        // wait until lifetime is almost over
        yield return new WaitForSeconds(55f/60f);

        // now spawn next vfx
        Instantiate(BeamLaserVFX, SpawnTransform.position, SpawnTransform.rotation);

        Renderer[] renderers = BeamLaserVFX.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            if (renderer.material.HasProperty("_LaserDissolve"))
            {
                StartCoroutine(Fade(renderer, 1.103f, 1.0f, 1f, "_LaserDissolve"));
            }
            else
            {
                Debug.Log("Has not laserDis");
            }
        }
    }

    private IEnumerator Fade(Renderer adjust, float from, float to, float duration,string name, float waitForSeconds = 0.0f)
    {
        float elapsedTime = 0f;

        yield return new WaitForSecondsRealtime(waitForSeconds);

        while (elapsedTime < duration)
        {

            elapsedTime += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsedTime / duration);

            float progress = Mathf.Lerp(from, to, t);
            adjust.material.SetFloat(name, progress);
            yield return null;
        }

        adjust.material.SetFloat(name, to);
    }
}
