﻿///<summary>
///		Script Manager:	Denver
///		Description:	Handles the functionality of the LevelSection.
///						Stores all enemies of its section and loops player.
///		Date Modified:	04/10/2018
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCompletionState
{
	NOT_CLEARED,
	CLEARED,
	FAILED
}

public class LevelSection : MonoBehaviour {

	[Tooltip("Number of attempts the player is given to complete a section before the section is skipped.")]
	[SerializeField] private int m_nNumberOfAttempts;

	[Tooltip("List of all the enemies in this section.")]
	public List<EnemyActor> m_enemiesList;

	private eCompletionState m_completionState;

	private int m_nCurrentSectionAttempts;

	void Start() {
		
		foreach(EnemyActor enemy in m_enemiesList) {
			enemy.m_section = this;
		}

		m_completionState = eCompletionState.NOT_CLEARED;

		m_nCurrentSectionAttempts = 0;

	}

	void FixedUpdate() {
		
		int enemyKillCount = 0;

		//check if player has defeated all enemies in the section
		foreach(EnemyActor enemy in m_enemiesList) {
			if(enemy.m_bIsAlive) {
				continue;
			}
			else {
				++enemyKillCount;
			}
		}

		if (enemyKillCount == m_enemiesList.Count) {
			m_completionState = eCompletionState.CLEARED;
		}

		// check if player has failed the section
		if (m_nCurrentSectionAttempts > m_nNumberOfAttempts) {
			m_completionState = eCompletionState.FAILED;
			foreach(EnemyActor enemy in m_enemiesList) {
				enemy.gameObject.SetActive(false);
			}
		}
	}

	void OnTriggerEnter(Collider other) {

		// check that other is a player
		if (other.tag == "Player") {
			// set player's CurrentSection to this
			other.gameObject.GetComponent<PlayerActor>().m_currentSection = this;

			// activate all enemies of the section and set their target
			foreach(EnemyActor enemy in m_enemiesList) {
				enemy.Activate(other.gameObject, GameObject.FindGameObjectWithTag("Playfield"));
			}
		}

	}

	void OnTriggerExit(Collider other) {

		// check that other is a playerMovementArea
		if (other.tag == "Playfield") {
			// section is not cleared
			if (m_completionState == eCompletionState.NOT_CLEARED) {
				// get the PlayerActor
				GameObject Playfield = other.gameObject;

				// move player to the beginning of the section
				Playfield.transform.position = new Vector3(Playfield.transform.position.x, Playfield.transform.position.y, transform.position.z - (transform.localScale.z / 2f + Playfield.transform.localScale.z / 2f));

				// increment attempts
				++m_nCurrentSectionAttempts;
			}

			// section is failed
			if (m_completionState == eCompletionState.FAILED) {
				// do the failed stuff
			}
		}

	}
}