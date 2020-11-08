using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayHousou : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        GetComponent<AudioSource>().Play();
        Debug.Log("当たった!");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
