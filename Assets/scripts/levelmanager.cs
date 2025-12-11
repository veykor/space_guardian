using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class levelmanager : MonoBehaviour
{
	
	public enum states {
		onFailing,
		onProcess,
		onWaiting,
	};
		
	
	
	//time is distributed delayToStart + duration + delayToEnd
	//example
	//delayToStart 5s
	//duration 60s
	//delayToEnd 5s
	//total time 70s
	[Header("status")]
	
	public int state;
	
	
	[Header("general config")]
	[SerializeField] public int id;
	[SerializeField] public float duration;
	[SerializeField] public float delayToStart, delayToEnd;
	[SerializeField] public GameObject gm;

	[Header("enemies config")]
	[SerializeField] public Vector3 enemyPlace;
	[SerializeField] public GameObject enemyPool;
	[SerializeField] public int enemiesPlaced=0;
	[SerializeField] public List<GameObject> enemies;
	[SerializeField] public int enemiesDestroyed=0;
	
	Vector3 enemyPosition;
	
	void place_enemy(GameObject enemyGO) {
		if(enemyGO.tag == "player") return;
		switch(enemyGO.GetComponent<ship>().type) {
			case (int)ship.types.bigboy:
			case (int)ship.types.littleboy: //y=(-0.8,0.7)
				enemyPosition=new Vector3(0f, Random.Range(-0.8f, 0.7f), 0f) + enemyPlace;
				
				break;
			case (int)ship.types.fastboy:
				enemyPosition=new Vector3(0f, Random.Range(-0.4f, 0.3f), 0f) + enemyPlace;
				break;
		}
		
		GameObject aux=Instantiate(enemyGO, enemyPosition, Quaternion.identity, enemyPool.transform);
		aux.GetComponent<ship>().myLevel=this;
		
	}
	
	
	IEnumerator proc() {
		state=(int)states.onWaiting;
		yield return new WaitForSeconds(delayToStart);
		state=(int)states.onProcess;
		float timeBetweenEnemies=duration/(enemies.Count-1);
		
		foreach(GameObject enemyGO in enemies) {
			place_enemy(enemyGO);
			yield return new WaitForSeconds(timeBetweenEnemies);
		}
		
		yield return new WaitUntil(()=> enemiesDestroyed>=enemies.Count);
		state=(int)states.onWaiting;
		yield return new WaitForSeconds(delayToEnd);		
		gm.SendMessage("closing_level");
		Destroy(this.gameObject);
	}
	
	
	
	
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		gm=GameObject.Find("GameManager");
		enemyPool=GameObject.Find("enemy_pool");
        StartCoroutine(proc());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	
	public void enemy_destroyed() {
		enemiesDestroyed++;
	}
	
	
	
}
