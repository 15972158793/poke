using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicManager : MonoBehaviour {

    //创建单例
    private static musicManager m_instance;
    public static musicManager instance
    {
        get
        {
            return m_instance;
        }
    }


    //创建音乐音效
    private AudioSource m_audioSource;

    //创建音乐效果的数组
    public List<MusicObject> m_musicObject = new List<MusicObject>();
    void Awake()
    {
        m_instance = this;
    }

	// Use this for initialization
	void Start () {
        m_audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void playEffectByName(string _effectName)
    {
        for (int i = 0; i < m_musicObject.Count; i++)
        {
            if (m_musicObject[i].m_name == _effectName)
            {
                m_audioSource.PlayOneShot(m_musicObject[i].m_clip);
            }
        }
    }

}
