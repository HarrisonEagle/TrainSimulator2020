using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Follow : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject firstcar;
    private NavMeshAgent nav;
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        nav.destination = firstcar.transform.position;
    }
}
