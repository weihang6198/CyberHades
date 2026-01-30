using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitVFXScript : MonoBehaviour
{
    [SerializeField] Renderer[] renderers;
    [SerializeField] Color hitColor = Color.red;
    [SerializeField] float intensity = 1.5f;
    [SerializeField] float fadeTime = 0.3f;

    List<Material> materials = new();
    Coroutine hitCoroutine;

    void Awake()
    {
        foreach (var r in renderers)
            materials.AddRange(r.materials);
    }

    public void PlayHit()
    {
        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);

        hitCoroutine = StartCoroutine(HitRoutine());
    }

    IEnumerator HitRoutine()
    {
        foreach (var mat in materials)
            mat.SetColor("_FresnelColor", hitColor * intensity);

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float lerp = 1f - t / fadeTime;

            foreach (var mat in materials)
                mat.SetColor("_FresnelColor", hitColor * intensity * lerp);

            yield return null;
        }

        foreach (var mat in materials)
            mat.SetColor("_FresnelColor", Color.black);

        hitCoroutine = null;
    }
}
