using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public Volume GlobalVol;
    public VolumeProfile PauseVolume;
    public VolumeProfile MainVolume;
    public CanvasGroup PauseMenuCanvasGroup;
    public bool IsPaused = false;

    private ColorAdjustments colorAdjustments;
    private ColorParameter blackColor = new ColorParameter(Color.black);

    TextMeshProUGUI TextMP;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TogglePause(!IsPaused);
        }
    }

    void TogglePause(bool pause)
    {
        IsPaused = pause;
        Time.timeScale = IsPaused ? 0f : 1f;
        PauseMenuCanvasGroup.gameObject.SetActive(IsPaused);
        GlobalVol.profile = IsPaused ? PauseVolume : MainVolume;
    }
    public void OnResumeGame()
    {
        TogglePause(false);
    }
    void Awake()
    {
        if (PauseVolume.TryGet(out colorAdjustments))
        {
            colorAdjustments.colorFilter.overrideState = true;
        }
    }
    public void OnExitToTitle()
    {
        if (PauseVolume.TryGet(out colorAdjustments))
        {
            StartCoroutine(FadeOutAndLoadScene(0, 0.5f));
        
        }
        else
        {
            Debug.LogError("ColorAdjustments component not found on the Volume Profile!");
            LoadScene(0);

        }
    } 

    public void OnRestartStage()
    {
        if (PauseVolume.TryGet(out colorAdjustments))
        {
            StartCoroutine(FadeOutAndLoadScene(GetCurrentSceneID(), 0.5f));
        
        }
        else
        {
            Debug.LogError("ColorAdjustments component not found on the Volume Profile!");
            LoadScene(GetCurrentSceneID());

        }
    }

    static int GetCurrentSceneID()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
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

}
