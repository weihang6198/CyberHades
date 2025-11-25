using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Pathfinding.BehaviorTree
{
    public interface IStrategy
    {
        Node.Status Process();
        void Reset()
        {
            //Noop
        }
    }
    public class ActionStrategy : IStrategy
    {
        readonly Action doSomething;
        public ActionStrategy(Action doSomething)
        {
            this.doSomething = doSomething;
        }

        public Node.Status Process()
        {
            doSomething();
            return Node.Status.Success;
        }
    }
    public class Condition : IStrategy
    {
        readonly Func<bool> predicate;

        public Condition(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public Node.Status Process() => predicate() ? Node.Status.Success : Node.Status.Failure;
    }
    public class PatrolStrategy : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        readonly List<Transform> patrolPoints;
        readonly float patrolSpeed;
        readonly float rotationSpeed; // new

        int currentIndex = 0;
        bool movingToPoint = false;

        public PatrolStrategy(Transform entity, NavMeshAgent agent, List<Transform> patrolPoints, float patrolSpeed = 7f, float rotationSpeed = 7f)
        {
            this.entity = entity;
            this.agent = agent;
            this.patrolPoints = patrolPoints;
            this.patrolSpeed = patrolSpeed;
            this.rotationSpeed = rotationSpeed;

            agent.updateRotation = false;      // we handle rotation manually
            agent.baseOffset = 0.0f;           // fix mesh height to NavMesh
        }

        public Node.Status Process()
        {
            if (currentIndex >= patrolPoints.Count)
                return Node.Status.Success;

            Transform target = patrolPoints[currentIndex];

            // 1. Only set destination once
            if (!movingToPoint)
            {
                agent.stoppingDistance = 0f;
                agent.speed = patrolSpeed;
                agent.SetDestination(target.position);

                movingToPoint = true;
                return Node.Status.Running;
            }

            // 2. Wait while path is being calculated
            if (agent.pathPending)
                return Node.Status.Running;

            // 3. Rotate smoothly toward target on horizontal plane
            Vector3 lookDir = target.position - entity.position;
            lookDir.y = 0f; // keep Y level
            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                entity.rotation = Quaternion.Slerp(entity.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            // 4. Check arrival
            if (agent.hasPath && agent.remainingDistance <= 0.05f)
            {
                currentIndex++;
                movingToPoint = false;
                return Node.Status.Running;
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            currentIndex = 0;
            movingToPoint = false;
        }
    }


}
