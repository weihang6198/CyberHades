using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarVisibility : MonoBehaviour
{
    [SerializeField] HPBarFill hpBarFillClass;
    [SerializeField] HealthBar EnemyhpBarClass;
    [SerializeField]  CanvasGroup canvas;
    [SerializeField]  float VisibilityDuration;
    bool isShowing = false;
    bool isHPChanged = false;

    void Update()
    {
        if (EnemyhpBarClass != null)
        {
            isHPChanged = EnemyhpBarClass.isHPChanged;
        }
        else if(hpBarFillClass != null)
        {
            isHPChanged = hpBarFillClass.isHPChanged;
        }

        if (isHPChanged && !isShowing)
        {
            Debug.Log("HPChanged");
            StartCoroutine(VisibleByDuration());
        }
    }
    IEnumerator VisibleByDuration()
    {
        isShowing = true;

        yield return StartCoroutine(FadeFilterByCanvas(canvas, 0, 1, 2f));

        yield return new WaitForSecondsRealtime(VisibilityDuration);

        yield return StartCoroutine(FadeFilterByCanvas(canvas, 1, 0, 2f));

        isShowing = false;
    }

    private IEnumerator FadeFilterByCanvas(CanvasGroup adjust, float from, float to, float duration, float waitForSeconds = 0.0f)
    {
        float elapsedTime = 0f;

        yield return new WaitForSecondsRealtime(waitForSeconds);

        while (elapsedTime < duration)
        {

            elapsedTime += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsedTime / duration);

            float newColor = Mathf.Lerp(from, to, t);
            adjust.alpha = newColor;
            yield return null;
        }

        adjust.alpha = to;
    }
}
