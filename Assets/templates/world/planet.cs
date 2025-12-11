using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planet : MonoBehaviour
{
	
	[SerializeField] float speed, scaleSetted, horizontalLimit;
	[SerializeField] Vector2 scaleRange;
	[SerializeField] SpriteRenderer mySprite;
	[SerializeField] List<Sprite> sprites;
	
	
	IEnumerator destroy_me() {
		yield return new WaitUntil(()=>transform.position.x<horizontalLimit);
		Destroy(this.gameObject);
	}
	
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		mySprite=GetComponent<SpriteRenderer>();
		mySprite.color=new Color(Random.Range(0f, 1f),Random.Range(0f, 1f),Random.Range(0f, 1f),1f);
		
		mySprite.sprite=sprites[(int)Random.Range(0, sprites.Count)];
		
        scaleSetted=Random.Range(scaleRange.x, scaleRange.y);
		transform.localScale=new Vector3(scaleSetted,scaleSetted,scaleSetted);
		transform.rotation=Quaternion.Euler(0f,0f,Random.Range(0f,360f));
		
		
		StartCoroutine(destroy_me());
    }

    // Update is called once per frame
    
	void FixedUpdate() {
		transform.Translate(new Vector2(-speed*(scaleSetted/scaleRange.y)*Time.fixedDeltaTime, 0f),  Space.World);
	}
	
	void Update()
    {
        
    }
}
