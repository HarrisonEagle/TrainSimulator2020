using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform goal;
    public Transform forward;
    public Transform back;
    double len = 20000;
    public Transform[] staobj=new Transform[7];
    public AudioSource[] arrive = new AudioSource[7];
    public AudioSource[] housou = new AudioSource[7];
    bool dooropen = false;
    string[] stationname = { "三鷹", "吉祥寺", "西荻窪", "荻窪", "阿佐ヶ谷", "高円寺", "中野" };
    int staindex = 0;

    public float[] acc = {2.4f,1.8f,1.3f,0.9f,-0.01f,-0.3f,-0.9f,-1.5f,-2.1f,-2.8f,-3.6f,-4.2f,-7.3f};
    string[] modes = {"Mode:P4", "Mode:P3", "Mode:P2", "Mode:P1", "Mode:N", "Mode:B1", "Mode:B2", "Mode:B3", "Mode:B4", "Mode:B5", "Mode:B6", "Mode:B7", "Mode:EB" };
    public float[] rate = { 1.0f, 0.95f, 0.93f, 0.91f, 0f, -0.89f, -0.92f, -0.95f, -0.95f, -0.975f, -0.98f, -0.99f, -1.0f };
    
    public int index = 12;
    private Timer MyTimer;
    private NavMeshAgent agent;

    public Text acctext;
    public Text destext;
    public Text speedtext;
    public Text disttext;

    public float timeOut=0.01f;
    private float timeElapsed;

    public AudioSource accsound,heisoku,rapid,keiteki;

 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;
        setacc(index);

    }

    void setacc(int index)
    {
        acctext.text = this.modes[index];
        if (index!=4)
        {
            rapid.Pause();
            heisoku.Pause();
            accsound.Play();
            accsound.pitch = this.rate[index];
            accsound.time = accsound.clip.length * (agent.speed * 3.6f / 100f);
            Debug.Log(this.rate[index]);
            Debug.Log(index);
            
        }
        else
        {
            accsound.Pause();
            if (agent.speed * 3.6f > 80)
            {
                rapid.Play();
            }
            else if(agent.speed * 3.6f > 10)
            {
                heisoku.Play();
            }
        }
    }
    // Update is called once per frame
    int before = 0;
    bool housourunned = false;
    void Update()
    {
        double stadis = Vector3.Distance(agent.transform.position, staobj[staindex].transform.position);
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= timeOut)
        {
            agent.speed += (acc[index]*timeOut);
            timeElapsed = 0.0f;
            speedtext.text = "Speed:" + (agent.speed*3.6f).ToString("F2") + "km/h";
            disttext.text = stationname[staindex] + "まで後" + stadis.ToString("F2") + "m";
        }
        if (agent.speed==0&&stadis<4.0)
        {
            dooropen = true;
            arrive[staindex].Play();
            before = staindex;
            staindex++;
            stadis = Vector3.Distance(agent.transform.position, staobj[staindex].transform.position);
            len = stadis;
            housourunned = false;
            Debug.Log(len);
        }
        if (len-stadis>100&&!housourunned&&staindex>=1)
        {
            housou[staindex].Play();
            housourunned = true;
        }
        if (!arrive[before].isPlaying)
        {
            dooropen = false;
        }

        if (Input.GetKeyDown(KeyCode.W) && !dooropen)
        {
            if (index < 12)
            {
                index++;
                setacc(index);

            }

        }
        if (Input.GetKeyDown(KeyCode.S) && !dooropen)
        {
            if (index > 0)
            {
                index--;
                setacc(index);

            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            keiteki.Play();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            agent.destination = forward.position;
            destext.text = "進行方向:↑";
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            agent.destination = back.position;
            destext.text = "進行方向:↓";
        }

    }
}
