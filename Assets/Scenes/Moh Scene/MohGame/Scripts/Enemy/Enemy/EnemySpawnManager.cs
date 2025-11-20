using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyTypes { Melee, Range, Wizard, Boss };
public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] public List<Transform> SpawnPosition;
    public EnemyController enemy;
    public int EnemyCount = 3;
    void Start()
    {
        SpawnEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        if (EnemyManager.instance.registredEnemiesCount <= 0) 
        {
            Debug.Log("all Enemy Destroyed!");
        }
    }

    public void SpawnEnemy(EnemyTypes type= EnemyTypes.Melee)
    {
        int rand = Random.Range(0, SpawnPosition.Count);
        Debug.Log("rand was:" + rand);
        //EnemyController e = Instantiate(enemy, SpawnPosition[rand].transform.position, Quaternion.identity);

        for (int i = 0; i < EnemyCount; i++)
        {
            EnemyController e = Instantiate(enemy, new Vector3(Random.Range(-12, 12), 0, Random.Range(-12, 12)), Quaternion.identity);
        }
    }
}
