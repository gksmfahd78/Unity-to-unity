using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour 
{
	void Start () 
	{
		SceneManager.LoadScene(1, LoadSceneMode.Additive);
		SceneManager.LoadScene(2, LoadSceneMode.Additive);
	}
	
	void Update () 
	{
		
	}
}
