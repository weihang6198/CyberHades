#undef USE_DISTANCE_TRIGGER 
//#define USE_DISTANCE_TRIGGER 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class SceneTrigger : MonoBehaviour
{
    public Transform playerTransfrom;
    public LoadingScene loadingScene;
    public float triggerDistance = 5.0f;
    public int SceneID;


    // Update is called once per frame
    void Update()
    {
        if (playerTransfrom == null || loadingScene == null) return;
        if (SceneID == int.MaxValue) return;

#if USE_DISTANCE_TRIGGER
        float dist = Vector3.Distance(playerTransfrom.position, transform.position);

        if(dist < triggerDistance)
        {
            loadingScene.LoadScene(SceneID);
        }
#else
       if( Input.GetKeyDown(KeyCode.E))
        {
            loadingScene.LoadScene(SceneID);
        }
#endif

    }
}
