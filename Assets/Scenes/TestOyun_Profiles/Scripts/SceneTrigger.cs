#undef USE_DISTANCE_TRIGGER 
//#define USE_DISTANCE_TRIGGER 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class SceneTrigger : MonoBehaviour
{
    //public Transform playerTransfrom;
    //public LoadingScene loadingScene;
    //public float triggerDistance = 5.0f;
    public int SceneID;


    // Update is called once per frame
    void Update()
    {


    }

    void OnCheckStageClear()
    {
        EnemyManager.instance.OnCheckAllEnemiesAlive();
    }

}
