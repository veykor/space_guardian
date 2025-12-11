using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ship : MonoBehaviour
{
	
	public enum types {
		yat,
		nives,
		littleboy,
		bigboy,
		fastboy,
	};
	
	[Header("ship config")]
	[SerializeField] public float life;
	[SerializeField] public float lifeMax;
	[SerializeField] public float shield;
	[SerializeField] public float impulseShield;
	[SerializeField] public float speed;
	[SerializeField] public int type; //0 player - 1 navis - 2 little boy - 3 big boy - 4 fast boy
	[SerializeField] public bool  enableShoot;
	[SerializeField] public int notDamagedByBullet;
	[SerializeField] public float collisionDamage;
	[SerializeField] public float periodFadeLife;
	[SerializeField] public float invulnerableTime;
	[SerializeField] public GameObject shieldGO;
	
	[Header("bullet config")]
	[SerializeField] public List<string> targetTags;
	[SerializeField] public float bulletDamage;
	[SerializeField] public float impulseBullet;
	[SerializeField] public Color bulletColor;
	[SerializeField] public Vector3 bulletOffset;
	[SerializeField] public List<Vector2> bulletsDir;
	[SerializeField] public AudioClip shootSound;
	public GameObject bulletPrefab;
	public float dieDelay;
	public GameObject explosionPrefab;
	
	[Header("bonus")]
	[SerializeField] public bonusclass bonus;
	
	public levelmanager myLevel;
	
	
	
	public Rigidbody2D myRb;
	public Animator myAnim;
	public AudioSource myAudio;
	
	
	public void update_fire_anim(float speed) {
		for(int i=0; i<transform.childCount; i++) {
			if(!transform.GetChild(i).gameObject.CompareTag("fire")) continue;
			transform.GetChild(i).SendMessage("set_speed", speed);
		}
	}
	
	public void shoot() {
		//configurar la bala antes de mandarla
		if(!enableShoot) return;
		foreach(Vector2 bulletDir in bulletsDir) {
			GameObject res = Instantiate(bulletPrefab, transform.position+bulletOffset, Quaternion.identity);
			bullet bulletCtl=res.GetComponent<bullet>();
			bulletCtl.dir=bulletDir;
			bulletCtl.myColor=bulletColor;	
			bulletCtl.targetTags=this.targetTags;
			bulletCtl.impulseForce=impulseBullet;
			bulletCtl.damage=bulletDamage+bonus.damageUp;
		}
		myAudio.clip=shootSound;
		myAudio.Play();
		
	}
	
	IEnumerator die() {
		GameObject res=Instantiate(explosionPrefab, transform.position, Quaternion.identity);	
		yield return new WaitForSeconds(dieDelay);
		Destroy(this.gameObject);
	}
	
	

	
	public void impulse(Vector3 impulse) {
		if(impulseShield >99.9f) return;
		myRb.AddForce(impulse*( 1f - impulseShield/100f ));
		
	}
	
	/*
	public IEnumerator fade_life(float deltaLife) {
		
		float newLife=Mathf.Max(life+deltaLife, 0f);
		float aux=newLife-life;
		int range=(int)Mathf.Abs(aux);
		float lifeStep=aux/range;
		int step=(int)(aux/Mathf.Abs(aux));
			
		for(int i = 0; i<range; i++)
		{
			life+=lifeStep;
			yield return new WaitForSeconds(periodFadeLife);
		}	
	}*/
	public IEnumerator invulnerable() {
		
		shieldGO.SetActive(++notDamagedByBullet>0);
		yield return new WaitForSeconds(invulnerableTime);
		shieldGO.SetActive(--notDamagedByBullet>0);
		
	}
	
	public IEnumerator three_shoot() {
		
		yield break;
	}
	
	public void receive_bonus(bonusclass bonus) {
		if(bonus.invulnerable) {StartCoroutine(invulnerable());}
		if(bonus.threeShoots) {StartCoroutine(invulnerable());}
		
		this.shield+=bonus.shield;
		life=Mathf.Min(life+bonus.healthUp, lifeMax);
		this.bonus.shootFireSpeedUp+=bonus.shootFireSpeedUp;
		this.bonus.damageUp+=bonus.damageUp;
	}
	
	public void hurt(float damage) {
		
		//life-=(damage*(1-(shield/100f)));
		//if(gameObject.CompareTag("enemy")) {
		life-=(damage*(1-(shield/100f)));
		//} else fade_life(-(damage*(1-(shield/100f))));
		
		if(life<=0) {StartCoroutine(die()); life=0f; }
		//Debug.Log($"La life de {gameObject.name}:{life}");
	}
	
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        bonus=ScriptableObject.CreateInstance<bonusclass>();
		myAnim=GetComponent<Animator>();
		TryGetComponent<Rigidbody2D>(out myRb);
		myAudio=GetComponent<AudioSource>();
		life=lifeMax;
		
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void OnCollisionEnter2D(Collision2D col) {
		if(!statictools.has_tag(col.gameObject.tag, targetTags) ) return;
		col.gameObject.SendMessage("hurt", collisionDamage);
		//Debug.Log(col.gameObject.tag);
	}
	
}
