#define FADE_IN 

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public CanvasGroup UIGroup;
    public CanvasGroup PanelGroup;
    public CanvasGroup mainMenuCanvasGroup;
    public CanvasGroup playScreenCanvasGroup;
    public CanvasGroup OptionsCanvasGroup;
    public CanvasGroup loadingScreenCanvasGroup;
    public GameObject mainMenu;
    public GameObject titleScreen;
    public GameObject playScreen;
    public GameObject optionScreen;
    public GameObject loadingScreen;
    public float ScaleSpeed = 0.5f;
    public float transitionDuration = 2.0f;
    public float fadeDuration = 10.0f;

    public Image LoadingBarFill;
    TextMeshProUGUI TextMP;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
#if FADE_IN
        StartCoroutine(FadeOutStart());
#endif
    }

 // Update is called once per frame
 void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) ||   Input.GetMouseButtonDown(0))
        {
            if (titleScreen.activeSelf)
            {
                OnTitleClick();
                Debug.Log("enter key has down");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (playScreen.activeSelf || optionScreen.activeSelf)
            {
                OnEscClick();
            }

            Debug.Log("esc key has down");
        }
    }

    public void OnTitleClick()
    {
        Debug.Log("OnTitleClick function called");

        FindTextByName("PushEnterToStart");

        if (TextMP != null)
        {
            StartCoroutine(ZoomAndFadeOutCoroutine(mainMenuCanvasGroup, false, true, false, false));
        }
    }

    public void OnEscClick()
    {
        Debug.Log("OnEscClick function called");

        OnFadeByCanvesGroup(mainMenuCanvasGroup, false, true, false, false);

    }

    public void OnPlayClick()
    {
        Debug.Log("OnPlayClick function called");

        FindTextByName("PlayTMP");

        if (TextMP != null)
        {
            StartCoroutine(ZoomAndFadeOutCoroutine(playScreenCanvasGroup, false, false, true, false));
        }
        else
            Debug.Log("Play TMP not found!");

    }

    public void OnExitClick()
    {
        Debug.Log("OnExitClick function called");

        Application.Quit();
    }

    public void OnNewGameClick(int SceneID)
    {
        Debug.Log("OnExitClick function called");

        FindTextByName("NewGameTMP");

        if (TextMP != null)
        {
            StartCoroutine(ZoomAndFadeOutCoroutine(loadingScreenCanvasGroup, false, false, false, false));
        }
        LoadScene(SceneID);
    }

    public void OnLoadGameClick(int SceneID)
    {
        Debug.Log("OnLoadGameClick function called");

        //ToDo
    }


    public void OnOptionsClick()
    {
        Debug.Log("OnOptionsClick function called");

        FindTextByName("OptionsTMP");

        if (TextMP != null)
        {
            StartCoroutine(ZoomAndFadeOutCoroutine(OptionsCanvasGroup, false, false, false, true));
        }
        else
            Debug.Log("Play TMP not found!");

    }

    public void LoadScene(int SceneID)
    {
        StartCoroutine(LoadSceneAsync(SceneID));
    }

    IEnumerator LoadSceneAsync(int SceneID)
    {

        //loadingScreen.SetActive(true);
        //ForDebug
        //yield return new WaitForSeconds(2f);

        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneID);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            LoadingBarFill.fillAmount = progressValue;


            yield return null;

        }
    }

    private void FindTextByName(string name)
    {
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in texts)
        {
            if (text == null) continue;

            if (text.name == name)
            {
                TextMP = text;
                Debug.Log("TextTMP: " + name);

                break;
            }
        }
    }

    public void OnFadeByCanvesGroup(CanvasGroup canvasGroup, bool isTitleActive, bool isMenuActive, bool isPlayActive, bool isOptionsActive)
    {
        titleScreen.SetActive(isTitleActive);
        mainMenu.SetActive(isMenuActive);
        playScreen.SetActive(isPlayActive);
        optionScreen.SetActive(isOptionsActive);

        mainMenuCanvasGroup.alpha = 0.0f;
        mainMenuCanvasGroup.interactable = false;
        StartCoroutine(FadeInCoroutine(canvasGroup));
    }

    private IEnumerator FadeInCoroutine(CanvasGroup canvasGroup)
    {
        float startTime = Time.time;
        float fadeDuration = transitionDuration;

        canvasGroup.gameObject.SetActive(true);

        while (Time.time < startTime + fadeDuration)
        {
            float elapsed = Time.time - startTime;
            float progress = elapsed / fadeDuration;

            float easedProgress = Mathf.SmoothStep(0.0f, 1.0f, progress);
            float currentAlpha = Mathf.Lerp(0.0f, 1.0f, easedProgress);

            canvasGroup.alpha = currentAlpha;

            yield return null;
        }

        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
    }

    private IEnumerator FadeOutStart()
    {
        Debug.Log("FadeInStart function Called!");

        GameObject panel = PanelGroup.GameObject();
        panel.SetActive(true);

        PanelGroup.alpha =1.0f;
        PanelGroup.interactable = false;
        PanelGroup.blocksRaycasts = true;

        float elapsedTime = 0.0f;

        while (elapsedTime< fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / fadeDuration;
            float easedProgress = Mathf.SmoothStep(0.0f, 1.0f, progress);
            PanelGroup.alpha = Mathf.Lerp(1.0f, 0.0f, easedProgress);
            yield return null;

        }

        PanelGroup.alpha = 0f;
        PanelGroup.interactable = false;
        PanelGroup.blocksRaycasts = false;
        panel.SetActive(false);

    }

    private IEnumerator ZoomAndFadeOutCoroutine(CanvasGroup canvasGroup, bool isTitleActive, bool isMenuActive, bool isPlayActive,bool isOptionsActive)
    {
        float startTime = Time.time;
        Color baseColor = TextMP.color;

        baseColor.a = 1.0f;

        while (Time.time < startTime + transitionDuration)
        {
            float elapsed = Time.time - startTime;
            float progress = elapsed / transitionDuration;

            float easedProgress = Mathf.SmoothStep(0.0f, 1.0f, progress);

            float currentScale = Mathf.Lerp(1.0f, 2.0f, easedProgress);
            TextMP.transform.localScale = new Vector3(currentScale, currentScale, currentScale);

            float currentAlpha = Mathf.Lerp(1.0f, 0.0f, easedProgress);

            Color c = baseColor;
            c.a = currentAlpha;
            TextMP.color = c;

            yield return null;
        }

        TextMP.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
        Color finalC = baseColor;
        finalC.a = 0.0f;
        TextMP.color = finalC;

        OnFadeByCanvesGroup(canvasGroup, isTitleActive, isMenuActive, isPlayActive, isOptionsActive);
        //titleScreen.SetActive(isTitleActive);
        //mainMenu.SetActive(isMenuActive);
        //playScreen.SetActive(isPlayActive);

        TextMP.transform.localScale = Vector3.one;
        finalC.a = 1.0f;
        TextMP.color = finalC;
        TextMP = null;
    }



}
