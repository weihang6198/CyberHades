    using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] CombatController    player;
    [SerializeField] EnemySpawnManager  enemySpawnManager;
    [SerializeField] Vector2 timeRangeBetweenAttacks = new Vector2(1, 4);
    public List<EnemyController> enemiesInRange = new List<EnemyController>();

    public static EnemyManager instance { get; private set; }
    public float notAttackingTimer = 2f;
    float timer = 0;
    private void Awake()
    {
        instance = this;
    }

    public void AddEnemyRange(EnemyController enemy)
    {
        if (!enemiesInRange.Contains(enemy))
            enemiesInRange.Add(enemy);
    }

    public void RemoveEnemyInRange(EnemyController enemy)
    {
        enemiesInRange.Remove(enemy);

        //if player is far away from enemy, enemy will be removed
        //if (enemy == player.TargetEnemy)
        //{
        //    enemy.MeshHighlighter.HighlightMesh(false);

        //    //look for a new target enemy to target when the prev enemy is removed
        //   // player.TargetEnemy = GetClosestEnemyToDir(player.GetTargetingDir());
        //    player.TargetEnemy?.MeshHighlighter.HighlightMesh(true);
        //}


    }

    private void Update()
    {
        if (enemiesInRange.Count == 0) return;
        //check if enemy is in attack state
        //if not atking, decrease notAttackingTimer
        //if notAttackingTimer reaches 0 , atk player
        if (!enemiesInRange.Any(e => e.IsInState(EnemyStates.Attack)))
        {
            if (notAttackingTimer > 0)
                notAttackingTimer -= Time.deltaTime;

            if (notAttackingTimer <= 0)
            {
                //attack player
                var attackingEnemy = SelectEnemyForAttack();

                if (attackingEnemy != null)
                {
                    attackingEnemy.ChangeState(EnemyStates.Attack);
                    notAttackingTimer = Random.Range(timeRangeBetweenAttacks.x, timeRangeBetweenAttacks.y);
                }

            }


        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            enemySpawnManager.SpawnEnemy();
        }

        //if (timer > 0.1f)
        //{


        //    timer = 0f;
        //    //get closest enemy to target lock on 
        //    //var closestEnemy = GetClosestEnemyToDir(player.GetTargetingDir());
        //    //if (closestEnemy != null && closestEnemy != player.TargetEnemy)
        //    //{
        //    //    var prevEnemy = player.TargetEnemy;

        //    //    player.TargetEnemy = closestEnemy;

        //    //    player?.TargetEnemy?.MeshHighlighter.HighlightMesh(true);
        //    //    prevEnemy?.MeshHighlighter?.HighlightMesh(false);
        //    //}
        //}
        //timer += Time.deltaTime;
    }

    EnemyController SelectEnemyForAttack()
    {
        return enemiesInRange.OrderByDescending(e => e.CombatMovementTimer).
            FirstOrDefault(e => e.Target != null && e.IsInState(EnemyStates.CombatMovement));
    }
}
