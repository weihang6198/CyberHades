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
    [SerializeField] public float notAttackingTimer = 2f;

    public int registredEnemiesCount = 0;
    public bool IsEnemiesAlive = false;
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

    public EnemyController GetAttackingEnemy()
    {
        //return the first enemy that does not satify/satisfy the condition
        return enemiesInRange.FirstOrDefault(e => e.IsInState(EnemyStates.Attack));
    }

    public EnemyController GetClosestEnemyForwardDir(Vector3 forwardDir,float maxAngle = 45f,float maxDistance = 10f)
    {
        EnemyController closestEnemy = null;
        float closestDist = Mathf.Infinity;

        Vector3 playerPos = player.transform.position;
        Vector3 forward = forwardDir; //this forward dir is when player about the atk, the expected rot to turn to

        // Get list of all enemies — replace with your own enemy manager
        EnemyController[] enemies = GameObject.FindObjectsOfType<EnemyController>();

        foreach (var enemy in enemies)
        {
            Vector3 dirToEnemy = enemy.transform.position - playerPos;
            float distance = dirToEnemy.magnitude;

            // skip if too far
            if (distance > maxDistance) continue;

            dirToEnemy.Normalize();

            // check forward angle
            float angle = Vector3.Angle(forward, dirToEnemy);
            if (angle > maxAngle) continue;

            // pick closest
            if (distance < closestDist)
            {
                closestDist = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    public bool OnCheckAllEnemiesAlive()
    {
        if (registredEnemiesCount <= 0)
        {
            IsEnemiesAlive = false;
        }

        return IsEnemiesAlive;
    }

    public void RegisterEnemy()
    {
        registredEnemiesCount++;
        IsEnemiesAlive = true;
    }
    public void UnregisterEnemy()
    {
        registredEnemiesCount--;
    }
}
