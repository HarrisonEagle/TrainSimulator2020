using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movedestination : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform goal;

    void Start()
    {
        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.destination = goal.position;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
