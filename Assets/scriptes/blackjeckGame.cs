using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//枚举两个发牌
public enum SEND_POKE_blackject
{
    ONE,
    TWO,
}


public class blackjeckGame : MonoBehaviour {

    private static blackjeckGame m_instance;
    public static blackjeckGame instance
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
    public GameObject m_addPoke;

    //每轮计分数
    private int m_currentScore = 0; 

    //玩家的21点分数为
    public int m_playerScore = 0;
    public int m_computerScore = 0;

    //创建两个发牌点
    public GameObject playerSpawn;
    public GameObject computerSpawn;
    public GameObject pokeRoot;

    //发牌顺序
    SEND_POKE_blackject m_sendPokeCompariteSize = SEND_POKE_blackject.ONE;

    //洗完牌的容器-没王
    public List<GameObject> m_pokeList_Flush_noKing;
    //洗完牌的容器-有王
    public List<GameObject> m_pokeList_Flush;

    //横纸牌之间的间隔大小
    public float HorizontalDetla = 0.5f;

    //发牌之间的间隔时间
    public float m_sendPokeDetaltTime = 0.25f;


    void Awake()
    {
        m_instance = this;
    }
    // Use this for initialization
    void Start()
    {
        //把之前洗好的牌放进单打独斗的容器里面
        m_pokeList_Flush_noKing = pokeManager.instance.m_pokeList_Flush_NoKing;
        m_pokeList_Flush = pokeManager.instance.m_pokeList_Flush;
        StartCoroutine("pokeRootHaveKing");
        StartCoroutine("sendPokeOnGameThree");
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
    IEnumerator sendPokeOnGameThree()
    {
        for (int i = 0; i < 4; i++)
        {
            switch (m_sendPokeCompariteSize)
            {
                case SEND_POKE_blackject.ONE:
                    {
                        //朝哪里发牌？-玩家的节点减去发牌节点
                        Vector3 dir = playerSpawn.transform.position - transform.position;
                        //单位化角度
                        dir.Normalize();
                        m_pokeList_Flush_noKing[0].GetComponent<pokeObject>().sendPoke(dir);
                        //当发完一张牌后，移去容器里面的内存
                        m_pokeList_Flush_noKing.RemoveAt(0);
                        musicManager.instance.playEffectByName("sendPoke");
                        m_sendPokeCompariteSize = SEND_POKE_blackject.TWO;
                    }
                    break;
                case SEND_POKE_blackject.TWO:
                    {
                        //朝哪里发牌？-玩家的节点减去发牌节点
                        Vector3 dir = computerSpawn.transform.position - transform.position;
                        //单位化角度
                        dir.Normalize();
                        m_pokeList_Flush_noKing[0].GetComponent<pokeObject>().sendPoke(dir);
                        //当发完一张牌后，移去容器里面的内存
                        m_pokeList_Flush_noKing.RemoveAt(0);
                        musicManager.instance.playEffectByName("sendPoke");
                        m_sendPokeCompariteSize = SEND_POKE_blackject.ONE;
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
        flushPlayerPoke(playerSpawn);
        flushPlayerPoke(computerSpawn);
        playerScoredelate1(playerSpawn);
        playerScoredelate2(computerSpawn);
        StopCoroutine("pokeRootFind");
        yield return 66;
    }
	void Update () {
        playerScore.text = "点数大小：" + m_playerScore;
	}
    // 给纸牌分隔开
    public void flushPlayerPoke(GameObject _playerSpawn)
    {

        //检查当前的牌的节点（节点在Unity里面就是一个容器）下有几张牌，利用transfom.childCount-就是查找当前它的下面有几个孩子
        //发到的牌的间隔隔开
        for (int i = 0; i < _playerSpawn.transform.childCount; i++)
        {
            //纸牌的在前在后顺序分清
            Vector3 newPos = new Vector3(_playerSpawn.transform.position.x + HorizontalDetla * i, _playerSpawn.transform.position.y);
            //把每一个创建的点都赋予给纸牌
            _playerSpawn.transform.GetChild(i).position = newPos;
            //根据索引值大小给纸牌排序
            _playerSpawn.transform.GetChild(i).GetComponent<pokeObject>().pokeSortOnIndex(i);
        }
    }

    //计算玩家的分数
    public void playerScoredelate1(GameObject _spawn)
    {
        m_currentScore = 0;
        for(int i=0;i<_spawn.transform.childCount;i++)
        {
            if (_spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize == 11 || _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize == 12 || _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize == 13)
            {
                _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize = 10;
            }
            if(_spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize == 14)
            {
                _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize = 1;
            }
            if (_spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize == 15)
            {
                _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize = 2;
            }
            m_currentScore += _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize;
        }
        m_playerScore = m_currentScore;
    }
    public void playerScoredelate2(GameObject _spawn)
    {
        m_currentScore = 0;
        for (int i = 0; i < _spawn.transform.childCount; i++)
        {
            if (_spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize == 11 || _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize == 12 || _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize == 13)
            {
                _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize = 10;
            }
            if (_spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize == 14)
            {
                _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize = 1;
            }
            if (_spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize == 15)
            {
                _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize = 2;
            }
            m_currentScore += _spawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize;
        }
        m_computerScore = m_currentScore;
    }

    public void sendPokeByPlayer()
    {
        //朝哪里发牌？-玩家的节点减去发牌节点
        Vector3 dir = playerSpawn.transform.position - transform.position;
        //单位化角度
        dir.Normalize();
        m_pokeList_Flush_noKing[0].GetComponent<pokeObject>().sendPoke(dir);
        //当发完一张牌后，移去容器里面的内存
        m_pokeList_Flush_noKing.RemoveAt(0);
        musicManager.instance.playEffectByName("sendPoke");
        StartCoroutine("flushPlayerPokeNextOne");
    }
    IEnumerator flushPlayerPokeNextOne()
    {
        yield return new WaitForSeconds(1.5f);
        playerScoredelate2(computerSpawn);
        flushPlayerPoke(computerSpawn);
        playerScoredelate1(playerSpawn);
        flushPlayerPoke(playerSpawn);
    }

    public void complatePlayerAndComputer()
    {
        if(m_computerScore < m_playerScore)
        {
            //朝哪里发牌？-玩家的节点减去发牌节点
            Vector3 dir = computerSpawn.transform.position - transform.position;
            //单位化角度
            dir.Normalize();
            m_pokeList_Flush_noKing[0].GetComponent<pokeObject>().sendPoke(dir);
            //当发完一张牌后，移去容器里面的内存
            m_pokeList_Flush_noKing.RemoveAt(0);
            musicManager.instance.playEffectByName("sendPoke");
            StartCoroutine("flushPlayerPokeNextOne");
        }
        if(m_playerScore < m_computerScore || m_computerScore > 21)
        {
            StartCoroutine("complateSize");
        }
    }
    IEnumerator complateSize()
    {
        yield return new WaitForSeconds(2.0f);
        computerScore.text = "电脑点数：" + m_computerScore;
        if (m_playerScore < m_computerScore && m_computerScore < 21 || m_playerScore == m_computerScore)
        {
            musicManager.instance.playEffectByName("loser");
            m_loser.SetActive(true);
        }
        if (m_playerScore > m_currentScore && m_playerScore < 21 || m_computerScore>21)
        {
            musicManager.instance.playEffectByName("winner");
            m_winner.SetActive(true);
        }
    }
}
