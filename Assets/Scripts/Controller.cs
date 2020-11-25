using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
    public AudioSource EB;
    public AudioSource ATS;
    bool dooropen = false;
    string[] stationname = { "三鷹", "吉祥寺", "西荻窪", "荻窪", "阿佐ヶ谷", "高円寺", "中野" };
    int staindex = 0;

    public float[] acc = {2.4f,1.8f,1.3f,0.9f,-0.01f,-0.3f,-0.9f,-1.5f,-2.1f,-2.8f,-3.6f,-4.2f,-7.3f};
    string[] modes = {"Mode[W][S]:P4", "Mode[W][S]:P3", "Mode[W][S]:P2", "Mode[W][S]:P1", "Mode[W][S]:N", "Mode[W][S]:B1", "Mode[W][S]:B2", "Mode[W][S]:B3", "Mode[W][S]:B4", "Mode[W][S]:B5", "Mode[W][S]:B6", "Mode[W][S]:B7", "Mode[W][S]:EB" };
    public float[] rate = { 1.0f, 0.95f, 0.93f, 0.91f, 0f, -0.89f, -0.92f, -0.95f, -0.95f, -0.975f, -0.98f, -0.99f, -1.0f };
    float[] seigen = { 30, 114514, 60, 114514 ,114514,114514};
    public Transform[] seigenobj = new Transform[4];
    int seigenindex = 0;

    public int index = 12;
    private Timer MyTimer;
    private NavMeshAgent agent;

    public Text acctext;
    public Text destext;
    public Text speedtext;
    public Text disttext;
    public Text timetext;
    public Text seigentext;

    public float timeOut=0.01f;
    bool reached90 = false;

    public AudioSource accsound,heisoku,rapid,keiteki;

    IEnumerator FuncCoroutine()
    {
        while (true)
        {
            double stadis = Vector3.Distance(agent.transform.position, staobj[staindex].transform.position);
            agent.speed += (acc[index] * timeOut);
            if (!reached90&&agent.speed*3.6>=90.0f&&index < 4)
            {
                index = 4;
                setacc(index);
                reached90 = true;
            }

            speedtext.text = "Speed:" + (agent.speed * 3.6f).ToString("F2") + "km/h";
            disttext.text = stationname[staindex] + "まで後" + stadis.ToString("F2") + "m";
            if (stadis < 4.0)
            {
                disttext.color = Color.blue;
            }else if(stadis < 300.0)
            {
                disttext.color = Color.green;
            }
            else
            {
                disttext.color = Color.black;
            }
            if (agent.speed == 0 && stadis < 4.0)
            {
                dooropen = true;
                arrive[staindex].Play();
                before = staindex;
                staindex++;
                stadis = Vector3.Distance(agent.transform.position, staobj[staindex].transform.position);
                len = stadis;
                housourunned = false;
            }
            if (len - stadis > 100 && !housourunned && staindex >= 1)
            {
                housou[staindex].Play();
                housourunned = true;
            }
            if (!arrive[before].isPlaying)
            {
                dooropen = false;
            }
            // 何か処理
            yield return new WaitForSeconds(timeOut);
        }
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        Screen.SetResolution(1280, 720, true, 60);
        agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;
        setacc(index);
        keiteki.Play();
        StartCoroutine(FuncCoroutine());

    }

    void setacc(int index)
    {
        acctext.text = this.modes[index];

        if (index!=4)
        {
            if(index > 4)
            {
                reached90 = false;
            }
            rapid.Pause();
            heisoku.Pause();
            accsound.Play();
            accsound.pitch = this.rate[index];
            accsound.time = accsound.clip.length * (agent.speed * 3.6f / 100f);

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

    bool ringing = false;
    void Update()
    {
        double seigendis = Vector3.Distance(agent.transform.position, seigenobj[seigenindex].transform.position);
        if (seigenindex<4&&seigendis < 5.0f)
        {
            seigentext.text = "制限:" + (seigen[seigenindex] == 114514 ? "-" : seigen[seigenindex].ToString());
            Color newCol;
            ColorUtility.TryParseHtmlString("#FFA500", out newCol);
            seigentext.color = (seigen[seigenindex] == 114514 ? Color.black : newCol);
            seigenindex++;
        }
        if (seigenindex-1>=0&&agent.speed * 3.6 > seigen[seigenindex-1])
        {
            speedtext.color = Color.red;
            if (!ringing)
            {
                ATS.time = 0;
                ATS.Play();
                ringing = true;
            }
        }
        else
        {
            speedtext.color = Color.black;
            if (ringing)
            {
                ATS.Stop();
                ringing = true;
            }
        }
        if (agent.speed == 0)
        {
            accsound.Stop();
        }
        if (index == 12)
        {
            acctext.color = Color.red;
        }
        else
        {
            acctext.color = dooropen ? Color.white : Color.black;
        }
        
        timetext.text = System.DateTime.Now.ToString("HH:mm:ss");

        if (Input.GetKeyDown(KeyCode.W) && !dooropen)
        {
            if (index < 12)
            {
                index++;
                setacc(index);
                if (index == 12) EB.Play();

            }

        }
        if (Input.GetKeyDown(KeyCode.S) && !dooropen)
        {
            if (index > 0)
            {
                if (index - 1 < 4 && agent.speed * 3.6 >= 90) return;
                index--;
                setacc(index);

            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            keiteki.Play();
        }
        if (agent.speed==0)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                agent.destination = forward.position;
                destext.text = "進行方向[↑][↓]:↑";
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                agent.destination = back.position;
                destext.text = "進行方向[↑][↓]:↓";
            }
        }

    }
}
