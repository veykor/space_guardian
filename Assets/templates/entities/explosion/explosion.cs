using System.Collections;
using UnityEngine;

public class explosion : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
	public float timeToDie;
	
	IEnumerator die() {
		yield return new WaitForSeconds(timeToDie);
		Destroy(this.gameObject);
		
	}
	
	void Start()
    {
        StartCoroutine(die());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
