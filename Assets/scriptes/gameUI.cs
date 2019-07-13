using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameUI : MonoBehaviour {

    //创建游戏UI
    public GameObject m_comeBack;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onTheGameReturn()
    {
        m_comeBack.SetActive(true);
    }
    public void onTheGameBack()
    {
        m_comeBack.SetActive(false);
    }
}
