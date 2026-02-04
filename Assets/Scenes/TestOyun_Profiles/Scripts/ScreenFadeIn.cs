using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFadeIn : MonoBehaviour
{
    [SerializeField] CanvasGroup canvas;
    [SerializeField] float fadeDuration = 1.2f;
    public bool isScreenFadeOut = false;

    void Start()
    {
        gameObject.SetActive(true);

        canvas.alpha = 1f;
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float normalized = t / fadeDuration;

            float eased = Mathf.SmoothStep(1f, 0f, normalized);

            canvas.alpha = eased;
            yield return null;
        }

        canvas.alpha = 0f;
        canvas.blocksRaycasts = false;
        gameObject.SetActive(false);

        //this.enabled = false;
    }
    public IEnumerator FadeOut()
    {
        gameObject.SetActive(true);

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float normalized = t / fadeDuration;

            float eased = Mathf.SmoothStep(0f, 1f, normalized);

            canvas.alpha = eased;
            yield return null;
        }

        canvas.alpha = 1f;
        canvas.blocksRaycasts = false;
        gameObject.SetActive(false);
        isScreenFadeOut = true;
        //this.enabled = false;
    }
}
