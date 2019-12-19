using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MasterScript : Singleton<MasterScript>
{
	public int FoodEnergy;
	public int startingDovePops = 0;
	public int startingHawkPops = 0;
	public int startingGuardianPops = 0;
	public int foodQuantity = 100;
	public GameObject popPrefab;
	public GameObject foodPrefab;
	public GameObject heartPrefab;
	public List<GameObject> Pops;
	public Transform[] Meals;
	public float squareSize;
	public List<GameObject> deads;

	public int timeSpeed = 1;

	public override void Awake()
	{
		squareSize = transform.localScale.x - 1;

		GameObject.Find("NavMesh").GetComponent<NavMeshSurface>().BuildNavMesh();
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
			PlacePop(newPop);
		}
		for (int i = 0; i < startingHawkPops; i++)
		{
			GameObject newPop = Instantiate(popPrefab);
			newPop.GetComponent<MeshRenderer>().material.color = Color.red;
			newPop.AddComponent<Hawk>();
			PlacePop(newPop);
		}
		for (int i = 0; i < startingGuardianPops; i++)
		{
			GameObject newPop = Instantiate(popPrefab);
			newPop.GetComponent<MeshRenderer>().material.color = Color.yellow;
			newPop.AddComponent<Guardian>();
			PlacePop(newPop);
		}
		SpawnFood();
	}

	void Update()
	{
		Time.timeScale = timeSpeed;

	}

	void SpawnFood ()
	{
		for (int i = 0; i < foodQuantity; i++)
		{
			GameObject newFood = Instantiate(foodPrefab);
			newFood.transform.position = new Vector3(Random.Range(1, squareSize), 0, Random.Range(1, squareSize));
			Meals[i] = newFood.transform;
		}	
	}

	void PlacePop (GameObject pop)
	{
		pop.transform.position = new Vector3(Random.Range(1, squareSize), 0, Random.Range(1, squareSize));
		pop.GetComponent<NavMeshAgent>().enabled = true;
	}

}