using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SEND_POKE_CompariteSize
{
    ONE,
    TWO,
}

public class comparitionOfSize : MonoBehaviour {

    private static comparitionOfSize m_instance;
    public static comparitionOfSize instance
    {
        get
        {
            return m_instance;
        }
    }

    //UI分数
    public Text playerScore;
    public Text computerScore;

    //UI面板
    public GameObject m_winner;
    public GameObject m_loser;
    public GameObject m_showPokeAndComparitionSize;


    //玩家的21点分数为
    public int m_playerScore = 0;
    public int m_computerScore = 0;

    //创建两个发牌点
    public GameObject oneSpawn;
    public GameObject threeSpawn;
    public GameObject pokeRoot;

    //发牌顺序
    SEND_POKE_CompariteSize m_sendPokeCompariteSize = SEND_POKE_CompariteSize.ONE;

    //洗完牌的容器-没王
    public List<GameObject> m_pokeList_Flush_noKing;
    //洗完牌的容器-有王
    public List<GameObject> m_pokeList_Flush;

    //发牌间隔时间
    public float m_sendPokeDetaltTime = 0.25f;

    void Awake()
    {
        m_instance = this;
    }

	// Use this for initialization
	void Start () {
        //把之前洗好的牌放进单打独斗的容器里面
        m_pokeList_Flush_noKing = pokeManager.instance.m_pokeList_Flush_NoKing;
        m_pokeList_Flush = pokeManager.instance.m_pokeList_Flush;
        StartCoroutine("pokeRootHaveKing");
        StartCoroutine("sendPoke");
	}
    IEnumerator pokeRootHaveKing()
    {
        for (int i = 0; i < m_pokeList_Flush.Count; i++)
        {
            Destroy(m_pokeList_Flush[i].transform.gameObject);
        }
        StopCoroutine("pokeRootHaveKing");
        yield return 66;
    }
	IEnumerator sendPoke()
    {
        for(int i=0;i<2;i++)
        {
            switch (m_sendPokeCompariteSize)
            {
                case SEND_POKE_CompariteSize.ONE:
                    {
                        //朝哪里发牌？-玩家的节点减去发牌节点
                        Vector3 dir = oneSpawn.transform.position - transform.position;
                        //单位化角度
                        dir.Normalize();
                        m_pokeList_Flush_noKing[0].GetComponent<pokeObject>().sendPoke(dir);
                        //当发完一张牌后，移去容器里面的内存
                        m_pokeList_Flush_noKing.RemoveAt(0);
                        m_sendPokeCompariteSize = SEND_POKE_CompariteSize.TWO;
                    }
                    break;
                case SEND_POKE_CompariteSize.TWO:
                    {
                        //朝哪里发牌？-玩家的节点减去发牌节点
                        Vector3 dir = threeSpawn.transform.position - transform.position;
                        //单位化角度
                        dir.Normalize();
                        m_pokeList_Flush_noKing[0].GetComponent<pokeObject>().sendPoke(dir);
                        //当发完一张牌后，移去容器里面的内存
                        m_pokeList_Flush_noKing.RemoveAt(0);
                    }
                    break;
            }
            yield return new WaitForSeconds(m_sendPokeDetaltTime);
        }
        yield return new WaitForSeconds(1.5f);
        StartCoroutine("pokeRootFind");
    }
    //开启优化纸牌函数
    IEnumerator pokeRootFind()
    {
        for (int i = 0; i < m_pokeList_Flush_noKing.Count; i++)
        {
            m_pokeList_Flush_noKing[i].transform.parent = pokeRoot.transform;
        }
        playerScoredelate1(oneSpawn);
        playerScoredelate2(threeSpawn);
        StopCoroutine("pokeRootFind");
        yield return 66;
    }
	// Update is called once per frame
	void Update () {
        playerScore.text = "点数大小：" + m_playerScore;
	}

    public void playerScoredelate1(GameObject _spawn)
    {
        for (int i = 0; i < _spawn.transform.childCount; i++)
        {
            if (_spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize == 14)
            {
                _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize = 1;
            }
            if (_spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize == 15)
            {
                _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize = 2;
            }
            m_playerScore += _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize;
        }
    }
    public void playerScoredelate2(GameObject _spawn)
    {
        for (int i = 0; i < _spawn.transform.childCount; i++)
        {
            if (_spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize == 14)
            {
                _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize = 1;
            }
            if (_spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize == 15)
            {
                _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize = 2;
            }
            m_computerScore += _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize;
        }
    }

    public void complateOneAndThree()
    {
        computerScore.text = "电脑点数大小：" + m_computerScore;
        if (m_playerScore > m_computerScore && m_playerScore < 21 )
        {
            musicManager.instance.playEffectByName("winner");
            m_winner.SetActive(true);
        }
        if (m_playerScore < m_computerScore && m_computerScore < 21 || m_playerScore == m_computerScore)
        {
            musicManager.instance.playEffectByName("loser");
            m_loser.SetActive(true);
        }
    }
}
