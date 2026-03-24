using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject foodGO;
    [SerializeField] int numberOfFoodObjects;

    void Start()
    {
        for (int i = 0; i < numberOfFoodObjects; i++)
        {
            SpawnFood();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnFood()
    {
        Vector3 randomPos = new Vector3(Random.Range(-20f, 20f), 0.5f, Random.Range(-20f, 20f));
        Instantiate(foodGO, randomPos, Quaternion.identity);
    }
}
