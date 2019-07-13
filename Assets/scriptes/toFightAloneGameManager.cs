using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//出牌规则
public enum OUT_POKE_RULE
{
    SINGLE,
    PAIR,
    THREE,
    FOUR,
    ONE_DRAGON_three,
    ONE_DRAGON_four,
    ONE_DRAGON_five,
    ONE_DRAGON_six,
    ONE_DRAGON_seven,
    ONE_DRAGON_eight,
    ONE_DRAGON_nine,
    ONE_DRAGON_ten,
    ONE_DRAGON_eleven,
    ONE_DRAGON_twelve,
    PAIR_DRAGON_three,
    PAIR_DRAGON_four,
    PAIR_DRAGON_five,
    PAIR_DRAGON_six,
}

//发牌顺序-当牌洗完之后翻开第一张牌
//这一张牌的点数取余四，玩家的位置为（东1、南2、西3、北4）当算出的数是多少就谁先发牌
public enum SEND_POKE_SORT
{
    EAST,
    SOUTH,
    WEST,
    NORTH,
}

//出牌顺序
public enum OUT_POKE_SORT
{
    ONE_SPAWN,
    TWO_SPAWN,
    THREE_SPAWN,
    FOUR_SPAWN,
}

public class toFightAloneGameManager : MonoBehaviour {

    

    //默认主玩家的位置为东，二号玩家的位置为南，以此类推
    private SEND_POKE_SORT m_sendPokeSort;

    //枚举出牌顺序
    private OUT_POKE_SORT m_outPokeSort;

    //出牌规则
    public OUT_POKE_RULE m_outPokeRule;

    //打出的牌是否可以进入规则
    public bool isTrueAndDoIt = false;

    //记录四个发牌点
    public GameObject oneSpawn;
    public GameObject twoSpawn;
    public GameObject threeSpawn;
    public GameObject fourSpawn;

    //发牌的之间的间隔时间
    public float m_sendPokeDetlaTime = 0.05f;

    //洗完牌的容器
    public List<GameObject> m_pokeList_Flush;
    public List<GameObject> m_pokeList_Flush_NoKing;

    //横纸牌之间的间隔大小
    public float HorizontalDetla = 0.5f;

    //纵向纸牌之间的间隔大小
    public float VerticalDetla = 0.3f;

    //打出去的牌的大小
    private int m_outPokeCurrentSize = 0;
    //重新出牌的计算数
    private int m_currentNoWant = 0;

    //UI
    //出牌的面板
    public GameObject m_outPokePlane;
    //不要的精灵
    public GameObject m_noWant1;
    public GameObject m_noWant2;
    public GameObject m_noWant3;
    public GameObject m_noWant4;

    //是否不要
    private bool isOut = false;

    //出牌空节点-作为容器收集玩家出的牌
    public GameObject outPokeSpawn;
    public GameObject outPokeTwoSpawn;
    public GameObject outPokeThreeSpawn;
    public GameObject outPokeFourSpawn;

    //正常洗牌的节点根
    public GameObject m_pokeRootNoKing;


    //出牌的方向
    public OUT_POKE_SORT m_outPokeSpawn;

    //提示向上位移的纸牌
    public float m_selectedDetla = 0.5f;

	// Use this for initialization
	void Start () {
        //获取随机组件
        Random.InitState(System.Environment.TickCount);

		//把之前洗好的牌放进单打独斗的容器里面
        m_pokeList_Flush = pokeManager.instance.m_pokeList_Flush;
        m_pokeList_Flush_NoKing = pokeManager.instance.m_pokeList_Flush_NoKing;

        //没王的牌的节点根函数
        StartCoroutine("pokeRootNoKing");

        //开始发牌顺序函数
        sendPokeSort();

        //开始发牌-用协同函数
        StartCoroutine("sendPoke");
        //开启优化牌的排序
        StartCoroutine("flushAllPoke");
	}

    //没王纸牌节点根函数
    IEnumerator pokeRootNoKing()
    {
        for (int i = 0; i < m_pokeList_Flush_NoKing.Count; i++)
        {
            Destroy(m_pokeList_Flush_NoKing[i].transform.gameObject);
        }
        StopCoroutine("pokeRootFind");
        yield return 66;
    }

    //开启出牌顺序
    IEnumerator outPokeSort()
    {
        print("进入出牌排序");
        //循环每一个点，找出方块3的函数，方块3先出牌-3.3f为方块3
        float m_outPokeSortNum = 3.1f;
        for(int i=0;i<oneSpawn.transform.childCount;i++)
        {
            if (oneSpawn.transform.GetChild(i).GetComponent<pokeObject>().mixPokeSize == m_outPokeSortNum)
            {
                m_outPokeSort = OUT_POKE_SORT.ONE_SPAWN;
                print("出牌者为东方");
            }
        }
        for (int i = 0; i < twoSpawn.transform.childCount; i++)
        {
            if (twoSpawn.transform.GetChild(i).GetComponent<pokeObject>().mixPokeSize == m_outPokeSortNum)
            {
                m_outPokeSort = OUT_POKE_SORT.TWO_SPAWN;
                print("出牌者为南方");
            }
        }
        for (int i = 0; i < threeSpawn.transform.childCount; i++)
        {
            if (threeSpawn.transform.GetChild(i).GetComponent<pokeObject>().mixPokeSize == m_outPokeSortNum)
            {
                m_outPokeSort = OUT_POKE_SORT.THREE_SPAWN;
                print("出牌者为西方");
            }
        }
        for (int i = 0; i < fourSpawn.transform.childCount; i++)
        {
            if (fourSpawn.transform.GetChild(i).GetComponent<pokeObject>().mixPokeSize == m_outPokeSortNum)
            {
                m_outPokeSort = OUT_POKE_SORT.FOUR_SPAWN;
                print("出牌者为北方");
            }
        }
        yield return 66;
        StartCoroutine("outPokeSortByCudeThree");
    }
    IEnumerator outPokeSortByCudeThree()
    {
        yield return new WaitForSeconds(1.5f);
        switch(m_outPokeSort)
        {
            case OUT_POKE_SORT.ONE_SPAWN:
                StartCoroutine("oneSpawnOutPoke");
                break;
            case OUT_POKE_SORT.TWO_SPAWN:
                oneSpawnNoCudeThree(twoSpawn, outPokeTwoSpawn);
                StartCoroutine("threeSpawnOutPoke");
                break;
            case OUT_POKE_SORT.THREE_SPAWN:
                oneSpawnNoCudeThree(threeSpawn, outPokeThreeSpawn);
                StartCoroutine("fourSpawnOutPoke");
                break;
            case OUT_POKE_SORT.FOUR_SPAWN:
                oneSpawnNoCudeThree(fourSpawn, outPokeFourSpawn);
                StartCoroutine("oneSpawnOutPoke");
                break;

        }
        StopCoroutine("outPokeSortByCudeThree");
    }

    //所有牌的排序的协同
    IEnumerator flushAllPoke()
    {
        while(true)
        {
            flushOnePlayerPoke();
            flushTwoPlayerPoke();
            flushThreePlayerPoke();
            flushFourPlayerPoke();
            int pokeSize = oneSpawn.transform.childCount + twoSpawn.transform.childCount + threeSpawn.transform.childCount + fourSpawn.transform.childCount;
            if(pokeSize == 54)
            {
                break;
            }
            yield return null;
        }
        //结束协同
        StopCoroutine("flushAllPoke"); 
    }

    //发牌顺序
    public void sendPokeSort()
    {
        //随机一个牌再放回去，先翻牌给所有人看，之后放回去
        m_pokeList_Flush[0].GetComponent<pokeObject>().showPoke();
        int m_sendPokeRandom = Random.Range(0, 54);
        int sendPokeSortNum = m_pokeList_Flush[m_sendPokeRandom].GetComponent<pokeObject>().m_pokeSize;
        //在A和2的时候先发东位和南位，当随机牌为大小王的时候，重新选牌
        if (sendPokeSortNum == 14 || sendPokeSortNum == 15 || sendPokeSortNum == 16 || sendPokeSortNum == 17)
        {
            if(sendPokeSortNum == 14)
            {
                m_sendPokeSort = SEND_POKE_SORT.EAST;
            }
            if(sendPokeSortNum == 15)
            {
                m_sendPokeSort = SEND_POKE_SORT.SOUTH;
            }
            if(sendPokeSortNum==16)
            {
                sendPokeSortNum = Random.Range(0, 4);
                print(sendPokeSortNum);
                switch (sendPokeSortNum)
                {
                    case 0:
                        m_sendPokeSort = SEND_POKE_SORT.EAST;
                        break;
                    case 1:
                        m_sendPokeSort = SEND_POKE_SORT.SOUTH;
                        break;
                    case 2:
                        m_sendPokeSort = SEND_POKE_SORT.WEST;
                        break;
                    case 3:
                        m_sendPokeSort = SEND_POKE_SORT.NORTH;
                        break;
                }
            }
            if(sendPokeSortNum==17)
            {
                sendPokeSortNum = Random.Range(0, 4);
                print(sendPokeSortNum);
                switch (sendPokeSortNum)
                {
                    case 0:
                        m_sendPokeSort = SEND_POKE_SORT.EAST;
                        break;
                    case 1:
                        m_sendPokeSort = SEND_POKE_SORT.SOUTH;
                        break;
                    case 2:
                        m_sendPokeSort = SEND_POKE_SORT.WEST;
                        break;
                    case 3:
                        m_sendPokeSort = SEND_POKE_SORT.NORTH;
                        break;
                }
            }
        }
        else
        {
            if (sendPokeSortNum == 3 || sendPokeSortNum==4)
            {
                switch (sendPokeSortNum)
                {
                    case 3:
                        m_sendPokeSort = SEND_POKE_SORT.WEST;
                        break;
                    case 4:
                        m_sendPokeSort = SEND_POKE_SORT.NORTH;
                        break;
                }
            }
            else
            {
                switch (sendPokeSortNum % 4)
                {
                    case 0:
                        m_sendPokeSort = SEND_POKE_SORT.NORTH;
                        break;
                    case 1:
                        m_sendPokeSort = SEND_POKE_SORT.EAST;
                        break;
                    case 2:
                        m_sendPokeSort = SEND_POKE_SORT.SOUTH;
                        break;
                    case 3:
                        m_sendPokeSort = SEND_POKE_SORT.WEST;
                        break;
                }
            }  
        }
    }

    //发牌的协同函数
    IEnumerator sendPoke()
    {
        //总共有54张牌-因此需要利用循环来
        for(int i=0;i<54;i++)
        {
            //发牌最好用状态机
            switch(m_sendPokeSort)
            {
                case SEND_POKE_SORT.EAST:
                    {
                        //朝哪里发牌？-玩家的节点减去发牌节点
                        Vector3 dir = oneSpawn.transform.position - transform.position;
                        //单位化角度
                        dir.Normalize();
                        //开始按角度发牌
                        m_pokeList_Flush[0].GetComponent<pokeObject>().sendPoke(dir);
                        //当发完一张牌后，移去容器里面的内存
                        m_pokeList_Flush.RemoveAt(0);
                        //发完这个节点到下一个节点
                        m_sendPokeSort = SEND_POKE_SORT.SOUTH;
                        
                    }
                    break;
                case SEND_POKE_SORT.SOUTH:
                    {
                        //朝哪里发牌？-玩家的节点减去发牌节点
                        Vector3 dir = twoSpawn.transform.position - transform.position;
                        //单位化角度
                        dir.Normalize();
                        //开始按角度发牌
                        m_pokeList_Flush[0].GetComponent<pokeObject>().sendPoke(dir);
                        //当发完一张牌后，移去容器里面的内存
                        m_pokeList_Flush.RemoveAt(0);
                        //发完这个节点到下一个节点
                        m_sendPokeSort = SEND_POKE_SORT.WEST;
                    }
                    break;
                case SEND_POKE_SORT.WEST:
                    {
                        //朝哪里发牌？-玩家的节点减去发牌节点
                        Vector3 dir = threeSpawn.transform.position - transform.position;
                        //单位化角度
                        dir.Normalize();
                        //开始按角度发牌
                        m_pokeList_Flush[0].GetComponent<pokeObject>().sendPoke(dir);
                        //当发完一张牌后，移去容器里面的内存
                        m_pokeList_Flush.RemoveAt(0);
                        //发完这个节点到下一个节点
                        m_sendPokeSort = SEND_POKE_SORT.NORTH;
                    }
                    break;
                case SEND_POKE_SORT.NORTH:
                    {
                        //朝哪里发牌？-玩家的节点减去发牌节点
                        Vector3 dir = fourSpawn.transform.position - transform.position;
                        //单位化角度
                        dir.Normalize();
                        //开始按角度发牌
                        m_pokeList_Flush[0].GetComponent<pokeObject>().sendPoke(dir);
                        //当发完一张牌后，移去容器里面的内存
                        m_pokeList_Flush.RemoveAt(0);
                        //发完这个节点到下一个节点
                        m_sendPokeSort = SEND_POKE_SORT.EAST;
                    }
                    break;
            }
            //隔一定的时间在发一张牌
            yield return new WaitForSeconds(m_sendPokeDetlaTime);
        }
        //等待一定时间再开启下一个协同函数
        yield return new WaitForSeconds(3.0f);
        //开启排序协同
        StartCoroutine("outPokeSort");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //牌的排序的函数
    //给每个玩家的牌隔开
    public void flushOnePlayerPoke()
    {

        //检查当前的牌的节点（节点在Unity里面就是一个容器）下有几张牌，利用transfom.childCount-就是查找当前它的下面有几个孩子
        //发到的牌的间隔隔开
        for(int i=0;i<oneSpawn.transform.childCount;i++)
        {
            //纸牌的在前在后顺序分清
            Vector3 newPos = new Vector3(oneSpawn.transform.position.x + HorizontalDetla * i, oneSpawn.transform.position.y);
            //把每一个创建的点都赋予给纸牌
            oneSpawn.transform.GetChild(i).position = newPos;
            //根据索引值大小给纸牌排序
            oneSpawn.transform.GetChild(i).GetComponent<pokeObject>().pokeSortOnIndex(i);
        }
        //给牌面的大小排序牌，基于原理并用Unity自带的一种冒泡排序
        for(int i=0;i<oneSpawn.transform.childCount;i++)
        {
            for(int j=0;j<oneSpawn.transform.childCount;j++)
            {
                //创建两个转换数
                Transform child_1 = oneSpawn.transform.GetChild(i);
                Transform child_2 = oneSpawn.transform.GetChild(j);
                //根据两个数值的大小进行交换，（由小到大排列：i>j,反之）
                if(oneSpawn.transform.GetChild(j).GetComponent<pokeObject>().mixPokeSize > oneSpawn.transform.GetChild(i).GetComponent<pokeObject>().mixPokeSize)
                {
                    //Unity自带转换函数
                    child_1.SetSiblingIndex(j);
                    child_2.SetSiblingIndex(i);
                }
            }
        }
    }
    public void flushTwoPlayerPoke()
    {

        //检查当前的牌的节点（节点在Unity里面就是一个容器）下有几张牌，利用transfom.childCount-就是查找当前它的下面有几个孩子
        //发到的牌的间隔隔开
        for (int i = 0; i < twoSpawn.transform.childCount; i++)
        {
            //纸牌的在前在后顺序分清
            Vector3 newPos = new Vector3(twoSpawn.transform.position.x, twoSpawn.transform.position.y + VerticalDetla * i);
            //把每一个创建的点都赋予给纸牌
            twoSpawn.transform.GetChild(i).position = newPos;
            //根据索引值大小给纸牌排序
            twoSpawn.transform.GetChild(i).GetComponent<pokeObject>().pokeSortOnIndex(i);
        }
        //给牌面的大小排序牌，基于原理并用Unity自带的一种冒泡排序
        for (int i = 0; i < twoSpawn.transform.childCount; i++)
        {
            for (int j = 0; j < twoSpawn.transform.childCount; j++)
            {
                //创建两个转换数
                Transform child_1 = twoSpawn.transform.GetChild(i);
                Transform child_2 = twoSpawn.transform.GetChild(j);
                //根据两个数值的大小进行交换，（由小到大排列：i>j,反之）
                if (twoSpawn.transform.GetChild(j).GetComponent<pokeObject>().mixPokeSize > twoSpawn.transform.GetChild(i).GetComponent<pokeObject>().mixPokeSize)
                {
                    //Unity自带转换函数
                    child_1.SetSiblingIndex(j);
                    child_2.SetSiblingIndex(i);
                }
            }
        }
    }
    public void flushThreePlayerPoke()
    {

        //检查当前的牌的节点（节点在Unity里面就是一个容器）下有几张牌，利用transfom.childCount-就是查找当前它的下面有几个孩子
        //发到的牌的间隔隔开
        for (int i = 0; i < threeSpawn.transform.childCount; i++)
        {
            //纸牌的在前在后顺序分清
            Vector3 newPos = new Vector3(threeSpawn.transform.position.x - HorizontalDetla * i, threeSpawn.transform.position.y);
            //把每一个创建的点都赋予给纸牌
            threeSpawn.transform.GetChild(i).position = newPos;
            //根据索引值大小给纸牌排序
            threeSpawn.transform.GetChild(i).GetComponent<pokeObject>().pokeSortOnIndex(i);
        }
        //给牌面的大小排序牌，基于原理并用Unity自带的一种冒泡排序
        for (int i = 0; i < threeSpawn.transform.childCount; i++)
        {
            for (int j = 0; j < threeSpawn.transform.childCount; j++)
            {
                //创建两个转换数
                Transform child_1 = threeSpawn.transform.GetChild(i);
                Transform child_2 = threeSpawn.transform.GetChild(j);
                //根据两个数值的大小进行交换，（由小到大排列：i>j,反之）
                if (threeSpawn.transform.GetChild(j).GetComponent<pokeObject>().mixPokeSize > threeSpawn.transform.GetChild(i).GetComponent<pokeObject>().mixPokeSize)
                {
                    //Unity自带转换函数
                    child_1.SetSiblingIndex(j);
                    child_2.SetSiblingIndex(i);
                }
            }
        }
    }
    public void flushFourPlayerPoke()
    {

        //检查当前的牌的节点（节点在Unity里面就是一个容器）下有几张牌，利用transfom.childCount-就是查找当前它的下面有几个孩子
        //发到的牌的间隔隔开
        for (int i = 0; i < fourSpawn.transform.childCount; i++)
        {
            //纸牌的在前在后顺序分清
            Vector3 newPos = new Vector3(fourSpawn.transform.position.x, fourSpawn.transform.position.y - VerticalDetla * i);
            //把每一个创建的点都赋予给纸牌
            fourSpawn.transform.GetChild(i).position = newPos;
            //根据索引值大小给纸牌排序
            fourSpawn.transform.GetChild(i).GetComponent<pokeObject>().pokeSortOnIndex(i);
        }
        //给牌面的大小排序牌，基于原理并用Unity自带的一种冒泡排序
        for (int i = 0; i < fourSpawn.transform.childCount; i++)
        {
            for (int j = 0; j < fourSpawn.transform.childCount; j++)
            {
                //创建两个转换数
                Transform child_1 = fourSpawn.transform.GetChild(i);
                Transform child_2 = fourSpawn.transform.GetChild(j);
                //根据两个数值的大小进行交换，（由小到大排列：i>j,反之）
                if (fourSpawn.transform.GetChild(j).GetComponent<pokeObject>().mixPokeSize > fourSpawn.transform.GetChild(i).GetComponent<pokeObject>().mixPokeSize)
                {
                    //Unity自带转换函数
                    child_1.SetSiblingIndex(j);
                    child_2.SetSiblingIndex(i);
                }
            }
        }
    }

    //主玩家出牌
    public void oneSpawnHaveCudeThree()
    {
        //牌型规则-牌型不对，不允许打牌-利用bool值来判断是否属于牌型规则
        bool isRule = false;

        //当玩家手中有方块3-出牌顺序为玩家（东）
        //当出牌的时候，判断选中的牌是否是已经选中了-选中既是容器
        List<GameObject> selectList = new List<GameObject>();
        for (int i = 0; i < oneSpawn.transform.childCount; i++)
        {
            //如果是被选中的牌
            if (oneSpawn.transform.GetChild(i).GetComponent<pokeObject>().isSelected)
            {
                //获取玩家选取的牌作为容器的内容
                selectList.Add(oneSpawn.transform.GetChild(i).gameObject);
            }
        }
        //判断所选中的牌是否符合出牌规则
        //打单张的时候，一定是single
        if (selectList.Count == 1)
        {
            m_outPokeRule = OUT_POKE_RULE.SINGLE;
            //打出去的牌为容器首牌
            m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
            print(m_outPokeCurrentSize);
            //单张
            isRule = true;
        }
        //打两张牌的时候
        if (selectList.Count == 2)
        {
            int pokeSize1 = selectList[0].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize2 = selectList[1].GetComponent<pokeObject>().m_pokeSize;
            if(pokeSize1==pokeSize2)
            {
                m_outPokeRule = OUT_POKE_RULE.PAIR;
                //把值赋予给规则单牌
                m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
                isRule = true;
            }
        }
        //打三张牌
        if (selectList.Count == 3)
        {
            int pokeSize1 = selectList[0].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize2 = selectList[1].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize3 = selectList[2].GetComponent<pokeObject>().m_pokeSize;
            if (pokeSize1 == pokeSize2&&pokeSize2 ==pokeSize3)
            {
                m_outPokeRule = OUT_POKE_RULE.THREE;
                //把值赋予给规则单牌
                m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
                //符合打牌规则
                isRule = true;
            }
            if(pokeSize1 + 1 == pokeSize2 
                && pokeSize2 + 1 == pokeSize3)
            {
                if(pokeSize3 != 15)
                {
                    m_outPokeRule = OUT_POKE_RULE.ONE_DRAGON_three;
                    m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
                    isRule = true;
                }
            }
        }
        //打4张牌
        if (selectList.Count == 4)
        {
            int pokeSize1 = selectList[0].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize2 = selectList[1].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize3 = selectList[2].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize4 = selectList[3].GetComponent<pokeObject>().m_pokeSize;
            //4个相同的牌
            if (pokeSize1 == pokeSize2 
                && pokeSize2 == pokeSize3 
                && pokeSize3 == pokeSize4)
            {
                m_outPokeRule = OUT_POKE_RULE.FOUR;
                //把值赋予给规则单牌
                m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
                isRule = true;
            }
            //顺子
            if (pokeSize1 + 1 == pokeSize2
                && pokeSize2 + 1 == pokeSize3
                && pokeSize3 + 1 == pokeSize4)
            {
                if(pokeSize4 != 15)
                {
                    m_outPokeRule = OUT_POKE_RULE.ONE_DRAGON_four;
                    m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
                    isRule = true;
                }
            }
        }
        //打5张牌
        if(selectList.Count == 5)
        {
            int pokeSize1 = selectList[0].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize2 = selectList[1].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize3 = selectList[2].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize4 = selectList[3].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize5 = selectList[4].GetComponent<pokeObject>().m_pokeSize;
            if (pokeSize1 + 1 == pokeSize2 
                && pokeSize2 + 1 == pokeSize3 
                && pokeSize3 + 1 == pokeSize4
                && pokeSize4 + 1 == pokeSize5)
            {
                if(pokeSize5 != 15)
                {
                    m_outPokeRule = OUT_POKE_RULE.ONE_DRAGON_five;
                    m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
                    isRule = true;
                }
            }
        }
        //打6张牌
        if(selectList.Count == 6)
        {
            int pokeSize1 = selectList[0].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize2 = selectList[1].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize3 = selectList[2].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize4 = selectList[3].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize5 = selectList[4].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize6 = selectList[5].GetComponent<pokeObject>().m_pokeSize;
            if(pokeSize1 == pokeSize2
                && pokeSize1 + 1 == pokeSize3
                && pokeSize3 == pokeSize4
                && pokeSize3 + 1 == pokeSize5
                && pokeSize5 == pokeSize6)
            {
                if(pokeSize5 != 15)
                {
                    m_outPokeRule = OUT_POKE_RULE.PAIR_DRAGON_three;
                    m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
                    isRule = true;
                }
            }
            if (pokeSize1 + 1 == pokeSize2
                && pokeSize2 + 1 == pokeSize3
                && pokeSize3 + 1 == pokeSize4
                && pokeSize4 + 1 == pokeSize5
                && pokeSize5 + 1 == pokeSize6)
            {
                if (pokeSize6 != 15)
                {
                    m_outPokeRule = OUT_POKE_RULE.ONE_DRAGON_six;
                    m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
                    isRule = true;
                }                
            }
        }
        //打7张牌
        if(selectList.Count == 7)
        {
            int pokeSize1 = selectList[0].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize2 = selectList[1].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize3 = selectList[2].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize4 = selectList[3].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize5 = selectList[4].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize6 = selectList[5].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize7 = selectList[6].GetComponent<pokeObject>().m_pokeSize;
            if (pokeSize1 + 1 == pokeSize2
                && pokeSize2 + 1 == pokeSize3
                && pokeSize3 + 1 == pokeSize4
                && pokeSize4 + 1 == pokeSize5
                && pokeSize5 + 1 == pokeSize6
                && pokeSize6 + 1 == pokeSize7)
            {
                if (pokeSize7 != 15)
                {
                    m_outPokeRule = OUT_POKE_RULE.ONE_DRAGON_seven;
                    m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
                    isRule = true;
                }
            }
        }
        //打8张牌
        if (selectList.Count == 8)
        {
            int pokeSize1 = selectList[0].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize2 = selectList[1].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize3 = selectList[2].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize4 = selectList[3].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize5 = selectList[4].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize6 = selectList[5].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize7 = selectList[6].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize8 = selectList[7].GetComponent<pokeObject>().m_pokeSize;
            //连对
            if (pokeSize1 == pokeSize2
                && pokeSize1 + 1 == pokeSize3
                && pokeSize3 == pokeSize4
                && pokeSize3 + 1 == pokeSize5
                && pokeSize5 == pokeSize6
                && pokeSize5 + 1 == pokeSize7
                && pokeSize7 == pokeSize8)
            {
                if (pokeSize7 != 15)
                {
                    m_outPokeRule = OUT_POKE_RULE.PAIR_DRAGON_four;
                    m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
                    isRule = true;
                }
            }
            //顺子
            if (pokeSize1 + 1 == pokeSize2
                && pokeSize2 + 1 == pokeSize3
                && pokeSize3 + 1 == pokeSize4
                && pokeSize4 + 1 == pokeSize5
                && pokeSize5 + 1 == pokeSize6
                && pokeSize6 + 1 == pokeSize7
                && pokeSize7 + 1 == pokeSize8)
            {
                if (pokeSize8 != 15)
                {
                    m_outPokeRule = OUT_POKE_RULE.ONE_DRAGON_eight;
                    m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
                    isRule = true;
                }
            }
        }
        //打9张牌
        if (selectList.Count == 9)
        {
            int pokeSize1 = selectList[0].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize2 = selectList[1].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize3 = selectList[2].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize4 = selectList[3].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize5 = selectList[4].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize6 = selectList[5].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize7 = selectList[6].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize8 = selectList[7].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize9 = selectList[8].GetComponent<pokeObject>().m_pokeSize;
            //顺子
            if (pokeSize1 + 1 == pokeSize2
                && pokeSize2 + 1 == pokeSize3
                && pokeSize3 + 1 == pokeSize4
                && pokeSize4 + 1 == pokeSize5
                && pokeSize5 + 1 == pokeSize6
                && pokeSize6 + 1 == pokeSize7
                && pokeSize7 + 1 == pokeSize8
                && pokeSize8 + 1 == pokeSize9)
            {
                if (pokeSize9 != 15)
                {
                    m_outPokeRule = OUT_POKE_RULE.ONE_DRAGON_nine;
                    m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
                    isRule = true;
                }
            }
        }
        //打10张牌
        if (selectList.Count == 10)
        {
            int pokeSize1 = selectList[0].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize2 = selectList[1].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize3 = selectList[2].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize4 = selectList[3].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize5 = selectList[4].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize6 = selectList[5].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize7 = selectList[6].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize8 = selectList[7].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize9 = selectList[8].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize10 = selectList[9].GetComponent<pokeObject>().m_pokeSize;
            //连对
            if (pokeSize1 == pokeSize2
                && pokeSize1 + 1 == pokeSize3
                && pokeSize3 == pokeSize4
                && pokeSize3 + 1 == pokeSize5
                && pokeSize5 == pokeSize6
                && pokeSize5 + 1 == pokeSize7
                && pokeSize7 == pokeSize8
                && pokeSize7 + 1 == pokeSize9
                && pokeSize9 == pokeSize10)
            {
                if (pokeSize10 != 15)
                {
                    m_outPokeRule = OUT_POKE_RULE.PAIR_DRAGON_five;
                    m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
                    isRule = true;
                }
            }
            //顺子
            if (pokeSize1 + 1 == pokeSize2
                && pokeSize2 + 1 == pokeSize3
                && pokeSize3 + 1 == pokeSize4
                && pokeSize4 + 1 == pokeSize5
                && pokeSize5 + 1 == pokeSize6
                && pokeSize6 + 1 == pokeSize7
                && pokeSize7 + 1 == pokeSize8
                && pokeSize8 + 1 == pokeSize9
                && pokeSize9 + 1 == pokeSize10)
            {
                if (pokeSize10 != 15)
                {
                    m_outPokeRule = OUT_POKE_RULE.ONE_DRAGON_ten;
                    m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
                    isRule = true;
                }
            }
        }
        //打11张牌
        if (selectList.Count == 11)
        {
            int pokeSize1 = selectList[0].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize2 = selectList[1].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize3 = selectList[2].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize4 = selectList[3].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize5 = selectList[4].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize6 = selectList[5].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize7 = selectList[6].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize8 = selectList[7].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize9 = selectList[8].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize10 = selectList[9].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize11 = selectList[10].GetComponent<pokeObject>().m_pokeSize;
            //顺子
            if (pokeSize1 + 1 == pokeSize2
                && pokeSize2 + 1 == pokeSize3
                && pokeSize3 + 1 == pokeSize4
                && pokeSize4 + 1 == pokeSize5
                && pokeSize5 + 1 == pokeSize6
                && pokeSize6 + 1 == pokeSize7
                && pokeSize7 + 1 == pokeSize8
                && pokeSize8 + 1 == pokeSize9
                && pokeSize9 + 1 == pokeSize10
                && pokeSize10 + 1 == pokeSize11)
            {
                if (pokeSize11 != 15)
                {
                    m_outPokeRule = OUT_POKE_RULE.ONE_DRAGON_eleven;
                    m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
                    isRule = true;
                }
            }
        }
        //打12张牌
        if (selectList.Count == 12)
        {
            int pokeSize1 = selectList[0].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize2 = selectList[1].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize3 = selectList[2].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize4 = selectList[3].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize5 = selectList[4].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize6 = selectList[5].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize7 = selectList[6].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize8 = selectList[7].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize9 = selectList[8].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize10 = selectList[9].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize11 = selectList[10].GetComponent<pokeObject>().m_pokeSize;
            int pokeSize12 = selectList[11].GetComponent<pokeObject>().m_pokeSize;
            //连对
            if (pokeSize1 == pokeSize2
                && pokeSize1 + 1 == pokeSize3
                && pokeSize3 == pokeSize4
                && pokeSize3 + 1 == pokeSize5
                && pokeSize5 == pokeSize6
                && pokeSize5 + 1 == pokeSize7
                && pokeSize7 == pokeSize8
                && pokeSize7 + 1 == pokeSize9
                && pokeSize9 == pokeSize10
                && pokeSize9 + 1 == pokeSize11
                && pokeSize11 == pokeSize12)
            {
                if (pokeSize12 != 15)
                {
                    m_outPokeRule = OUT_POKE_RULE.PAIR_DRAGON_six;
                    m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
                    isRule = true;
                }
            }
            //顺子
            if (pokeSize1 + 1 == pokeSize2
                && pokeSize2 + 1 == pokeSize3
                && pokeSize3 + 1 == pokeSize4
                && pokeSize4 + 1 == pokeSize5
                && pokeSize5 + 1 == pokeSize6
                && pokeSize6 + 1 == pokeSize7
                && pokeSize7 + 1 == pokeSize8
                && pokeSize8 + 1 == pokeSize9
                && pokeSize9 + 1 == pokeSize10
                && pokeSize10 + 1 == pokeSize11
                && pokeSize11 + 1 == pokeSize12)
            {
                if (pokeSize12 != 15)
                {
                    m_outPokeRule = OUT_POKE_RULE.ONE_DRAGON_twelve;
                    m_outPokeCurrentSize = selectList[0].GetComponent<pokeObject>().m_pokeSize;
                    isRule = true;
                }
            }
        }
        if(isRule)
        {
            //循环把打出去的牌毁了
            for (int i = 0; i < outPokeSpawn.transform.childCount;i++)
            {
                Destroy(outPokeSpawn.transform.GetChild(i).gameObject);
            }
            //声音
            switch (m_outPokeRule)
            {
                case OUT_POKE_RULE.SINGLE:
                    {
                        //创建音效点
                        sigleMusic(m_outPokeCurrentSize);
                    }
                    break;
                case OUT_POKE_RULE.PAIR:
                    {
                        //创建音效点
                        pairMusic(m_outPokeCurrentSize);
                    }
                    break;
                case OUT_POKE_RULE.THREE:
                    {
                        musicManager.instance.playEffectByName("three");
                    }
                    break;
                case OUT_POKE_RULE.FOUR:
                    {
                        musicManager.instance.playEffectByName("four");
                    }
                    break;
                case OUT_POKE_RULE.ONE_DRAGON_three:
                    {
                        musicManager.instance.playEffectByName("one_Gargon");
                    }
                    break;
                case OUT_POKE_RULE.ONE_DRAGON_four:
                    {
                        musicManager.instance.playEffectByName("one_Gargon");
                    }
                    break;
                case OUT_POKE_RULE.ONE_DRAGON_five:
                    {
                        musicManager.instance.playEffectByName("one_Gargon");
                    }
                    break;
                case OUT_POKE_RULE.ONE_DRAGON_six:
                    {
                        musicManager.instance.playEffectByName("one_Gargon");
                    }
                    break;
                case OUT_POKE_RULE.ONE_DRAGON_seven:
                    {
                        musicManager.instance.playEffectByName("one_Gargon");
                    }
                    break;
                case OUT_POKE_RULE.ONE_DRAGON_eight:
                    {
                        musicManager.instance.playEffectByName("one_Gargon");
                    }
                    break;
                case OUT_POKE_RULE.ONE_DRAGON_nine:
                    {
                        musicManager.instance.playEffectByName("one_Gargon");
                    }
                    break;
                case OUT_POKE_RULE.ONE_DRAGON_ten:
                    {
                        musicManager.instance.playEffectByName("one_Gargon");
                    }
                    break;
                case OUT_POKE_RULE.ONE_DRAGON_eleven:
                    {
                        musicManager.instance.playEffectByName("one_Gargon");
                    }
                    break;
                case OUT_POKE_RULE.ONE_DRAGON_twelve:
                    {
                        musicManager.instance.playEffectByName("one_Gargon");
                    }
                    break;
                case OUT_POKE_RULE.PAIR_DRAGON_three:
                    {
                        musicManager.instance.playEffectByName("pairLine");
                    }
                    break;
                case OUT_POKE_RULE.PAIR_DRAGON_four:
                    {
                        musicManager.instance.playEffectByName("pairLine");
                    }
                    break;
                case OUT_POKE_RULE.PAIR_DRAGON_five:
                    {
                        musicManager.instance.playEffectByName("pairLine");
                    }
                    break;
                case OUT_POKE_RULE.PAIR_DRAGON_six:
                    {
                        musicManager.instance.playEffectByName("pairLine");
                    }
                    break;
            }
            //玩家出牌了，关闭出牌框
            //上家出完下家出牌
            //把出的牌放置到主玩家的上方outPokeSpawn
            for (int i = 0; i < selectList.Count; i++)
            {
                selectList[i].transform.position = new Vector3(outPokeSpawn.transform.position.x + i * HorizontalDetla, outPokeSpawn.transform.position.y);
                selectList[i].transform.parent = outPokeSpawn.transform;
            }
            //把剩余的牌进行排序
            flushOnePlayerPoke();

            //出完牌就排序
            m_outPokePlane.SetActive(false);
            m_outPokeSort = OUT_POKE_SORT.TWO_SPAWN;
            StartCoroutine("twoSpawnOutPoke");
        }
    }
    //南位玩家出牌
    IEnumerator twoSpawnOutPoke()
    {
        m_outPokeSpawn = OUT_POKE_SORT.TWO_SPAWN;
        yield return new WaitForSeconds(2.0f);
        //利用条件语句显示上一位的状态
        switch(m_outPokeRule)
        {
                //规则为单张时：
            case OUT_POKE_RULE.SINGLE:
                singleOutPoke(twoSpawn, outPokeTwoSpawn);
                break;
                //规则为对时：
            case OUT_POKE_RULE.PAIR:
                pairOutPoke(twoSpawn, outPokeTwoSpawn);
                break;
            case OUT_POKE_RULE.THREE:
                threeOutPoke(twoSpawn, outPokeTwoSpawn);
                break;
            case  OUT_POKE_RULE.FOUR:
                fourOutPoke(twoSpawn, outPokeTwoSpawn);
                break;
                //顺子
            case OUT_POKE_RULE.ONE_DRAGON_three:
                threeOutPokeOneDargon(twoSpawn, outPokeTwoSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_four:
                fourOutPokeOneDargon(twoSpawn, outPokeTwoSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_five:
                fiveOutPokeOneDargon(twoSpawn, outPokeTwoSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_six:
                sixOutPokeOneDargon(twoSpawn, outPokeTwoSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_seven:
                sevenOutPokeOneDargon(twoSpawn, outPokeTwoSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_eight:
                eightOutPokeOneDargon(twoSpawn, outPokeTwoSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_nine:
                nineOutPokeOneDargon(twoSpawn, outPokeTwoSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_ten:
                tenOutPokeOneDargon(twoSpawn, outPokeTwoSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_eleven:
                elevenOutPokeOneDargon(twoSpawn, outPokeTwoSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_twelve:
                twelveOutPokeOneDargon(twoSpawn, outPokeTwoSpawn);
                break;
                //连对
            case OUT_POKE_RULE.PAIR_DRAGON_three:
                threePairDargonOutPoke(twoSpawn, outPokeTwoSpawn);
                break;
            case OUT_POKE_RULE.PAIR_DRAGON_four:
                fourPairDargonOutPoke(twoSpawn, outPokeTwoSpawn);
                break;
            case OUT_POKE_RULE.PAIR_DRAGON_five:
                fivePairDargonOutPoke(twoSpawn, outPokeTwoSpawn);
                break;
            case OUT_POKE_RULE.PAIR_DRAGON_six:
                sixPairDargonOutPoke(twoSpawn, outPokeTwoSpawn);
                break;
        }
        //给打牌了的玩家洗一次牌
        flushTwoPlayerPoke();
        yield return new WaitForSeconds(2.0f); 
        //开始西位玩家出牌
        StartCoroutine("threeSpawnOutPoke");
    }
    //西位玩家出牌
    IEnumerator threeSpawnOutPoke()
    {
        m_outPokeSpawn = OUT_POKE_SORT.THREE_SPAWN;
        //利用条件语句显示上一位的状态
        switch (m_outPokeRule)
        {
            case OUT_POKE_RULE.SINGLE:
                singleOutPoke(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount;i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
            case OUT_POKE_RULE.PAIR:
                pairOutPoke(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount; i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
            case OUT_POKE_RULE.THREE:
                threeOutPoke(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount; i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
            case OUT_POKE_RULE.FOUR:
                fourOutPoke(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount; i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
                //顺子
            case OUT_POKE_RULE.ONE_DRAGON_three:
                threeOutPokeOneDargon(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount; i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
            case OUT_POKE_RULE.ONE_DRAGON_four:
                fourOutPokeOneDargon(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount; i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
            case OUT_POKE_RULE.ONE_DRAGON_five:
                fiveOutPokeOneDargon(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount; i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
            case OUT_POKE_RULE.ONE_DRAGON_six:
                sixOutPokeOneDargon(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount; i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
            case OUT_POKE_RULE.ONE_DRAGON_seven:
                sevenOutPokeOneDargon(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount; i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
            case OUT_POKE_RULE.ONE_DRAGON_eight:
                eightOutPokeOneDargon(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount; i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
            case OUT_POKE_RULE.ONE_DRAGON_nine:
                nineOutPokeOneDargon(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount; i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
            case OUT_POKE_RULE.ONE_DRAGON_ten:
                tenOutPokeOneDargon(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount; i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
            case OUT_POKE_RULE.ONE_DRAGON_eleven:
                elevenOutPokeOneDargon(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount; i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
            case OUT_POKE_RULE.ONE_DRAGON_twelve:
                twelveOutPokeOneDargon(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount; i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
                //连对
            case OUT_POKE_RULE.PAIR_DRAGON_three:
                threePairDargonOutPoke(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount; i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
            case OUT_POKE_RULE.PAIR_DRAGON_four:
                fourPairDargonOutPoke(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount; i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
            case OUT_POKE_RULE.PAIR_DRAGON_five:
                fivePairDargonOutPoke(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount; i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
            case OUT_POKE_RULE.PAIR_DRAGON_six:
                sixPairDargonOutPoke(threeSpawn, outPokeThreeSpawn);
                for (int i = 0; i < outPokeThreeSpawn.transform.childCount; i++)
                {
                    outPokeThreeSpawn.transform.GetChild(i).gameObject.transform.position = new Vector3(outPokeThreeSpawn.transform.position.x + HorizontalDetla * i, outPokeThreeSpawn.transform.position.y);
                }
                break;
        }
        flushThreePlayerPoke();
        yield return new WaitForSeconds(2);
        //开始北位玩家出牌
        StartCoroutine("fourSpawnOutPoke");
    }
    //北位玩家出牌
    IEnumerator fourSpawnOutPoke()
    {
        m_outPokeSpawn = OUT_POKE_SORT.FOUR_SPAWN;
        //利用条件语句显示上一位的状态
        switch (m_outPokeRule)
        {
            case OUT_POKE_RULE.SINGLE:
                singleOutPoke(fourSpawn, outPokeFourSpawn);
                break;
            case OUT_POKE_RULE.PAIR:
                pairOutPoke(fourSpawn, outPokeFourSpawn);
                break;
            case OUT_POKE_RULE.THREE:
                threeOutPoke(fourSpawn, outPokeFourSpawn);
                break;
            case OUT_POKE_RULE.FOUR:
                fourOutPoke(fourSpawn, outPokeFourSpawn);
                break;
                //顺子
            case OUT_POKE_RULE.ONE_DRAGON_three:
                threeOutPokeOneDargon(fourSpawn, outPokeFourSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_four:
                fourOutPokeOneDargon(fourSpawn, outPokeFourSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_five:
                fiveOutPokeOneDargon(fourSpawn, outPokeFourSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_six:
                sixOutPokeOneDargon(fourSpawn, outPokeFourSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_seven:
                sevenOutPokeOneDargon(fourSpawn, outPokeFourSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_eight:
                eightOutPokeOneDargon(fourSpawn, outPokeFourSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_nine:
                nineOutPokeOneDargon(fourSpawn, outPokeFourSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_ten:
                tenOutPokeOneDargon(fourSpawn, outPokeFourSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_eleven:
                elevenOutPokeOneDargon(fourSpawn, outPokeFourSpawn);
                break;
            case OUT_POKE_RULE.ONE_DRAGON_twelve:
                twelveOutPokeOneDargon(fourSpawn, outPokeFourSpawn);
                break;
                //连对
            case OUT_POKE_RULE.PAIR_DRAGON_three:
                threePairDargonOutPoke(fourSpawn, outPokeFourSpawn);
                break;
            case OUT_POKE_RULE.PAIR_DRAGON_four:
                fourPairDargonOutPoke(fourSpawn, outPokeFourSpawn);
                break;
            case OUT_POKE_RULE.PAIR_DRAGON_five:
                fivePairDargonOutPoke(fourSpawn, outPokeFourSpawn);
                break;
            case OUT_POKE_RULE.PAIR_DRAGON_six:
                sixPairDargonOutPoke(fourSpawn, outPokeFourSpawn);
                break;
        }
        flushFourPlayerPoke();
        yield return new WaitForSeconds(2);
        //开始东位玩家出牌
        StartCoroutine("oneSpawnOutPoke");
    }
    //东位玩家出牌
    IEnumerator oneSpawnOutPoke()
    {

        m_outPokeSpawn = OUT_POKE_SORT.ONE_SPAWN;
        m_noWant1.SetActive(false);
        m_outPokePlane.SetActive(true);
        yield return 66;
    }

    //打单张的函数
    private void singleOutPoke(GameObject _playerSpawn,GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerSpawn.transform.childCount;i++ )
        {
            if (_playerSpawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize)
            {
                noWantUIFalse(m_outPokeSpawn);
                isOut = true;
                //把打出来的牌放到outPokeTwoSpawn位置上
                _playerSpawn.transform.GetChild(i).position = _outSpawn.transform.position;
                //打出来的牌翻开
                _playerSpawn.transform.GetChild(i).GetComponent<pokeObject>().showPoke();
                //把打出来的牌的大小赋予在公用的单牌上，方便后面玩家识别并打牌
                m_outPokeCurrentSize = _playerSpawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize;
                //创建音效点
                sigleMusic(m_outPokeCurrentSize);
                //把打出的牌的父亲节点放到outPokeTwoSpawn中
                _playerSpawn.transform.GetChild(i).parent = _outSpawn.transform;   
                break;
            }
            else
            {
                isOut = false;
            }
        } 
        if(!isOut)
        {
            noWantMusic();
            noWantUITrue(m_outPokeSpawn);
        }
    }
    //打对
    private void pairOutPoke(GameObject _playerSpawn,GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerSpawn.transform.childCount - 1; i++)
        {
            //为了方便牌面识别需要用到容器
            List<GameObject> twoPoke = new List<GameObject>();
            twoPoke.Add(_playerSpawn.transform.GetChild(i).gameObject);
            twoPoke.Add(_playerSpawn.transform.GetChild(i + 1).gameObject);
            if (twoPoke[0].GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize && twoPoke[0].GetComponent<pokeObject>().m_pokeSize == twoPoke[1].GetComponent<pokeObject>().m_pokeSize)
            {
                noWantUIFalse(m_outPokeSpawn);
                isOut = true;
                for (int j = 0; j < twoPoke.Count; j++)
                {
                    //把打出来的牌放到outPokeTwoSpawn位置上
                    twoPoke[j].transform.position = new Vector3(_outSpawn.transform.position.x, _outSpawn.transform.position.y + VerticalDetla * j);
                    //打出来的牌翻开-由于已经放到容器里面所以就可以直接翻牌
                    twoPoke[j].GetComponent<pokeObject>().showPoke();
                    //把打出来的牌的大小赋予在公用的单牌上，方便后面玩家识别并打牌
                    m_outPokeCurrentSize = twoPoke[j].GetComponent<pokeObject>().m_pokeSize;
                    //创建音效点
                    pairMusic(m_outPokeCurrentSize);
                    //把打出的牌的父亲节点放到outPokeTwoSpawn中
                    twoPoke[j].transform.parent = _outSpawn.transform;
                }
                break;
            }
            else
            {
                isOut = false;
            }
        }
        if(!isOut)
        {
            noWantMusic();
            noWantUITrue(m_outPokeSpawn);
        }
    }
    //打三个
    private void threeOutPoke(GameObject _playerSpawn,GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerSpawn.transform.childCount - 2; i++)
        {
            List<GameObject> threePoke = new List<GameObject>();
            threePoke.Add(_playerSpawn.transform.GetChild(i).gameObject);
            threePoke.Add(_playerSpawn.transform.GetChild(i + 1).gameObject);
            threePoke.Add(_playerSpawn.transform.GetChild(i + 2).gameObject);
            if(threePoke[0].GetComponent<pokeObject>().m_pokeSize>m_outPokeCurrentSize
                &&threePoke[0].GetComponent<pokeObject>().m_pokeSize==threePoke[1].GetComponent<pokeObject>().m_pokeSize
                &&threePoke[1].GetComponent<pokeObject>().m_pokeSize==threePoke[2].GetComponent<pokeObject>().m_pokeSize)
            {
                noWantUIFalse(m_outPokeSpawn);
                isOut = true;
                for(int j=0;j<threePoke.Count;j++)
                {
                    threePoke[j].transform.position = new Vector3(_outSpawn.transform.position.x, _outSpawn.transform.position.y + VerticalDetla * j);
                    threePoke[j].GetComponent<pokeObject>().showPoke();
                    m_outPokeCurrentSize = threePoke[j].GetComponent<pokeObject>().m_pokeSize;
                    musicManager.instance.playEffectByName("three");
                    threePoke[j].transform.parent = _outSpawn.transform;
                }
                break;
            }
            else
            {
                isOut = false;
            }
        }
        if(!isOut)
        {
            noWantMusic();
            noWantUITrue(m_outPokeSpawn);
        }
    }
    //打四个
    private void fourOutPoke(GameObject _playerSpawn,GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerSpawn.transform.childCount - 3; i++)
        {
            List<GameObject> fourPoke = new List<GameObject>();
            fourPoke.Add(_playerSpawn.transform.GetChild(i).gameObject);
            fourPoke.Add(_playerSpawn.transform.GetChild(i + 1).gameObject);
            fourPoke.Add(_playerSpawn.transform.GetChild(i + 2).gameObject);
            fourPoke.Add(_playerSpawn.transform.GetChild(i + 3).gameObject);
            if(fourPoke[0].GetComponent<pokeObject>().m_pokeSize>m_outPokeCurrentSize&&fourPoke[0].GetComponent<pokeObject>().m_pokeSize==fourPoke[1].GetComponent<pokeObject>().m_pokeSize&&fourPoke[1].GetComponent<pokeObject>().m_pokeSize==fourPoke[2].GetComponent<pokeObject>().m_pokeSize&&fourPoke[2].GetComponent<pokeObject>().m_pokeSize==fourPoke[3].GetComponent<pokeObject>().m_pokeSize)
            {
                for(int j=0;j<fourPoke.Count;j++)
                {
                    fourPoke[j].transform.position = new Vector3(_outSpawn.transform.position.x, _outSpawn.transform.position.y + VerticalDetla * j);
                    fourPoke[j].GetComponent<pokeObject>().showPoke();
                    m_outPokeCurrentSize = fourPoke[j].GetComponent<pokeObject>().m_pokeSize;
                    musicManager.instance.playEffectByName("four");
                    fourPoke[j].transform.parent = _outSpawn.transform;
                }
                break;
            }
        }
    }
    //打3顺子
    private void threeOutPokeOneDargon(GameObject _playerSpawn,GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for(int i=0;i<_playerSpawn.transform.childCount-2;i++)
        {
            List<GameObject> outPoke = new List<GameObject>();
            outPoke.Add(_playerSpawn.transform.GetChild(i).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 1).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 2).gameObject);
            if(outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize>m_outPokeCurrentSize
                && outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[2].GetComponent<pokeObject>().m_pokeSize != 15)
            {
                noWantUIFalse(m_outPokeSpawn);
                isOut = true;
                for(int j=0;j<outPoke.Count;j++)
                {
                    outPoke[j].transform.position = new Vector3(_outSpawn.transform.position.x, _outSpawn.transform.position.y + VerticalDetla * j);
                    outPoke[j].GetComponent<pokeObject>().showPoke();
                    m_outPokeCurrentSize = outPoke[0].GetComponent<pokeObject>().m_pokeSize;
                    musicManager.instance.playEffectByName("shunzi");
                    outPoke[j].transform.parent = _outSpawn.transform; 
                }
                break;
            }
            else
            {
                isOut = false;
            }
        }
        if (!isOut)
        {
            noWantMusic();
            noWantUITrue(m_outPokeSpawn);
        }
    }
    //打4顺子
    private void fourOutPokeOneDargon(GameObject _playerSpawn, GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerSpawn.transform.childCount - 3; i++)
        {
            List<GameObject> outPoke = new List<GameObject>();
            outPoke.Add(_playerSpawn.transform.GetChild(i).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 1).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 2).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 3).gameObject);
            if (outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                && outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[3].GetComponent<pokeObject>().m_pokeSize != 15)
            {
                noWantUIFalse(m_outPokeSpawn);
                isOut = true;
                for (int j = 0; j < outPoke.Count; j++)
                {
                    outPoke[j].transform.position = new Vector3(_outSpawn.transform.position.x, _outSpawn.transform.position.y + VerticalDetla * j);
                    outPoke[j].GetComponent<pokeObject>().showPoke();
                    m_outPokeCurrentSize = outPoke[0].GetComponent<pokeObject>().m_pokeSize;
                    musicManager.instance.playEffectByName("shunzi");
                    outPoke[j].transform.parent = _outSpawn.transform;
                }
                break;
            }
            else
            {
                isOut = false;
            }
        }
        if (!isOut)
        {
            noWantMusic();
            noWantUITrue(m_outPokeSpawn);
        }
    }
    //打5顺子
    private void fiveOutPokeOneDargon(GameObject _playerSpawn, GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerSpawn.transform.childCount - 4; i++)
        {
            List<GameObject> outPoke = new List<GameObject>();
            outPoke.Add(_playerSpawn.transform.GetChild(i).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 1).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 2).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 3).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 4).gameObject);
            if (outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                && outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[4].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[4].GetComponent<pokeObject>().m_pokeSize != 15)
            {
                noWantUIFalse(m_outPokeSpawn);
                isOut = true;
                for (int j = 0; j < outPoke.Count; j++)
                {
                    outPoke[j].transform.position = new Vector3(_outSpawn.transform.position.x, _outSpawn.transform.position.y + VerticalDetla * j);
                    outPoke[j].GetComponent<pokeObject>().showPoke();
                    m_outPokeCurrentSize = outPoke[0].GetComponent<pokeObject>().m_pokeSize;
                    musicManager.instance.playEffectByName("shunzi");
                    outPoke[j].transform.parent = _outSpawn.transform;
                }
                break;
            }
            else
            {
                isOut = false;
            }
        }
        if (!isOut)
        {
            noWantMusic();
            noWantUITrue(m_outPokeSpawn);
        }
    }
    //打6顺子
    private void sixOutPokeOneDargon(GameObject _playerSpawn, GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerSpawn.transform.childCount - 5; i++)
        {
            List<GameObject> outPoke = new List<GameObject>();
            outPoke.Add(_playerSpawn.transform.GetChild(i).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 1).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 2).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 3).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 4).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 5).gameObject);
            if (outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                && outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[4].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[4].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[5].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[5].GetComponent<pokeObject>().m_pokeSize != 15)
            {
                for (int j = 0; j < outPoke.Count; j++)
                {
                    outPoke[j].transform.position = new Vector3(_outSpawn.transform.position.x, _outSpawn.transform.position.y + VerticalDetla * j);
                    outPoke[j].GetComponent<pokeObject>().showPoke();
                    m_outPokeCurrentSize = outPoke[0].GetComponent<pokeObject>().m_pokeSize;
                    musicManager.instance.playEffectByName("shunzi");
                    outPoke[j].transform.parent = _outSpawn.transform;
                }
                break;
            }
        }
    }
    //打7顺子
    private void sevenOutPokeOneDargon(GameObject _playerSpawn, GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerSpawn.transform.childCount - 6; i++)
        {
            List<GameObject> outPoke = new List<GameObject>();
            outPoke.Add(_playerSpawn.transform.GetChild(i).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 1).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 2).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 3).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 4).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 5).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 6).gameObject);
            if (outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                && outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[4].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[4].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[5].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[5].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[6].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[6].GetComponent<pokeObject>().m_pokeSize != 15)
            {
                for (int j = 0; j < outPoke.Count; j++)
                {
                    outPoke[j].transform.position = new Vector3(_outSpawn.transform.position.x, _outSpawn.transform.position.y + VerticalDetla * j);
                    outPoke[j].GetComponent<pokeObject>().showPoke();
                    m_outPokeCurrentSize = outPoke[0].GetComponent<pokeObject>().m_pokeSize;
                    musicManager.instance.playEffectByName("shunzi");
                    outPoke[j].transform.parent = _outSpawn.transform;
                }
                break;
            }
        }
    }
    //打8顺子
    private void eightOutPokeOneDargon(GameObject _playerSpawn, GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerSpawn.transform.childCount - 7; i++)
        {
            List<GameObject> outPoke = new List<GameObject>();
            outPoke.Add(_playerSpawn.transform.GetChild(i).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 1).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 2).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 3).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 4).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 5).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 6).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 7).gameObject);
            if (outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                && outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[4].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[4].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[5].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[5].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[6].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[6].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[7].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[7].GetComponent<pokeObject>().m_pokeSize != 15)
            {
                for (int j = 0; j < outPoke.Count; j++)
                {
                    outPoke[j].transform.position = new Vector3(_outSpawn.transform.position.x, _outSpawn.transform.position.y + VerticalDetla * j);
                    outPoke[j].GetComponent<pokeObject>().showPoke();
                    m_outPokeCurrentSize = outPoke[0].GetComponent<pokeObject>().m_pokeSize;
                    musicManager.instance.playEffectByName("shunzi");
                    outPoke[j].transform.parent = _outSpawn.transform;
                }
                break;
            }
        }
    }
    //打9顺子
    private void nineOutPokeOneDargon(GameObject _playerSpawn, GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerSpawn.transform.childCount - 8; i++)
        {
            List<GameObject> outPoke = new List<GameObject>();
            outPoke.Add(_playerSpawn.transform.GetChild(i).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 1).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 2).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 3).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 4).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 5).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 6).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 7).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 8).gameObject);
            if (outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                && outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[4].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[4].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[5].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[5].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[6].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[6].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[7].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[7].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[8].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[8].GetComponent<pokeObject>().m_pokeSize != 15)
            {
                for (int j = 0; j < outPoke.Count; j++)
                {
                    outPoke[j].transform.position = new Vector3(_outSpawn.transform.position.x, _outSpawn.transform.position.y + VerticalDetla * j);
                    outPoke[j].GetComponent<pokeObject>().showPoke();
                    m_outPokeCurrentSize = outPoke[0].GetComponent<pokeObject>().m_pokeSize;
                    musicManager.instance.playEffectByName("shunzi");
                    outPoke[j].transform.parent = _outSpawn.transform;
                }
                break;
            }
        }
    }
    //打10顺子
    private void tenOutPokeOneDargon(GameObject _playerSpawn, GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerSpawn.transform.childCount - 9; i++)
        {
            List<GameObject> outPoke = new List<GameObject>();
            outPoke.Add(_playerSpawn.transform.GetChild(i).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 1).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 2).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 3).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 4).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 5).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 6).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 7).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 8).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 9).gameObject);
            if (outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                && outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[4].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[4].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[5].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[5].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[6].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[6].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[7].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[7].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[8].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[8].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[9].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[9].GetComponent<pokeObject>().m_pokeSize != 15)
            {
                for (int j = 0; j < outPoke.Count; j++)
                {
                    outPoke[j].transform.position = new Vector3(_outSpawn.transform.position.x, _outSpawn.transform.position.y + VerticalDetla * j);
                    outPoke[j].GetComponent<pokeObject>().showPoke();
                    m_outPokeCurrentSize = outPoke[0].GetComponent<pokeObject>().m_pokeSize;
                    musicManager.instance.playEffectByName("shunzi");
                    outPoke[j].transform.parent = _outSpawn.transform;
                }
                break;
            }
        }
    }
    //打11顺子
    private void elevenOutPokeOneDargon(GameObject _playerSpawn, GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerSpawn.transform.childCount - 10; i++)
        {
            List<GameObject> outPoke = new List<GameObject>();
            outPoke.Add(_playerSpawn.transform.GetChild(i).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 1).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 2).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 3).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 4).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 5).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 6).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 7).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 8).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 9).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 10).gameObject);
            if (outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                && outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[4].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[4].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[5].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[5].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[6].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[6].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[7].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[7].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[8].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[8].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[9].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[9].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[10].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[10].GetComponent<pokeObject>().m_pokeSize != 15)
            {
                for (int j = 0; j < outPoke.Count; j++)
                {
                    outPoke[j].transform.position = new Vector3(_outSpawn.transform.position.x, _outSpawn.transform.position.y + VerticalDetla * j);
                    outPoke[j].GetComponent<pokeObject>().showPoke();
                    m_outPokeCurrentSize = outPoke[0].GetComponent<pokeObject>().m_pokeSize;
                    musicManager.instance.playEffectByName("shunzi");
                    outPoke[j].transform.parent = _outSpawn.transform;
                }
                break;
            }
        }
    }
    //打12顺子
    private void twelveOutPokeOneDargon(GameObject _playerSpawn, GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerSpawn.transform.childCount - 11; i++)
        {
            List<GameObject> outPoke = new List<GameObject>();
            outPoke.Add(_playerSpawn.transform.GetChild(i).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 1).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 2).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 3).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 4).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 5).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 6).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 7).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 8).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 9).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 10).gameObject);
            outPoke.Add(_playerSpawn.transform.GetChild(i + 11).gameObject);
            if (outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                && outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[4].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[4].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[5].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[5].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[6].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[6].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[7].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[7].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[8].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[8].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[9].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[9].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[10].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[10].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[11].transform.GetComponent<pokeObject>().m_pokeSize
                && outPoke[11].GetComponent<pokeObject>().m_pokeSize != 15)
            {
                for (int j = 0; j < outPoke.Count; j++)
                {
                    outPoke[j].transform.position = new Vector3(_outSpawn.transform.position.x, _outSpawn.transform.position.y + VerticalDetla * j);
                    outPoke[j].GetComponent<pokeObject>().showPoke();
                    m_outPokeCurrentSize = outPoke[0].GetComponent<pokeObject>().m_pokeSize;
                    musicManager.instance.playEffectByName("shunzi");
                    outPoke[j].transform.parent = _outSpawn.transform;
                }
                break;
            }
        }
    }
    //打3连对
    private void threePairDargonOutPoke(GameObject _playerSpawn,GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerSpawn.transform.childCount - 5; i++)
        {
            List<GameObject> threePairDargon = new List<GameObject>();
            threePairDargon.Add(_playerSpawn.transform.GetChild(i).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 1).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 2).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 3).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 4).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 5).gameObject);
            //假如相邻的两个牌相等且两个为一体的之后的牌的数逐层+1
            if (threePairDargon[5].GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize 
                && threePairDargon[0].GetComponent<pokeObject>().m_pokeSize == threePairDargon[1].GetComponent<pokeObject>().m_pokeSize 
                && threePairDargon[0].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[2].GetComponent<pokeObject>().m_pokeSize 
                && threePairDargon[2].GetComponent<pokeObject>().m_pokeSize == threePairDargon[3].GetComponent<pokeObject>().m_pokeSize 
                && threePairDargon[2].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[4].GetComponent<pokeObject>().m_pokeSize 
                && threePairDargon[4].GetComponent<pokeObject>().m_pokeSize == threePairDargon[5].GetComponent<pokeObject>().m_pokeSize 
                && threePairDargon[5].GetComponent<pokeObject>().m_pokeSize != 15)
            {
                for (int j = 0; j < threePairDargon.Count; j++)
                {
                    threePairDargon[j].transform.position = new Vector3(_outSpawn.transform.position.x, _outSpawn.transform.position.y + VerticalDetla * j);
                    threePairDargon[j].GetComponent<pokeObject>().showPoke();
                    m_outPokeCurrentSize = threePairDargon[5].GetComponent<pokeObject>().m_pokeSize;
                    musicManager.instance.playEffectByName("pairLine");
                    threePairDargon[j].transform.parent = _outSpawn.transform; 
                }
                break;
            }
        }
    }
    //打4连对
    private void fourPairDargonOutPoke(GameObject _playerSpawn, GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerSpawn.transform.childCount - 7; i++)
        {
            List<GameObject> threePairDargon = new List<GameObject>();
            threePairDargon.Add(_playerSpawn.transform.GetChild(i).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 1).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 2).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 3).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 4).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 5).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 6).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 7).gameObject);
            //假如相邻的两个牌相等且两个为一体的之后的牌的数逐层+1
            if (threePairDargon[0].GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                && threePairDargon[0].GetComponent<pokeObject>().m_pokeSize == threePairDargon[1].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[0].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[2].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[2].GetComponent<pokeObject>().m_pokeSize == threePairDargon[3].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[2].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[4].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[4].GetComponent<pokeObject>().m_pokeSize == threePairDargon[5].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[4].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[6].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[6].GetComponent<pokeObject>().m_pokeSize == threePairDargon[7].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[7].GetComponent<pokeObject>().m_pokeSize != 15)
            {
                for (int j = 0; j < threePairDargon.Count; j++)
                {
                    threePairDargon[j].transform.position = new Vector3(_outSpawn.transform.position.x, _outSpawn.transform.position.y + VerticalDetla * j);
                    threePairDargon[j].GetComponent<pokeObject>().showPoke();
                    m_outPokeCurrentSize = threePairDargon[0].GetComponent<pokeObject>().m_pokeSize;
                    musicManager.instance.playEffectByName("pairLine");
                    threePairDargon[j].transform.parent = _outSpawn.transform;
                }
                break;
            }
        }
    }
    //打5连对
    private void fivePairDargonOutPoke(GameObject _playerSpawn, GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerSpawn.transform.childCount - 9; i++)
        {
            List<GameObject> threePairDargon = new List<GameObject>();
            threePairDargon.Add(_playerSpawn.transform.GetChild(i).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 1).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 2).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 3).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 4).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 5).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 6).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 7).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 8).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 9).gameObject);
            //假如相邻的两个牌相等且两个为一体的之后的牌的数逐层+1
            if (threePairDargon[0].GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                && threePairDargon[0].GetComponent<pokeObject>().m_pokeSize == threePairDargon[1].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[0].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[2].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[2].GetComponent<pokeObject>().m_pokeSize == threePairDargon[3].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[2].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[4].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[4].GetComponent<pokeObject>().m_pokeSize == threePairDargon[5].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[4].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[6].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[6].GetComponent<pokeObject>().m_pokeSize == threePairDargon[7].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[6].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[8].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[8].GetComponent<pokeObject>().m_pokeSize == threePairDargon[9].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[9].GetComponent<pokeObject>().m_pokeSize != 15)
            {
                for (int j = 0; j < threePairDargon.Count; j++)
                {
                    threePairDargon[j].transform.position = new Vector3(_outSpawn.transform.position.x, _outSpawn.transform.position.y + VerticalDetla * j);
                    threePairDargon[j].GetComponent<pokeObject>().showPoke();
                    m_outPokeCurrentSize = threePairDargon[0].GetComponent<pokeObject>().m_pokeSize;
                    musicManager.instance.playEffectByName("pairLine");
                    threePairDargon[j].transform.parent = _outSpawn.transform;
                }
                break;
            }
        }
    }
    //打6连对
    private void sixPairDargonOutPoke(GameObject _playerSpawn, GameObject _outSpawn)
    {
        for (int i = 0; i < _outSpawn.transform.childCount; i++)
        {
            Destroy(_outSpawn.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _playerSpawn.transform.childCount - 11; i++)
        {
            List<GameObject> threePairDargon = new List<GameObject>();
            threePairDargon.Add(_playerSpawn.transform.GetChild(i).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 1).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 2).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 3).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 4).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 5).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 6).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 7).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 8).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 9).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 10).gameObject);
            threePairDargon.Add(_playerSpawn.transform.GetChild(i + 11).gameObject);
            //假如相邻的两个牌相等且两个为一体的之后的牌的数逐层+1
            if (threePairDargon[0].GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                && threePairDargon[0].GetComponent<pokeObject>().m_pokeSize == threePairDargon[1].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[0].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[2].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[2].GetComponent<pokeObject>().m_pokeSize == threePairDargon[3].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[2].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[4].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[4].GetComponent<pokeObject>().m_pokeSize == threePairDargon[5].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[4].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[6].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[6].GetComponent<pokeObject>().m_pokeSize == threePairDargon[7].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[6].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[8].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[8].GetComponent<pokeObject>().m_pokeSize == threePairDargon[9].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[8].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[10].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[10].GetComponent<pokeObject>().m_pokeSize == threePairDargon[11].GetComponent<pokeObject>().m_pokeSize
                && threePairDargon[11].GetComponent<pokeObject>().m_pokeSize != 15)
            {
                for (int j = 0; j < threePairDargon.Count; j++)
                {
                    threePairDargon[j].transform.position = new Vector3(_outSpawn.transform.position.x, _outSpawn.transform.position.y + VerticalDetla * j);
                    threePairDargon[j].GetComponent<pokeObject>().showPoke();
                    m_outPokeCurrentSize = threePairDargon[0].GetComponent<pokeObject>().m_pokeSize;
                    musicManager.instance.playEffectByName("pairLine");
                    threePairDargon[j].transform.parent = _outSpawn.transform;
                }
                break;
            }
        }
    }

    //提示该出的牌
    public void oneSpawnIdiot()
    {
        switch (m_outPokeRule)
        {
            case OUT_POKE_RULE.SINGLE:
                {
                    for (int i = 0; i < oneSpawn.transform.childCount; i++)
                    {
                        if (oneSpawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize)
                        {
                            oneSpawn.transform.GetChild(i).transform.Translate(0, m_selectedDetla, 0);
                            break;
                        }
                    }
                }
                break;
            case OUT_POKE_RULE.PAIR:
                {
                    for (int i = 0; i < oneSpawn.transform.childCount; i++)
                    {
                        //为了方便牌面识别需要用到容器
                        List<GameObject> twoPoke = new List<GameObject>();
                        twoPoke.Add(oneSpawn.transform.GetChild(i).gameObject);
                        twoPoke.Add(oneSpawn.transform.GetChild(i + 1).gameObject);
                        if (twoPoke[0].GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize && twoPoke[0].GetComponent<pokeObject>().m_pokeSize == twoPoke[1].GetComponent<pokeObject>().m_pokeSize)
                        {
                            for (int j = 0; j < twoPoke.Count; j++)
                            {
                                twoPoke[j].transform.Translate(0, m_selectedDetla, 0);
                            }
                            break;
                        }
                    }
                }
                break;
            case OUT_POKE_RULE.THREE:
                for (int i = 0; i < oneSpawn.transform.childCount - 2; i++)
                {
                    List<GameObject> threePoke = new List<GameObject>();
                    threePoke.Add(oneSpawn.transform.GetChild(i).gameObject);
                    threePoke.Add(oneSpawn.transform.GetChild(i + 1).gameObject);
                    threePoke.Add(oneSpawn.transform.GetChild(i + 2).gameObject);
                    if (threePoke[0].GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                        && threePoke[0].GetComponent<pokeObject>().m_pokeSize == threePoke[1].GetComponent<pokeObject>().m_pokeSize
                        && threePoke[1].GetComponent<pokeObject>().m_pokeSize == threePoke[2].GetComponent<pokeObject>().m_pokeSize)
                    {
                        for (int j = 0; j < threePoke.Count; j++)
                        {
                            threePoke[j].transform.Translate(0, m_selectedDetla, 0);
                        }
                        break;
                    }
                }
                break;
            case OUT_POKE_RULE.FOUR:
                for (int i = 0; i < oneSpawn.transform.childCount - 2; i++)
                {
                    List<GameObject> fourPoke = new List<GameObject>();
                    fourPoke.Add(oneSpawn.transform.GetChild(i).gameObject);
                    fourPoke.Add(oneSpawn.transform.GetChild(i + 1).gameObject);
                    fourPoke.Add(oneSpawn.transform.GetChild(i + 2).gameObject);
                    fourPoke.Add(oneSpawn.transform.GetChild(i + 3).gameObject);
                    if (fourPoke[0].GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize 
                        && fourPoke[0].GetComponent<pokeObject>().m_pokeSize == fourPoke[1].GetComponent<pokeObject>().m_pokeSize 
                        && fourPoke[1].GetComponent<pokeObject>().m_pokeSize == fourPoke[2].GetComponent<pokeObject>().m_pokeSize 
                        && fourPoke[2].GetComponent<pokeObject>().m_pokeSize == fourPoke[3].GetComponent<pokeObject>().m_pokeSize)
                    {
                        for (int j = 0; j < fourPoke.Count; j++)
                        {
                            fourPoke[i].transform.Translate(0, m_selectedDetla, 0);
                        }
                        break;
                    }
                }
                break;
            case OUT_POKE_RULE.ONE_DRAGON_three:
                for (int i = 0; i < oneSpawn.transform.childCount - 2; i++)
                {
                    List<GameObject> outPoke = new List<GameObject>();
                    outPoke.Add(oneSpawn.transform.GetChild(i).gameObject);
                    outPoke.Add(oneSpawn.transform.GetChild(i + 1).gameObject);
                    outPoke.Add(oneSpawn.transform.GetChild(i + 2).gameObject);
                    if (outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                        && outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize
                        && outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize
                        && outPoke[2].GetComponent<pokeObject>().m_pokeSize != 15)
                    {
                        noWantUIFalse(m_outPokeSpawn);
                        isOut = true;
                        for (int j = 0; j < outPoke.Count; j++)
                        {
                            outPoke[i].transform.Translate(0, m_selectedDetla, 0);
                        }
                        break;
                    }
                }
                break;
            case OUT_POKE_RULE.ONE_DRAGON_four:
                for (int i = 0; i < oneSpawn.transform.childCount - 3; i++)
                {
                    List<GameObject> outPoke = new List<GameObject>();
                    outPoke.Add(oneSpawn.transform.GetChild(i).gameObject);
                    outPoke.Add(oneSpawn.transform.GetChild(i + 1).gameObject);
                    outPoke.Add(oneSpawn.transform.GetChild(i + 2).gameObject);
                    outPoke.Add(oneSpawn.transform.GetChild(i + 3).gameObject);
                    if (outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                        && outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize
                        && outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize
                        && outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize
                        && outPoke[3].GetComponent<pokeObject>().m_pokeSize != 15)
                    {
                        for (int j = 0; j < outPoke.Count; j++)
                        {
                            outPoke[i].transform.Translate(0, m_selectedDetla, 0);
                        }
                        break;
                    }
                }
                break;
            case OUT_POKE_RULE.ONE_DRAGON_five:
                for (int i = 0; i < oneSpawn.transform.childCount - 4; i++)
                {
                    List<GameObject> outPoke = new List<GameObject>();
                    outPoke.Add(oneSpawn.transform.GetChild(i).gameObject);
                    outPoke.Add(oneSpawn.transform.GetChild(i + 1).gameObject);
                    outPoke.Add(oneSpawn.transform.GetChild(i + 2).gameObject);
                    outPoke.Add(oneSpawn.transform.GetChild(i + 3).gameObject);
                    outPoke.Add(oneSpawn.transform.GetChild(i + 4).gameObject);
                    if (outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                        && outPoke[0].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize
                        && outPoke[1].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize
                        && outPoke[2].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize
                        && outPoke[3].transform.GetComponent<pokeObject>().m_pokeSize + 1 == outPoke[4].transform.GetComponent<pokeObject>().m_pokeSize
                        && outPoke[4].GetComponent<pokeObject>().m_pokeSize != 15)
                    {
                        noWantUIFalse(m_outPokeSpawn);
                        isOut = true;
                        for (int j = 0; j < outPoke.Count; j++)
                        {
                            outPoke[i].transform.Translate(0, m_selectedDetla, 0);
                        }
                        break;
                    }
                }
                break;
            case OUT_POKE_RULE.PAIR_DRAGON_three:
                for (int i = 0; i < oneSpawn.transform.childCount - 5; i++)
                {
                    List<GameObject> threePairDargon = new List<GameObject>();
                    threePairDargon.Add(oneSpawn.transform.GetChild(i).gameObject);
                    threePairDargon.Add(oneSpawn.transform.GetChild(i + 1).gameObject);
                    threePairDargon.Add(oneSpawn.transform.GetChild(i + 2).gameObject);
                    threePairDargon.Add(oneSpawn.transform.GetChild(i + 3).gameObject);
                    threePairDargon.Add(oneSpawn.transform.GetChild(i + 4).gameObject);
                    threePairDargon.Add(oneSpawn.transform.GetChild(i + 5).gameObject);
                    //假如相邻的两个牌相等且两个为一体的之后的牌的数逐层+1
                    if (threePairDargon[5].GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                        && threePairDargon[0].GetComponent<pokeObject>().m_pokeSize == threePairDargon[1].GetComponent<pokeObject>().m_pokeSize
                        && threePairDargon[0].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[2].GetComponent<pokeObject>().m_pokeSize
                        && threePairDargon[2].GetComponent<pokeObject>().m_pokeSize == threePairDargon[3].GetComponent<pokeObject>().m_pokeSize
                        && threePairDargon[2].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[4].GetComponent<pokeObject>().m_pokeSize
                        && threePairDargon[4].GetComponent<pokeObject>().m_pokeSize == threePairDargon[5].GetComponent<pokeObject>().m_pokeSize
                        && threePairDargon[5].GetComponent<pokeObject>().m_pokeSize != 15)
                    {
                        for (int j = 0; j < threePairDargon.Count; j++)
                        {
                            threePairDargon[i].transform.Translate(0, m_selectedDetla, 0); 
                        }
                        break;
                    }
                }
                break;
            case OUT_POKE_RULE.PAIR_DRAGON_four:
                for (int i = 0; i < oneSpawn.transform.childCount - 7; i++)
                {
                    List<GameObject> threePairDargon = new List<GameObject>();
                    threePairDargon.Add(oneSpawn.transform.GetChild(i).gameObject);
                    threePairDargon.Add(oneSpawn.transform.GetChild(i + 1).gameObject);
                    threePairDargon.Add(oneSpawn.transform.GetChild(i + 2).gameObject);
                    threePairDargon.Add(oneSpawn.transform.GetChild(i + 3).gameObject);
                    threePairDargon.Add(oneSpawn.transform.GetChild(i + 4).gameObject);
                    threePairDargon.Add(oneSpawn.transform.GetChild(i + 5).gameObject);
                    threePairDargon.Add(oneSpawn.transform.GetChild(i + 6).gameObject);
                    threePairDargon.Add(oneSpawn.transform.GetChild(i + 7).gameObject);
                    //假如相邻的两个牌相等且两个为一体的之后的牌的数逐层+1
                    if (threePairDargon[0].GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize
                        && threePairDargon[0].GetComponent<pokeObject>().m_pokeSize == threePairDargon[1].GetComponent<pokeObject>().m_pokeSize
                        && threePairDargon[0].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[2].GetComponent<pokeObject>().m_pokeSize
                        && threePairDargon[2].GetComponent<pokeObject>().m_pokeSize == threePairDargon[3].GetComponent<pokeObject>().m_pokeSize
                        && threePairDargon[2].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[4].GetComponent<pokeObject>().m_pokeSize
                        && threePairDargon[4].GetComponent<pokeObject>().m_pokeSize == threePairDargon[5].GetComponent<pokeObject>().m_pokeSize
                        && threePairDargon[4].GetComponent<pokeObject>().m_pokeSize + 1 == threePairDargon[6].GetComponent<pokeObject>().m_pokeSize
                        && threePairDargon[6].GetComponent<pokeObject>().m_pokeSize == threePairDargon[7].GetComponent<pokeObject>().m_pokeSize
                        && threePairDargon[7].GetComponent<pokeObject>().m_pokeSize != 15)
                    {
                        for (int j = 0; j < threePairDargon.Count; j++)
                        {
                            threePairDargon[j].transform.Translate(0, m_selectedDetla, 0); 
                        }
                        break;
                    }
                }
                break;
        }
    }

    //主玩家不出牌
    public void oneSpawnNoCudeThree(GameObject _playerSpawn,GameObject _outSpawn)
    {
        //玩家手中没有方块3-出牌顺序由代码搜寻方块3
        for (int i = 0; i < _playerSpawn.transform.childCount; i++)
        {
            if (_playerSpawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize > m_outPokeCurrentSize)
            {
                //把打出来的牌放到outPokeTwoSpawn位置上
                _playerSpawn.transform.GetChild(i).position = _outSpawn.transform.position;
                //打出来的牌翻开
                _playerSpawn.transform.GetChild(i).GetComponent<pokeObject>().showPoke();
                //把打出来的牌的大小赋予在公用的单牌上，方便后面玩家识别并打牌
                m_outPokeCurrentSize = _playerSpawn.transform.GetChild(i).GetComponent<pokeObject>().m_pokeSize;
                //创建音效点
                sigleMusic(m_outPokeCurrentSize);
                //把打出的牌的父亲节点放到outPokeTwoSpawn中
                _playerSpawn.transform.GetChild(i).parent = _outSpawn.transform;
                m_outPokeRule = OUT_POKE_RULE.SINGLE;
                break;
            }
            
        }
    }

    public void noWantOutPoke()
    {
        int randNum = Random.Range(0, 3);
        if(randNum==0)
        {
            musicManager.instance.playEffectByName("NoWant1");
        }
        if(randNum==1)
        {
            musicManager.instance.playEffectByName("NoWant2");
        }
        if(randNum==2)
        {
            musicManager.instance.playEffectByName("NoWant3");
        }
        for (int i = 0; i < outPokeSpawn.transform.childCount;i++ )
        {
            Destroy(outPokeSpawn.transform.GetChild(i).gameObject);
        }
        m_noWant1.SetActive(true);
        StartCoroutine("twoSpawnOutPoke");
        m_outPokePlane.SetActive(false);
    }

    //单个牌的音效
    public void sigleMusic(int _insteadCurrentSize)
    {
        switch (_insteadCurrentSize)
        {
            case 3:
                musicManager.instance.playEffectByName("single3");
                break;
            case 4:
                musicManager.instance.playEffectByName("single4");
                break;
            case 5:
                musicManager.instance.playEffectByName("single5");
                break;
            case 6:
                musicManager.instance.playEffectByName("single6");
                break;
            case 7:
                musicManager.instance.playEffectByName("single7");
                break;
            case 8:
                musicManager.instance.playEffectByName("single8");
                break;
            case 9:
                musicManager.instance.playEffectByName("single9");
                break;
            case 10:
                musicManager.instance.playEffectByName("single10");
                break;
            case 11:
                musicManager.instance.playEffectByName("single11");
                break;
            case 12:
                musicManager.instance.playEffectByName("single12");
                break;
            case 13:
                musicManager.instance.playEffectByName("single13");
                break;
            case 14:
                musicManager.instance.playEffectByName("single1");
                break;
            case 15:
                musicManager.instance.playEffectByName("single2");
                break;
            case 16:
                musicManager.instance.playEffectByName("singleSmallKing");
                break;
            case 17:
                musicManager.instance.playEffectByName("singleBigKing");
                break;
        }
    }
    //对牌的音效
    public void pairMusic(int _insteadCurrentSize)
    {
        switch (_insteadCurrentSize)
        {
            case 3:
                musicManager.instance.playEffectByName("pair3");
                break;
            case 4:
                musicManager.instance.playEffectByName("pair4");
                break;
            case 5:
                musicManager.instance.playEffectByName("pair5");
                break;
            case 6:
                musicManager.instance.playEffectByName("pair6");
                break;
            case 7:
                musicManager.instance.playEffectByName("pair7");
                break;
            case 8:
                musicManager.instance.playEffectByName("pair8");
                break;
            case 9:
                musicManager.instance.playEffectByName("pair9");
                break;
            case 10:
                musicManager.instance.playEffectByName("pair10");
                break;
            case 11:
                musicManager.instance.playEffectByName("pair11");
                break;
            case 12:
                musicManager.instance.playEffectByName("pair12");
                break;
            case 13:
                musicManager.instance.playEffectByName("pair13");
                break;
            case 14:
                musicManager.instance.playEffectByName("pair1");
                break;
            case 15:
                musicManager.instance.playEffectByName("pair2");
                break;
        }
    }
    //不要的音效函数
    public void noWantMusic()
    {
        int randNum = Random.Range(0, 3);
        if (randNum == 0)
        {
            musicManager.instance.playEffectByName("NoWant1");
        }
        if (randNum == 1)
        {
            musicManager.instance.playEffectByName("NoWant2");
        }
        if (randNum == 2)
        {
            musicManager.instance.playEffectByName("NoWant3");
        }
    }

    //不要牌的UI
    public void noWantUITrue(OUT_POKE_SORT _outPokeSpawn)
    {

        switch (_outPokeSpawn)
        {
            case OUT_POKE_SORT.TWO_SPAWN:
                m_noWant2.SetActive(true);
                break;
            case OUT_POKE_SORT.THREE_SPAWN:
                m_noWant3.SetActive(true);
                break;
            case OUT_POKE_SORT.FOUR_SPAWN:
                m_noWant4.SetActive(true);
                break;
        }
    }
    public void noWantUIFalse(OUT_POKE_SORT _outPokeSpawn)
    {
        switch (_outPokeSpawn)
        {
            case OUT_POKE_SORT.TWO_SPAWN:
                m_noWant2.SetActive(false);
                break;
            case OUT_POKE_SORT.THREE_SPAWN:
                m_noWant3.SetActive(false);
                break;
            case OUT_POKE_SORT.FOUR_SPAWN:
                m_noWant4.SetActive(false);
                break;
        }
    }

    //public void aginOutPoke()
    //{
    //    if(m_outPokeCurrentSize == 17)
    //    {

    //    }
    //}

}
