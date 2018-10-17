﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///Script Manager: Drake
///Description:
///This script manages each enemy's patterns, and controls when the enemy should shoot. 
///</summary>
public class EnemyBulletPatternManager : MonoBehaviour
{
	[Tooltip("What this pattern fires from")]
	[SerializeField] private GameObject m_parent;

	[Tooltip("Whether or not this pattern moves with the playfield or not")]
	[SerializeField] private bool m_bEnvironment;

	[Tooltip("How long between pattern firing in seconds. \nThis should always be higher or equal to the longest child timer, or else bad stuff can happen")]
	[SerializeField] private float m_fTimer;

	//The current time in seconds
	private float m_fCurrentTimer;

	[Tooltip("All spawn locations of bullets in this pattern")]
	[SerializeField] private GameObject[] m_children;

	//Debug code to check if timer is longer than all child timers
	void Start()
	{
		foreach (GameObject child in m_children)
		{
			EnemyBulletPatternChild patternChild = child.GetComponent<EnemyBulletPatternChild>();

			patternChild.m_bEnvironment = m_bEnvironment;

			if (m_fTimer < patternChild.GetTimer())
				Debug.LogError("Timer on " + gameObject.name + " is shorter than " + child.name + ", you should fix this immediately!");
		}
	}

	// Use this for initialization
	void Awake()
	{
		m_fCurrentTimer = 0.0f;
	}
	
	// Update is called once per frame
	void Update()
	{
		if(m_parent.GetComponent<EnemyActor>() != null)
		{
			if(m_parent.GetComponent<EnemyActor>().m_bIsActive)
			{
				m_fCurrentTimer += Time.deltaTime;

				if (m_fCurrentTimer >= m_fTimer)
				{
					m_fCurrentTimer = 0.0f;

					foreach(GameObject child in m_children)
					{
						EnemyBulletPatternChild patternChild = child.GetComponent<EnemyBulletPatternChild>();

						patternChild.StartPattern();
					}
				}
			}
		}
		else
		{
			m_fCurrentTimer += Time.deltaTime;

			if (m_fCurrentTimer >= m_fTimer)
			{
				m_fCurrentTimer = 0.0f;

				foreach (GameObject child in m_children)
				{
					EnemyBulletPatternChild patternChild = child.GetComponent<EnemyBulletPatternChild>();

					patternChild.StartPattern();
				}
			}
		}
	}
}
