using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
	
	[SerializeField] public float speed, timeLife;
	[SerializeField] public Vector2 dir;
	[SerializeField] public Color myColor;
	[SerializeField] public List<string> targetTags;
	[SerializeField] public float damage;
	[SerializeField] public float impulseForce;
	public GameObject explosionPrefab;
	
	
	[SerializeField] IEnumerator killMe;
	
	SpriteRenderer mySprite;
	
	
	IEnumerator destroy_me() {
		yield return new WaitForSeconds(timeLife);
		Destroy(this.gameObject);
		
	}
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		dir=dir.normalized;
		transform.rotation=Quaternion.Euler(0f,0f,Mathf.Rad2Deg*Mathf.Atan2(dir.y, dir.x));
        mySprite=GetComponent<SpriteRenderer>();
		mySprite.color=myColor;
		killMe=destroy_me();
		StartCoroutine(killMe);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
	
	
	void OnTriggerEnter2D(Collider2D col) {
		if(!statictools.has_tag(col.gameObject.tag, targetTags) ) return;
		Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		
		ship aux = col.gameObject.GetComponent<ship>();
		
		if(aux == null || aux.notDamagedByBullet>0) return;
		
		//aux.myRb.AddForce(impulseForce * dir);
		
		aux.impulse(impulseForce * dir);
		col.gameObject.SendMessage("hurt", damage);
		StopCoroutine(killMe);
		
		Destroy(this.gameObject);
		
	}
	
}
