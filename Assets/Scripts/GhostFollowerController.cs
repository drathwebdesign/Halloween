using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostFollowerController : MonoBehaviour {
    Transform player;
    private NavMeshAgent agent;
    private bool shouldFollow = false;
    public float maxFollowDistance = 1f;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.transform;
    }

    void Update() {
        if (shouldFollow) {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > maxFollowDistance) {
                agent.SetDestination(player.position);
            } else {
                agent.ResetPath();
            }
        }
    }

    public void StartFollowing() {
        shouldFollow = true;
    }

    public void StopFollowing() {
        shouldFollow = false;
    }
}