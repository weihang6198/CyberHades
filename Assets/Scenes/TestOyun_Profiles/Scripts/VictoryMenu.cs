using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class VictoryMenu : MonoBehaviour
{

    [SerializeField] DeadState bossDeadStateClass;

    public CanvasGroup VictoryCanvasGroup;

    private ColorAdjustments colorAdjustments;
    private ColorParameter blackColor = new ColorParameter(Color.black);
    [SerializeField] float waitDuration = 3.0f;
    [SerializeField] PauseMenu pauseMenuClass;
    public bool isVectory = false;
    void Update()
    {
        if (bossDeadStateClass.isDead && !isVectory)
        {
            isVectory = true;
            VictoryCanvasGroup.gameObject.SetActive(isVectory);
            StartCoroutine(FadeFilterByCanvas(VictoryCanvasGroup, 0, 1, 2f));
        }
    }

    public void LoadScene(int SceneID)
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneID);
    }



    private IEnumerator FadeOutAndLoadScene(int SceneID, float duration)
    {
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneID);

        operation.allowSceneActivation = false;

        Color fromColor = colorAdjustments.colorFilter.value;

        yield return StartCoroutine(FadeFilter(fromColor, ((Color)blackColor), duration));

        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        operation.allowSceneActivation = true;

    }

    private IEnumerator FadeFilter(Color fromColor, Color toColor, float duration)
    {
        float elapsedTime = 0f;
        colorAdjustments.colorFilter.overrideState = true;

        colorAdjustments.colorFilter.value = fromColor;


        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsedTime / duration);

            Color newColor = Color.Lerp(fromColor, toColor, t);
            GetComponentInParent<CanvasGroup>().alpha = Mathf.Lerp(1, 0, t);
            colorAdjustments.colorFilter.value = newColor;

            yield return null;
        }

        colorAdjustments.colorFilter.value = toColor;
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

        yield return new WaitForSecondsRealtime(waitDuration);
        Debug.Log("waitDuration is over now going to call my bro");
        pauseMenuClass.OnExitToTitle();
    }
}
