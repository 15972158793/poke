using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//纸牌的花色分为：方块（cude） 梅花（clubs） 红桃（heart） 黑桃（spade）
public enum POKE_TYPE
{
    CUDE,
    CLUBS,
    HEART,
    SPADE,
    NONE,
}

public class pokeObject : MonoBehaviour,IPointerDownHandler {

    //单张牌的精灵
    private Sprite m_poke;

    //获取当前牌面的精灵的渲染组件
    private SpriteRenderer m_spriteRenderer;

    //单张牌的背面
    public Sprite m_backPoke;

    //纸牌的刚体
    private Rigidbody2D m_rigidbody;

    //当前牌面的点数大小
    public int m_pokeSize;

    //默认花色是没有的
    public POKE_TYPE m_flowerColor = POKE_TYPE.NONE;

    //根据浮点数据记录花色的大小
    public float m_pokeTypeSize = 0;

    //发牌速度
    public float m_seedSpeed = 100;

    //是否点击到了牌（即牌是否被选中）
    public bool isSelected = false;

    //点击牌的偏移量（只有玩家具有，其他电脑玩家不能点击）
    public float m_selectedDetla = 0.5f;

    //初始当前牌的函数
    public void initPoke(Sprite _sprite,int index)
    {
        getPokeSizeByIndex(index);
        getPokeTypeByIndex(index);
        m_poke = _sprite;
    }

    //求出当前牌面的点数和花色
    //-、根据当前提供的牌的索引算出该牌的点数大小
    public int getPokeSizeByIndex(int index)
    {
        //当前的牌（大小王需要单独判断）
        if(index==52)
        {
            m_pokeSize = 16;
            return m_pokeSize;
        }
        if (index == 53)
        {
            m_pokeSize = 17;
            return m_pokeSize;
        }
        m_pokeSize = index % 13 + 3;
        return m_pokeSize;
    }
    //根据索引求出当前牌面的花色
    public POKE_TYPE getPokeTypeByIndex(int index)
    {
        //利用索引除以13，求出该牌为什么类型的花色
        //即：0为方块、1为梅花、2为红桃、3为黑桃、4就为大小王-啥也没有
        //利用状态机完成花色的分配
        int result = index / 13;
        switch(result)
        {
            case 0:
                m_flowerColor = POKE_TYPE.CUDE;
                break;
            case 1:
                m_flowerColor = POKE_TYPE.CLUBS;
                break;
            case 2:
                m_flowerColor = POKE_TYPE.HEART;
                break;
            case 3:
                m_flowerColor = POKE_TYPE.SPADE;
                break;
            case 4:
                m_flowerColor = POKE_TYPE.NONE;
                break;
        }
        return m_flowerColor;
    }
    //花色大小用float来分清-即数越大，花色越高
    //利用浮点数据，把花色的大小分清，数据越大即花色越高
    public float pokeTypeSize
    {
        //利用属性来只读花色的属性
        get
        {
            switch (m_flowerColor)
            {
                case POKE_TYPE.NONE:
                    m_pokeTypeSize = 0;
                    break;
                case POKE_TYPE.CUDE:
                    m_pokeTypeSize = 0.1f;
                    break;
                case POKE_TYPE.CLUBS:
                    m_pokeTypeSize = 0.2f;
                    break;
                case POKE_TYPE.HEART:
                    m_pokeTypeSize = 0.3f;
                    break;
                case POKE_TYPE.SPADE:
                    m_pokeTypeSize = 0.4f;
                    break;
            }
            return m_pokeTypeSize;
        }
    }

    //为了避免序列化数据的影响，所以牌面的精灵使用属性赋值
    public Sprite pokeSprite
    {
        set
        {
            m_poke = value;
        }
        get
        {
            return m_poke;
        }
    }

    //显示当前的牌面
    public void showPoke()
    {
        GetComponent<SpriteRenderer>().sprite = m_poke;
    }

    public void backPoke()
    {
        GetComponent<SpriteRenderer>().sprite = m_backPoke;
    }

    //利用属性，把牌面的int值（点数）跟牌面的float值（花色）相加，得出该牌的准确信息
    public float mixPokeSize
    {
        get
        {
            //返回两个值的相加
            return m_pokeSize + pokeTypeSize;
        }
    }

    //发牌点-朝哪个方向发牌
    public void sendPoke(Vector3 dir)
    {
        //发牌的方向以及发牌速度
        m_rigidbody.AddForce(new Vector3(dir.x * m_seedSpeed, dir.y * m_seedSpeed));
    }

    void Awake()
    {
        //对精灵渲染器和刚体的获得
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_rigidbody = GetComponent<Rigidbody2D>();
    }
	// Use this for initialization
	void Start () {
		
	}
	
    //update尽量保持空-因为它是不停的更新循环，里面放东西可能存在资源浪费
    //实在存在必要-选择协同函数
	// Update is called once per frame
	void Update () {
		
	}

    //扑克自己的渲染排序层顺序-层层覆盖的效果
    public void pokeSortOnIndex(int _index)
    {
        //顺序排序用精灵渲染器中的sortingOrder函数值，传入渲染就可以了
        m_spriteRenderer.sortingOrder = _index;
    }

    //利用GameObject（纸牌）是否碰到标签Spawn（发牌）
    public void OnTriggerEnter2D(Collider2D col)
    {
        //玩家自己的纸牌
        if (col.tag == "oneSpawn")
        {
            //当纸牌碰撞到标签，纸牌就马上停止，并且把纸牌翻开
            m_rigidbody.Sleep();
            //把节点变为父亲节点
            transform.parent = col.transform;
            showPoke();
        }
        //其他三个玩家的纸牌
        if (col.tag == "twoSpawn" || col.tag == "fourSpawn")
        {
            //仅仅需要发牌，先不翻牌
            m_rigidbody.Sleep();
            //但其他玩家的牌中二玩家跟四玩家需要把牌翻转90度
            //把节点变为父亲节点
            transform.parent = col.transform;
            rotatePoke();
        }
        //三玩家的牌
        if (col.tag == "threeSpawn")
        {
            //当纸牌碰撞到标签，纸牌就马上停止
            m_rigidbody.Sleep();
            //把节点变为父亲节点
            transform.parent = col.transform;
        }
        //玩家自己的纸牌
        if (col.tag == "playerSpawn")
        {
            //当纸牌碰撞到标签，纸牌就马上停止，并且把纸牌翻开
            m_rigidbody.Sleep();
            //把节点变为父亲节点
            transform.parent = col.transform;
            showPoke();
        }
        if (col.tag == "computerSpawn")
        {
            //当纸牌碰撞到标签，纸牌就马上停止
            m_rigidbody.Sleep();
            //把节点变为父亲节点
            transform.parent = col.transform;
        }
    }

    //扑克旋转90度-需要用到四维数Quaternion的Euler(new Vector3(0,0,0))要旋转哪一个跟多少度直接修改
    public void rotatePoke()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
    }

    //处理鼠标点击玩家手牌的时候，玩家的牌向上偏移(oneSpawn为玩家的发牌点)
    public void OnPointerDown(PointerEventData data)
    {
        //判断这个牌的点击为玩家
        if (transform.parent.tag == "oneSpawn")
        {
            //开关点击-当玩家点击到的时候，由true变为false
            isSelected = !isSelected;
            if(isSelected)
            {
                transform.Translate(0, m_selectedDetla, 0);
            }
            else
            {
                transform.Translate(0, -m_selectedDetla, 0);
            }
        }
    }
}
