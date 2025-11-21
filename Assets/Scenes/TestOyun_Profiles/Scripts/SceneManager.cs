using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public LoadingScene loadSceneClass; 
    public static SceneManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    public void LoadSceneByID(int newScene)
    {
        if (loadSceneClass != null)
        {
            loadSceneClass.OnLoadScene(newScene);
        }
    }

    // Update is called once per frame
    void Update()
    {


    }
}
