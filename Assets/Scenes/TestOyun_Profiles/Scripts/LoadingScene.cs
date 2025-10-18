using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Image LoadingBarFill;

    public void LoadScene(int SceneID)
    {
        StartCoroutine(LoadSceneAsync(SceneID));
    }

    IEnumerator LoadSceneAsync(int SceneID)
    {
        LoadingScreen.SetActive(true);

        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneID);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            LoadingBarFill.fillAmount = progressValue; 
            yield return null;

        }
    }
}
