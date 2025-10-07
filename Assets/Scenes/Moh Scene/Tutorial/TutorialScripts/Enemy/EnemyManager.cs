using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] Vector2 timeRangeBetweenAttacks = new Vector2(1, 4);
    public static EnemyManager instance {  get; private set; }

    private void Awake()
    {
        instance = this;
    }
     List<EnemyController> enemiesInRange=new List<EnemyController>();
    float notAttackingTimer = 2f;


    public void AddEnemyRange(EnemyController enemy)
    {
        if(!enemiesInRange.Contains(enemy))
            enemiesInRange.Add(enemy);
    }

    public void RemoveEnemyInRange(EnemyController enemy)
    {
        enemiesInRange.Remove(enemy);
    }

    private void Update()
    {
        if(enemiesInRange.Count==0) return;
        //check if enemy is in attack state
        //if not atking, decrease notAttackingTimer
        //if notAttackingTimer reaches 0 , atk player
        if (!enemiesInRange.Any(e => e.IsInState(EnemyState.Attack)))
        {
            if(notAttackingTimer> 0) 
                notAttackingTimer-=Time.deltaTime;

            if(notAttackingTimer <= 0)
            {
                //attack player
                var attackingEnemy = SelectEnemyForAttack();
                attackingEnemy.ChangeState(EnemyState.Attack);
                notAttackingTimer = Random.Range(timeRangeBetweenAttacks.x, timeRangeBetweenAttacks.y);
            }


        }
    }

    EnemyController SelectEnemyForAttack()
    {
        return enemiesInRange.OrderByDescending(e=>e.CombatMovementTimer).FirstOrDefault();
    }
}
