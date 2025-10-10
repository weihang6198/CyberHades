using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public Transform PlayerTransfrom;
    public float Distance = 5;
    public string SceneName;
    public void LoadScreen(string newScene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(newScene);
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(transform.position,PlayerTransfrom.position);

        Debug.Log(dist);

        if (dist < Distance)
        {
            LoadScreen(SceneName);
        }

    }
}
