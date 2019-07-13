using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class winOrLost : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void addPoke()
    {
        blackjeckGame.instance.sendPokeByPlayer();
    }
    public void winOrLostOfGameOver1()
    {
        blackjeckGame.instance.complatePlayerAndComputer();
    }

    public void winOrLostOfGameOver2()
    {
        comparitionOfSize.instance.complateOneAndThree();
    }
}
