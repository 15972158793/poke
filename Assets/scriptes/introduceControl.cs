using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;


public class introduceControl : MonoBehaviour {

    //先创建视频组件
    private VideoPlayer m_videoPlayer;

	// Use this for initialization
	void Start () {
        //在开始函数获取组件
        m_videoPlayer = GetComponent<VideoPlayer>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!m_videoPlayer.isPlaying)
        {
            SceneManager.LoadScene("gameMenu");
        }
        if(Input.anyKeyDown)
        {
            SceneManager.LoadScene("gameMenu");
        }
	}
}
