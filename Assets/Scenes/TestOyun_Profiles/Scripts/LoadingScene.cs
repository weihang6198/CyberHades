using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public GameObject LoadingScreen;
    public CanvasGroup LoadingSceneCanvasGroup;

    public Image LoadingBarFill;

    public void OnLoadScene(int SceneID)
    {
        StartCoroutine(LoadSceneAsync(SceneID));
    }

    IEnumerator LoadSceneAsync(int SceneID)
    {
        LoadingScreen.SetActive(true);
        FadeFilterByCanvas(LoadingSceneCanvasGroup,0,1,0.5f);

        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneID);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            LoadingBarFill.fillAmount = progressValue; 
            yield return null;

        }
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
