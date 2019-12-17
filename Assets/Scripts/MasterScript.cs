using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MasterScript : MonoBehaviour
{

	public int startingDovePops = 0;
	public int startingHawkPops = 0;
	public int startingGuardianPops = 0;
	public int foodQuantity = 100;
	public GameObject popPrefab;
	public GameObject foodPrefab;
	public GameObject heartPrefab;
	public List<GameObject> Pops;
	public Transform[] Meals;
	public float radius;
	public List<GameObject> deads;

	public int timeSpeed = 1;

	private void Awake()
	{
		radius = (transform.localScale.x - 4f) / 2f;
	}

	void Start()
	{
		Pops = new List<GameObject>();
		Meals = new Transform[foodQuantity];
		deads = new List<GameObject>();
		
		for (int i = 0; i < startingDovePops; i++)
		{
			GameObject newPop = Instantiate(popPrefab);
			newPop.GetComponent<MeshRenderer>().material.color = Color.cyan;
			newPop.AddComponent<Dove>();
			PlacePops();
		}
		for (int i = 0; i < startingHawkPops; i++)
		{
			GameObject newPop = Instantiate(popPrefab);
			newPop.GetComponent<MeshRenderer>().material.color = Color.red;
			newPop.AddComponent<Hawk>();
			PlacePops();
		}
		for (int i = 0; i < startingGuardianPops; i++)
		{
			GameObject newPop = Instantiate(popPrefab);
			newPop.GetComponent<MeshRenderer>().material.color = Color.yellow;
			newPop.AddComponent<Guardian>();
			PlacePops();
		}
		SpawnFood();
	}

	private void Update()
	{
		Time.timeScale = timeSpeed;

	}

	void SpawnFood ()
	{
		for (int i = 0; i < foodQuantity; i++)
		{
			float randomAngle = Random.Range(0f,Mathf.PI*2);
			float maxPosX = Mathf.Cos(randomAngle) * radius;
			float maxPosZ = Mathf.Sin(randomAngle) * radius;
			float randomX = Random.Range(0f,maxPosX);
			float randomZ = Random.Range(0f,maxPosZ);
			GameObject newFood = (GameObject)Instantiate(foodPrefab, new Vector3(randomX, 0, randomZ), Quaternion.identity);
			Meals[i] = newFood.transform;
		}	
	}

	void PlacePops ()
	{
		for (int i = 0; i < Pops.Count; i++)
		{
			float randomAngle = Random.Range(0f, Mathf.PI * 2);
			float maxPosX = Mathf.Cos(randomAngle) * radius;
			float maxPosZ = Mathf.Sin(randomAngle) * radius;
			float randomX = Random.Range(0f, maxPosX);
			float randomZ = Random.Range(0f, maxPosZ);
			Vector3 pos = new Vector3(randomX, 0.575f, randomZ);
			Pops[i].GetComponent<NavMeshAgent>().Warp(pos);
		}
	}

}