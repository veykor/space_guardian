using UnityEngine;
using System.Collections;

public class powerup : MonoBehaviour
{
	
	[SerializeField] public float speed, timeLife;
	[SerializeField] public Rigidbody2D myRb;
	
	[SerializeField] public bonusclass bonus;
	
	void pack_bonus() {
		
	}
	
	IEnumerator destroy_me() {
		yield return new WaitForSeconds(timeLife);
		Destroy(this.gameObject);
	}
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		myRb=GetComponent<Rigidbody2D>();
        myRb.linearVelocityX=-speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void OnCollisionEnter2D(Collision2D col) {
		if(!col.gameObject.CompareTag("player")) return;
		
		col.gameObject.SendMessage("receive_bonus", bonus);
		Destroy(gameObject);
	}
	
}
