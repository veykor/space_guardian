using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class startmanager : MonoBehaviour
{
	
	[SerializeField] public UIDocument uiDoc;
	[SerializeField] public VisualElement blackScreen;
	[SerializeField] public List<string> introText;
	[SerializeField] public float timeBetweenText, timeBetweenLetters, TimeBeforeChangeScene;
	[SerializeField] public bool writing=false;
	[SerializeField] public AudioSource[] myAudios;
	[SerializeField] public float timeAudioFade, timeAudioStep;
	
	
	
	
	
	public class uiDataClass {
		public string introText;
		
		
		
		
	}
	
	
	uiDataClass uiData;
	
	
	
	void start_game() {
		myAudios[0].Play();
		StartCoroutine(intro_scene());
		StartCoroutine(fade_music(false));
	}
	
	void exit_game() {
		myAudios[0].Play();
		Application.Quit();
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
			uiData.introText=s.Substring(0,i);
			myAudios[0].pitch=4f;
			myAudios[0].PlayOneShot(myAudios[0].clip, 0.6f);
			yield return new WaitForSeconds(timeBetweenLetters);
		}
		yield return new WaitForSeconds(timeBetweenText);
		
		writing=false;
	}
	
	IEnumerator intro_scene() {
		blackScreen.style.display=DisplayStyle.Flex;
		blackScreen.EnableInClassList("backblack",true);
		yield return new WaitForSeconds(3f);
		foreach(string s in introText) {
			writing=true;
			StartCoroutine(write_text(s));
			yield return new WaitUntil(()=>!writing);
			
			//yield return new WaitForSeconds(timeBetweenText);
		}
		uiData.introText="";
		yield return new WaitForSeconds(TimeBeforeChangeScene);
		
		SceneManager.LoadScene("game");
		
		
		
	}
	
	IEnumerator intro_start() {
		
		
		yield return new WaitForSeconds(1f);
		
		blackScreen.EnableInClassList("transition2s",true);		
		blackScreen.EnableInClassList("backblack",false);
		myAudios[1].Play();
		StartCoroutine(fade_music());
		//if(SceneManager.loadedSceneCount>1) SceneManager.UnloadSceneAsync("game");
		yield return new WaitForSeconds(2f);
		
		
		blackScreen.style.display=DisplayStyle.None;
	}
	
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		
		myAudios = GetComponents<AudioSource>();
		
		uiData=new uiDataClass();
		uiDoc=GetComponent<UIDocument>();
		
		var aux = uiDoc.rootVisualElement.Q<Button>("start");
		aux.clicked += start_game;
		aux = uiDoc.rootVisualElement.Q<Button>("exit");
		aux.clicked += exit_game;
		
		uiDoc.rootVisualElement.dataSource = uiData;
		blackScreen = uiDoc.rootVisualElement.Q<VisualElement>("blackscreen");
		
		
		StartCoroutine(intro_start());
		
		
		
        //StartCoroutine(test());
    }
	

    // Update is called once per frame
    void Update()
    {
        
    }
}
