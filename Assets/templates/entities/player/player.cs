using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class player : ship
{
	
	[SerializeField] public float acceleration = 10f;
	[SerializeField] public bool locked=false;
	[SerializeField] public float brakeFactor = 0.1f, recoilFactor;
	[SerializeField] Vector2 moveInput, currentVelocity=Vector2.zero;
	[SerializeField] bool shootInput,enableMove=true, respawning=false;
	[SerializeField] public float timeReenableShoot, respawnDelay=5f, respawnPeriod=0.2f;
	[SerializeField] public Vector3 initRespawnPoint, endRespawnPoint;
	
		
	[SerializeField] InputActionReference moveAction;
	[SerializeField] InputActionReference shootAction;
	
	public BoxCollider2D myCol;

	
	private void OnEnable() {
		moveAction.action.Enable();
		shootAction.action.Enable();
		
		moveAction.action.started+=OnMove;
		moveAction.action.performed+=OnMove;
		moveAction.action.canceled+=OnMove;
		
		shootAction.action.started+=OnShoot;
		shootAction.action.performed+=OnShoot;
		shootAction.action.canceled+=OnShoot;
		
		
	}
	
	private void OnDisable() {
		moveAction.action.Disable();
		shootAction.action.Disable();
		
		moveAction.action.started-=OnMove;
		moveAction.action.performed-=OnMove;
		moveAction.action.canceled-=OnMove;
		
		shootAction.action.started-=OnShoot;
		shootAction.action.performed-=OnShoot;
		shootAction.action.canceled-=OnShoot;
		
	}
	
	[ContextMenu("launch")]
	
	public void launch() {
		StartCoroutine(launch_coroutine());
	}
	
	public IEnumerator launch_coroutine() {
		respawning=true;
		int steps = (int)(respawnDelay/respawnPeriod);
		enableShoot=enableMove=myCol.enabled=false;
		transform.position = initRespawnPoint;
		Vector3 deltaMove=(endRespawnPoint-initRespawnPoint)/steps;
		
		float deltaLife=(float)lifeMax/(float)steps;
		for(int i=0; i<steps;i++) {			
			transform.position=initRespawnPoint + deltaMove*i;
			life=deltaLife*i;
			yield return new WaitForSeconds(respawnPeriod);
		}
		enableShoot=enableMove=myCol.enabled=true;
		respawning=false;
	}
	
	
	
	
	
	
	
	
	
	public void set_aside() {
		StartCoroutine(set_aside_coroutine());		
	}
	
	IEnumerator set_aside_coroutine() {
		myRb.linearVelocity=new Vector2(2f,-2f);
		yield return new WaitUntil(()=>transform.position.y<-2f);
		myRb.linearVelocity=Vector2.zero;
	}
	
	void die() {
		GameObject res=Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		
		launch();		
		life=lifeMax;
		
		
	}
	
	new public void hurt(float damage) {
		life-=(damage*(1-(shield/100f)));
		if(life<=0) {die(); life=0f; }
	}
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new void Start()
    {
		base.Start();
		
		myCol=GetComponent<BoxCollider2D>();
		launch();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if(locked) return;
		
        currentVelocity += moveInput * acceleration * Time.deltaTime;
		currentVelocity = currentVelocity.normalized * Mathf.Clamp(currentVelocity.magnitude, -speed, speed);
	
		myRb.linearVelocity = currentVelocity;
		myAnim.SetFloat("hspeed", moveInput.x!=0?moveInput.x:0);

		currentVelocity *= (1f-(brakeFactor));
		
		
		update_fire_anim(moveInput.x);
		
    }
	
	private void OnMove(InputAction.CallbackContext obj) {
		moveInput=Vector2.zero;
		if(!enableMove) return;
		moveInput=obj.ReadValue<Vector2>();
		
	}
	
	private IEnumerator reenable_shoot() {
		yield return new WaitForSeconds(timeReenableShoot*(1f-bonus.shootFireSpeedUp));
		enableShoot=!respawning;
	}
	
	private void OnShoot(InputAction.CallbackContext obj) { //pasar el contenido y usar el shoot
		shootInput=obj.ReadValueAsButton();
		if(!shootInput || !enableShoot) return;
		shoot();
		//Instantiate(bulletPrefab, transform.position+bulletOffset, Quaternion.identity);
		StartCoroutine(reenable_shoot());
		currentVelocity.x=currentVelocity.x-recoilFactor;
		enableShoot=false;
		//myAudio.clip=shootSound;
		//myAudio.Play();
		
	}
	
}
