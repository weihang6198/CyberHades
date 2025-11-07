using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyTypes { Melee, Range, Wizard, Boss };
public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] public List<Transform> SpawnPosition;
    public EnemyController enemy;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemy(EnemyTypes type= EnemyTypes.Melee)
    {
        int rand = Random.Range(0, SpawnPosition.Count);
        Debug.Log("rand was:" + rand);
        EnemyController e = Instantiate(enemy, SpawnPosition[rand].transform.position, Quaternion.identity);
    }
}
