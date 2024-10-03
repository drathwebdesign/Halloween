using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonAIController : MonoBehaviour {
    private NavMeshAgent agent;
    private Transform player;
    private Vector3 currentDestination;

    //Patrol
    private bool isPatrolling = true;
    private float waitTimer = 0f;
    public float patrolRadius = 10f;
    public float patrolWaitTime = 5f;


    //Detect
    public float detectionAngle = 45f;
    public float detectionDistance = 5f;

    //Pursuit player
    public float lostSightDuration = 5f;
    private float timeSinceLostSight = 0f;
    private bool isPursuing = false;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        PatrolToNewPoint();
    }

    void Update() {
        if (DetectPlayer()) {
            isPursuing = true;
            timeSinceLostSight = 0f;
            agent.SetDestination(player.position);
        } else if (isPursuing) {
            timeSinceLostSight += Time.deltaTime;
            if (timeSinceLostSight <= lostSightDuration) {
                agent.SetDestination(player.position);
            } else {
                isPursuing = false;
                PatrolToNewPoint();
            }
        } else {
            PatrolBehavior();
        }
    }

    void PatrolBehavior() {
        if (isPatrolling && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
            waitTimer += Time.deltaTime;
            if (waitTimer >= patrolWaitTime) {
                PatrolToNewPoint();
                waitTimer = 0f;
            }
        }
    }

    void PatrolToNewPoint() {
        Vector3 randomPoint = GetRandomPointOnNavMesh(transform.position, patrolRadius);
        if (randomPoint != Vector3.zero) {
            currentDestination = randomPoint;
            agent.SetDestination(currentDestination);
        }
    }

    bool DetectPlayer() {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionDistance) {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer <= detectionAngle / 2) {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer.normalized, out hit, detectionDistance)) {
                    if (hit.transform.CompareTag("Player")) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    Vector3 GetRandomPointOnNavMesh(Vector3 center, float radius) {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas)) {
            return hit.position;
        }
        return Vector3.zero;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, detectionDistance);

        Vector3 forward = transform.forward * detectionDistance;
        Vector3 leftBoundary = Quaternion.Euler(0, -detectionAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, detectionAngle / 2, 0) * forward;

        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
    }
}