using System.Collections;
using UnityEngine;

public class fire : MonoBehaviour
{
	
	//[SerializeField] public float rangeTrigger;
	[SerializeField] public bool inverSpeed;
	[SerializeField] public float speed;
	[SerializeField] public float slowUpdatePeriod=0.01f;
	[SerializeField] public Animator myAnim;
	
    
	
	void set_speed(float speed){
		this.speed=speed;
	}
	
	IEnumerator slow_update() {
		while(true) {
			myAnim.SetFloat("speed",inverSpeed?-speed:speed);
			yield return new WaitForSeconds(slowUpdatePeriod);
		}		
	}
	
	
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        myAnim=GetComponent<Animator>();
		StartCoroutine(slow_update());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
