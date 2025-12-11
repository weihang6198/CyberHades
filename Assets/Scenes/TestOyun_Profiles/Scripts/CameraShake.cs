using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Transform originalTransform;

    public void ShakeByDuration(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originPos = originalTransform.localPosition;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            originalTransform.localPosition = new Vector3(x + originPos.x, y + originPos.y, originPos.z);

            elapsed += Time.deltaTime;


            yield return null;
        }

        originalTransform.localPosition = originPos;
    }
}
