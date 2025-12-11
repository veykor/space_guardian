using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


public class gamemanager : MonoBehaviour
{
	
	[Header("UI config")]
	[SerializeField] UIDocument uiCtl;
	[SerializeField] VisualElement blackScreen, gameOverDiag, tutorial;
	[SerializeField] float periodUpdate=0.05f;
	[SerializeField] bool writing=false;
	[SerializeField] public List<string> outroText;
	[SerializeField] public float timeBetweenText, timeBetweenLetters, TimeBeforeChangeScene;
	[SerializeField] public float timeAudioFade, timeAudioStep;
	
	[Header("Levels config")]
	[SerializeField] public bool nextLevel=false, boolTest=false;
	[SerializeField] public float startLevelTimer;
	[SerializeField] public int level;
	[SerializeField] levelmanager currentLevel;
	[SerializeField] List<GameObject> levels;
	
	
	
	[Header("Ships config")]
	
	[SerializeField] public ship navis;
	[SerializeField] public player yat;	
	[SerializeField] public float timeToRespawnYat;
	[SerializeField] public GameObject limitsCollider;
	
	[Header("Camera config")]
	[SerializeField] public Camera myCam;
	[SerializeField] public Vector2 camZoom;
	[SerializeField] public float timeCameraTransition, timeCameraStep;
	[SerializeField] public bool cameraZooming=false;
	
	[Header("Game config")]
	[SerializeField] public int totalPoints=0, levelPoints=0;
	[SerializeField] public AudioSource[] myAudios;
	[SerializeField] public bool gameOver;
	[SerializeField] public GameObject planetPrefab;
	[SerializeField] public Vector2 planetTimeSpawnRange;
	[SerializeField] public Vector2 planetPositionRange;
	[SerializeField] public float planetHorizontalPosition;
	
	[SerializeField] public class uiDataClass {
		public string level;
		public Length levelProgress;
		public string totalPoints;
		public Length navisLife;
		public Length yatLife;
		public Translate lvlChartPosition;
		public string outroText;
	}
	
	public uiDataClass uiData;
	
	void Awake() {
		uiData=new uiDataClass();
		uiCtl=GetComponent<UIDocument>();
		uiCtl.rootVisualElement.dataSource = uiData;
		gameOverDiag=uiCtl.rootVisualElement.Q<VisualElement>("gameoverframe");
		tutorial=uiCtl.rootVisualElement.Q<VisualElement>("tutorial");
		blackScreen=uiCtl.rootVisualElement.Q<VisualElement>("blackscreen");
		blackScreen.EnableInClassList("transition2s", false);
		blackScreen.EnableInClassList("backblack", true);
		
	}

	IEnumerator planet_spawner() {
		for(int i=0; i<10000000;i++) {
			yield return new WaitForSeconds(Random.Range(planetTimeSpawnRange.x, planetTimeSpawnRange.y));
			Instantiate(planetPrefab, new Vector3(planetHorizontalPosition, Random.Range(planetPositionRange.x, planetPositionRange.y), 0.5f), Quaternion.identity);
		}
		
	}

	
	void add_points(int points) {
		levelPoints+=points;
	}
	
	Length calc_healthlife_bar(float life, float lifeMax) {
		return Length.Percent((1f-life/lifeMax)*97.1f);
	}
	
	Length calc_progress_bar(float totalTime) {
		return Length.Percent(Mathf.Min(88f,(Time.time-startLevelTimer)*(77f/totalTime)+11f));
	}

	IEnumerator uiUpdate() {
		for(int i = 0; i<1000000000; i++) {
			uiData.navisLife=calc_healthlife_bar(navis.life, navis.lifeMax);
			uiData.yatLife=calc_healthlife_bar(yat.life, yat.lifeMax);
			uiData.totalPoints=(totalPoints+levelPoints).ToString("D8");
			if(currentLevel!=null) uiData.levelProgress=calc_progress_bar(currentLevel.duration+currentLevel.delayToStart);
			uiData.level=(level+1).ToString();
			
			
			//me queda por hacer el cartelito de level que aparece al comienzo del nivel, y asignarlo
			
			yield return new WaitForSeconds(periodUpdate);
		}
	}
	
	IEnumerator show_level() {
				
		uiData.lvlChartPosition=new Translate(Length.Percent(0f), Length.Percent(0));
		yield return new WaitForSeconds(4f);
		uiData.lvlChartPosition=new Translate(Length.Percent(500f), Length.Percent(0));
		yield return new WaitForSeconds(2f);
		uiData.lvlChartPosition=new Translate(Length.Percent(0f), Length.Percent(1000f));
		yield return new WaitForSeconds(2f);
		uiData.lvlChartPosition=new Translate(Length.Percent(-500f), Length.Percent(0));
	}
	
	IEnumerator fade_music(bool inOut=true) {//true in, false out
		myAudios[1].volume=inOut?0f:1f;
		int steps=(int)(timeAudioFade/timeAudioStep);
		
		float volumeStep=1f/(float)steps;
		
		for(int i=0; i<steps; i++) {
			
			myAudios[1].volume+=(volumeStep*(inOut?1f:-1f));
			yield return new WaitForSeconds(timeAudioStep);
		}
		
		myAudios[1].volume=inOut?1f:0f;
	}
	
	IEnumerator write_text(string s) {
		
		for(int i=1; i<=s.Length; i++) {
			uiData.outroText=s.Substring(0,i);
			myAudios[0].pitch=4f;
			myAudios[0].PlayOneShot(myAudios[0].clip, 0.6f);
			yield return new WaitForSeconds(timeBetweenLetters);
		}
		yield return new WaitForSeconds(timeBetweenText);
		
		writing=false;
	}
	
	IEnumerator intro_game() {
		yield return new WaitForSeconds(1f);
		blackScreen.EnableInClassList("transition2s",true);
		blackScreen.EnableInClassList("backblack",false);
		blackScreen.style.display=DisplayStyle.None;
		StartCoroutine(fade_music());
		myAudios[1].Play();
	}
	
	IEnumerator outro_game() {
		//Se pone musica de victoria y minicreditos con la ultratecnica aprendida de startmanager
		
		cameraZooming=true;
		StartCoroutine(toggle_camera_zoom(false));
		limitsCollider.SetActive(false);
		yat.locked=true;
		yat.set_aside();	
		navis.myRb.linearVelocityX=2f;
		yield return new WaitUntil(()=>navis.transform.position.x>10f);
		blackScreen.style.display=DisplayStyle.Flex;
		blackScreen.EnableInClassList("backblack",true);
		
		yield return new WaitForSeconds(2f);
		
		outroText.Add($"Take your payment, {totalPoints}$");
		
		foreach(string s in outroText) {
			writing=true;
			StartCoroutine(write_text(s));
			yield return new WaitUntil(()=>!writing);
		}
		StartCoroutine(fade_music(false));
		yield return new WaitForSeconds(TimeBeforeChangeScene);
		
		SceneManager.LoadScene("start");
		
	}
	
	IEnumerator proc() {
		for(this.level=0;this.level<levels.Count;this.level++) {
			nextLevel=false;
			totalPoints+=levelPoints;
			levelPoints=0;
			startLevelTimer=Time.time;
			
			navis.life=navis.lifeMax;
			yat.life=yat.lifeMax;
			
			StartCoroutine(show_level());
			
			GameObject currentLevelGO=Instantiate(levels[level], transform);
			currentLevel= currentLevelGO.GetComponent<levelmanager>();
			yield return new WaitUntil(()=>nextLevel&&!gameOver);
			currentLevel=null;
			
			tutorial.style.display=DisplayStyle.None;
			
			//añadir animacion de que se mueve la nave navis
			
		}
		StartCoroutine(outro_game());
	}

	IEnumerator check_game_over() {
		yield return new WaitUntil(()=>navis.life<1);
		gameOver=true;
		yield return new WaitForSeconds(2f);
		gameOverDiag.style.display=DisplayStyle.Flex;
		yat.locked=true;
		
	}
	
	void pause_game() {
		myAudios[0].Play();
		Debug.Log("pausar");
	}
	
	void exit_game() {
		myAudios[0].Play();
		Application.Quit();
	}
	
	void retry_game() {
		
		SceneManager.LoadScene("game");
		return;
		/*myAudios[0].Play();
		limitsCollider.SetActive(true);
		
		yat.launch();
		myCam.orthographicSize=camZoom.x;
		yat.locked=false;
		Debug.Log("reintentar");*/
	}
	
		
	
	
	
	void Start()
    {
		
		var aux = uiCtl.rootVisualElement.Q<Button>("pause");
		aux.clicked += pause_game;
		aux = uiCtl.rootVisualElement.Q<Button>("quit");
		aux.clicked += exit_game;
		aux = uiCtl.rootVisualElement.Q<Button>("quit2");
		aux.clicked += exit_game;
		aux = uiCtl.rootVisualElement.Q<Button>("retry");
		aux.clicked += retry_game;
		
		
		myAudios = GetComponents<AudioSource>();
		
		StartCoroutine(intro_game());
        StartCoroutine(uiUpdate());
		StartCoroutine(proc());
		StartCoroutine(check_game_over());
		StartCoroutine(planet_spawner());
    }

    // Update is called once per frame
    void Update()
    {
		
    }
	
	void closing_level() {
		Debug.Log("se acabo el nivel weon");
		nextLevel=true;
	}
	
	IEnumerator toggle_camera_zoom(bool inOut=false) {//false -> near 1.5, true -> near 6
		float dir=inOut?-1f:1f;
		float nearStep=dir*(camZoom.y-camZoom.x)/(timeCameraTransition/timeCameraStep);
		int step=(int)(timeCameraTransition/timeCameraStep);
		for( int i=0; i<step; i++) {
			myCam.orthographicSize+=nearStep;
			yield return new WaitForSeconds(timeCameraStep);
		}
		cameraZooming=false;
	}
	
	
}
