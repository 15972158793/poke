using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//了解前提：牌的精灵刚开始就是一张白纸
//所有的代码都是给牌面进行渲染-所有的牌的任何操作都是通过脚本跟组件进行操作
//进行洗牌，发牌
//由于它是洗牌体，所以具有所有脚本都能访问的功能-制作单例

public class pokeManager : MonoBehaviour {

    private static pokeManager m_instance;
    public static pokeManager instance
    {
        get
        {
            return m_instance;
        }
    }

    //单张牌的预制体
    public GameObject m_poke;

    //创建54张牌的数组
    public Sprite[] m_pokeArray;

    //洗完牌的容器，把所有牌都放进这个容器里面list（由于已经是帮体，所以用GamObject）
    public List<GameObject> m_pokeList_Flush = new List<GameObject>();

    //去掉两个王的洗牌
    public List<GameObject> m_pokeList_Flush_NoKing = new List<GameObject>();

    void Awake()
    {
        m_instance = this;
        flushPoke();
        noKingFlushPoke();
    }

	// Use this for initialization
	void Start () {
		//获取随机数的系统组件
        Random.InitState(System.Environment.TickCount);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //进行洗牌(洗牌的前提是牌已经赋予花色跟点数）
    public void flushPoke()
    {
        //总共54张牌
        for(int i=0;i<54;i++)
        {
            //创建每一个纸牌（Instantiate）-开始就是白纸（没有任何属性仅仅是单纯的精灵）一枚
            GameObject poke = Instantiate(m_poke, transform.position, Quaternion.identity);
            //利用牌的属性给每一张牌赋值
            poke.GetComponent<pokeObject>().initPoke(m_pokeArray[i], i);
            //把创建完的纸牌放入容器中-为后来的洗牌做准备
            m_pokeList_Flush.Add(poke);
        }
        //接下来用冒泡排序或选择排序进行洗牌
        for(int i=0;i<54;i++)
        {
            //随机一个数作为洗牌容器里面的下标-洗牌的原则就是打乱牌就可以，不管是任何方法
            int ranNumber = Random.Range(0, 54);
            GameObject randPoke = m_pokeList_Flush[ranNumber];
            m_pokeList_Flush[ranNumber] = m_pokeList_Flush[i];
            m_pokeList_Flush[i] = randPoke;
        }
    }
    public void noKingFlushPoke()
    {
        //总共54张牌,去掉两个王还有52张牌
        for (int i = 0; i < 52; i++)
        {
            //创建每一个纸牌（Instantiate）-开始就是白纸（没有任何属性仅仅是单纯的精灵）一枚
            GameObject poke = Instantiate(m_poke, transform.position, Quaternion.identity);
            //利用牌的属性给每一张牌赋值
            poke.GetComponent<pokeObject>().initPoke(m_pokeArray[i], i);
            //把创建完的纸牌放入容器中-为后来的洗牌做准备
            m_pokeList_Flush_NoKing.Add(poke);
        }
        //接下来用冒泡排序或选择排序进行洗牌
        for (int i = 0; i < 52; i++)
        {
            //随机一个数作为洗牌容器里面的下标-洗牌的原则就是打乱牌就可以，不管是任何方法
            int ranNumber = Random.Range(0, 52);
            GameObject randPoke = m_pokeList_Flush_NoKing[ranNumber];
            m_pokeList_Flush_NoKing[ranNumber] = m_pokeList_Flush_NoKing[i];
            m_pokeList_Flush_NoKing[i] = randPoke;
        }
    }
}
