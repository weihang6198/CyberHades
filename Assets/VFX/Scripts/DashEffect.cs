using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEffect : MonoBehaviour
{
    public GameObject DashVFX;
    public float lifetime;

    public void Execute(Vector3 position, Vector3 positionOffset = new Vector3())
    {
        if (DashVFX != null)
        {
            GameObject dashFX = Instantiate(
                            DashVFX,
                            position,
                            transform.rotation
                        );
            dashFX.transform.parent = transform;
            dashFX.transform.localPosition += new Vector3 ( 0, 0, 0.3f );

            dashFX.SetActive(true);
            ParticleSystem[] particles = dashFX.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in particles)
            {
                ps.Play();
            }

            TrailRenderer[] trails = dashFX.GetComponentsInChildren<TrailRenderer>();
            foreach (TrailRenderer trail in trails)
            {
                trail.Clear();
                trail.emitting = true;
            }

            StartCoroutine(StopTrailAfter(dashFX, lifetime));
        }
    }


    IEnumerator StopTrailAfter(GameObject fxInstance, float time)
    {
        yield return new WaitForSeconds(time);

        TrailRenderer[] trails = fxInstance.GetComponentsInChildren<TrailRenderer>();
        foreach (TrailRenderer tr in trails)
            tr.emitting = false;

        ParticleSystem[] particles = fxInstance.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particles)
            ps.Stop();

        Destroy(fxInstance, 0.5f);
    }
}
