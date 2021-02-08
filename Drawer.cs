using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Drawer : MonoBehaviour
{
    public static Drawer instance;
    Dictionary<string, List<GameObject>> folds;
    Dictionary<string, List<GameObject>> outs;
    Dictionary<string, int> maxInstanceCount;
    public int DefaultMaxCount;
    public bool Log;
    float logTimer;
    void Start()
    {
        instance = this;
        if (DefaultMaxCount == 0)
        {
            DefaultMaxCount = 100;
        }
        maxInstanceCount = new Dictionary<string, int>();
        folds = new Dictionary<string, List<GameObject>>();
        outs = new Dictionary<string, List<GameObject>>();
    }

    void Update()
    {
        if (Log)
        {
            logTimer += Time.deltaTime;
            if(logTimer > 3)
            {
                logTimer = 0;
                PrintLog();
            }
        }
    }
    void PrintLog()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("=====DRAWER=====");
        foreach (string key in folds.Keys)
        {
            sb.Append("\n");              
            sb.Append("[" + key + "] ");
            sb.Append(outs[key].Count);
            sb.Append("/");
            sb.Append((folds[key].Count + outs[key].Count));
        }
        print(sb);
    }
    public void SetMaxInstanceCount(string prefabName, int num)
    {
        maxInstanceCount[prefabName] = num;
    }
    public GameObject Pull(string prefabName)
    {

        //폴즈에 키가 있으면
        if (folds.ContainsKey(prefabName))
        {
            List<GameObject> nonActives = folds[prefabName];
            List<GameObject> actives = outs[prefabName];
            //논액티브에 인스턴스가 있으면
            if (nonActives.Count > 0)
            {
                //논액티브에서 하나 꺼내온다
                GameObject go = nonActives[0];
                nonActives.RemoveAt(0);
                actives.Add(go);
                go.SetActive(true);
                return go;
            }
            //없으면
            else
            {
                //총 인스턴스 갯수가 maxium을 넘으면
                if (maxInstanceCount[prefabName] < actives.Count + nonActives.Count)
                {
                    //액티브에서 폴즈에 집어넣었다 바로 꺼내준다
                    GameObject go = actives[0];
                    //OnEnable 함수를 사용하기 위해 한번 비활성화 시켜준다
                    go.SetActive(false);
                    go.SetActive(true);
                    //인스턴스를 맨 뒤로 이동시키기 위해 제거했다 넣는다.
                    actives.RemoveAt(0);
                    actives.Add(go);
                    return go;
                }
                //총 인스턴스 갯수가 maxium보다 작으면
                else
                {
                    //하나 생성 후 액티브에 넣음
                    GameObject go = (GameObject)Instantiate(Resources.Load(prefabName));
                    actives.Add(go); 
                    return go;
                }
            }
        }
        //폴즈에 키가 없으면
        else
        {
            //프리팹 이름으로 키를 생성한다.
            folds[prefabName] = new List<GameObject>();
            outs[prefabName] = new List<GameObject>();
            maxInstanceCount[prefabName] = DefaultMaxCount;
            //재귀적으로 다시 리턴한다.
            return Pull(prefabName);
        }
    }


    public void Put(GameObject go)
    {
        string prefabName = go.name;
        List<GameObject> nonActives = folds[prefabName];
        List<GameObject> actives = outs[prefabName];
        //논액티브에 추가하고 액티브에서 제거
        nonActives.Add(go);
        actives.Remove(go);
        go.SetActive(false);
    }

}
