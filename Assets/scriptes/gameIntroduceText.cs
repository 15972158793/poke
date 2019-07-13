using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameIntroduceText : MonoBehaviour {

    //创建UI的Text
    public Text m_text;

    private float m_countTime;

	// Use this for initialization
	void Start () {
		//获取组件
        m_text.DOText("这是一款以娱乐为主题的纸牌游戏，它的操作简单易上手，深受广大群众的喜爱，希望老铁你玩了之后不要上瘾！！！", 15);
	}
	
	// Update is called once per frame
	void Update () {
        m_countTime += Time.deltaTime;
        if(m_countTime>15)
        {
            SceneManager.LoadScene("gameMenu");
        }
	}
}
