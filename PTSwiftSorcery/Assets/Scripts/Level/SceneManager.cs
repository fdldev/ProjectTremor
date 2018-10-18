﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

///<summary>
///		Script Manager: Denver
///		Description:	Singleton class that handles the state of the scene and switching
///						between scenes
///		Date Modified:	18/10/2018
///</summary>

public enum eSceneState {
	RUNNING,
	BOSS_FIGHT,
	PAUSED,
	COMPLETE,
	FAILED
}

[System.Serializable]
public struct sGameScene {
	public string name;
	public int index;
	public Scene scene;
	public CursorLockMode lockMode;
}

public class SceneManager : MonoBehaviour {

	#region Make a Singleton Class
	private static SceneManager instance = null;
	public static SceneManager Instance {
		get {
			// if SceneManager doesn't already exist
			if (instance == null) {
				// check that a GameObject doesn't already have a SceneManager component
				instance = GameObject.FindObjectOfType<SceneManager>();

				// if there isn't such a GameObject
				if (instance == null) {
					// create a GameObject for itself that won't be able to be replaced
					GameObject go = new GameObject();
					go.name = "SceneManager";
					instance = go.AddComponent<SceneManager>();
				}
			}
			return instance;
		}
	}

	void Awake() {
		if (instance == null) {
			// set instance
			instance = this;

			// set current scene
			m_currentScene.scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			m_currentScene.name = m_currentScene.scene.name;
			m_currentScene.index = 0;

			// initialise game scene data
			for(int i = 0; i < m_gameScenes.Length; ++i) {
			m_gameScenes[i].scene = UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(m_gameScenes[i].index); 
			}

			// let SceneManager trascend scenes
			DontDestroyOnLoad(instance);
		}
		else {
			Destroy(gameObject);
		}
	}

	private SceneManager() {}
	#endregion

	private eSceneState m_sceneState = eSceneState.RUNNING;
	public eSceneState SceneState {
		get {
			return m_sceneState;
		}

		set {
			// if value is a scene state
			if (value.GetType() == typeof(eSceneState)) {
			// change value
			m_sceneState = value;

			// run respective code
			Invoke("SceneStateChangedTo" + value.ToString(), 0);
			}
		}
	}

	private sGameScene m_currentScene;
	public sGameScene CurrentScene {
		get {
			return m_currentScene;
		}
	}

	private sGameScene m_previousScene;

	[Tooltip("List of all scenes in the game")]
	[SerializeField] private sGameScene[] m_gameScenes;
	public sGameScene[] GameScenes {
		get {
			return m_gameScenes;
		}
	}

	private GameObject m_pauseMenu;
	public GameObject PauseMenu {
		get {
			return m_pauseMenu;
		}

		set {
			if (value.tag == "PauseMenu") {
				m_pauseMenu = value;
			}
		}
	}

	void SceneStateChangedToRUNNING() {
		
		// change time scale and fixed delta time
		Time.timeScale = 1f;
		Time.fixedDeltaTime = 0.02f;

		// lock cursor
		Cursor.lockState = m_currentScene.lockMode;

		// stop displaying pause menu
		m_pauseMenu.SetActive(false);

		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerActor>().m_bCanMove = true;
	}

	void SceneStateChangedToPAUSED() {
	
		// change time scale
		Time.timeScale = 0f;

		// display pause menu
		m_pauseMenu.SetActive(true);

		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerActor>().m_bCanMove = false;

		// unlock cursor
		Cursor.lockState = CursorLockMode.None;
	}

	public void LoadScene(sGameScene newScene, LoadSceneMode loadSceneMode) {
		
		// change previous scene to what used to be the current scene
		m_previousScene = m_currentScene;

		// change current scene to desired scene
		m_currentScene = newScene;

		// load new scene
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(m_currentScene.index, loadSceneMode);

		SceneStateChangedToRUNNING();
	}

	public void UnloadCurrentScene() {

		// make sure there is a previous scene to return to
		if (m_previousScene.scene.isLoaded) {
			// save m_currentScene in an intermediate variable
			sGameScene intermediate = m_currentScene;

			// set currentScene to previousScene
			m_currentScene = m_previousScene;

			// set previousScene to intermediate
			m_previousScene = intermediate;

			// unload previous scene
			UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(m_previousScene.index);
		}
	}

	public sGameScene GetGameSceneWithName(string name) {

		foreach(sGameScene scene in m_gameScenes) {
			if (scene.name == name) {
				return scene;
			}
		}

		return new sGameScene();
	}
}