#undef USE_DISTANCE_TRIGGER 
//#define USE_DISTANCE_TRIGGER 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;




public class SceneTrigger : MonoBehaviour
{
    //public Transform playerTransfrom;
    //public LoadingScene loadingScene;
    //public float triggerDistance = 5.0f;
    public int SceneID;
    BoxCollider boxCollider;
    [SerializeField] public GameObject[] FireVFXs;
    [SerializeField] public GameObject FireVFX;
    [SerializeField] public Transform player;
    bool isVFXCreated = false;

    private void Awake()
    {
        boxCollider = GetComponentInChildren<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        OnStageClearVFX();

        if (boxCollider.bounds.Contains(player.position) && isVFXCreated)
        {
            //Debug.Log("player in bounds");
            SceneManager.instance.LoadSceneByID(SceneID);
        }
    }

    void OnStageClearVFX()
    {

        if (!EnemyManager.instance.OnCheckAllEnemiesAlive() && !isVFXCreated)
        {
            foreach (var item in FireVFXs)
            {
                GameObject fireVFX = Instantiate(FireVFX, item.transform.position, Quaternion.identity);
                item.gameObject.SetActive(true);
            }
            Debug.Log("StageClear");
            isVFXCreated = true;
        }
    }

}
