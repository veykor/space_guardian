using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class enemy : ship
{
	
	[SerializeField] public int points;
	[SerializeField] public GameObject gm;
	
	[Header("shoot config")]
	
	[SerializeField] public Vector2 rangeTimeBetweenShoot;
	
	[Header("enemy config")]
	
	[SerializeField] public bool waveMovement;
	[SerializeField] public float freqMovement, amplitudeMovement;
	
	[Header("bonus config")]
	[SerializeField] public float powerUpRate;
	[SerializeField] public List<GameObject> powerUpPrefab;
	
	
	
	[Header("auxiliar vars")]
	[SerializeField] public float lastSineMove;


	
	
	IEnumerator shooting_update() {
		for(int i=0; i<10000; i++) {
			float aux=Random.Range(rangeTimeBetweenShoot.x, rangeTimeBetweenShoot.y);
			yield return new WaitForSeconds(aux);
			shoot();			
		}
	}
	
	
	void move() {
		
	}
	
	
	
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new void Start()
    {
		base.Start();
		
		gm=GameObject.Find("GameManager");
		//myRb.linearVelocity=new Vector2(-speed, myRb.linearVelocity.y);
		
		StartCoroutine(shooting_update());
		
		if(!waveMovement) amplitudeMovement=0;
		
    }
	
	void FixedUpdate() {
		
		if(life<1) return;
		
		float sineMove=amplitudeMovement*Mathf.Cos(freqMovement*Time.time);
		myRb.linearVelocity=new Vector2(-speed, sineMove);
		update_fire_anim(10*(sineMove-lastSineMove));
		lastSineMove=sineMove;
	}
	
    // Update is called once per frame
    void Update()
    {
        
    }
	
	void OnTriggerEnter2D(Collider2D col) {
		switch(col.gameObject.tag) {
			case "bullet":
				break;
			case "player":
				Debug.Log("soy jugadorsito");
				break;
			
		}
		//Debug.Log($"pues es tigreton {col.gameObject.tag}");
	}
	
	void OnDestroy() {
		if(myLevel == null || life>1) return;
		myLevel.SendMessage("enemy_destroyed");
		gm.SendMessage("add_points", points);
		if(Random.Range(0f,1f)>powerUpRate) return;
		int selPowerUp=Random.Range(0, powerUpPrefab.Count);
		Instantiate(powerUpPrefab[selPowerUp], transform.position, Quaternion.identity);
		
		
	}
	
}
