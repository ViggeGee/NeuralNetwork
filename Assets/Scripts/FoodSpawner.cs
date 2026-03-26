using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] GameObject foodGO;
    [SerializeField] int numberOfFoodObjects;

    void Start()
    {
        for (int i = 0; i < numberOfFoodObjects; i++)
        {
            SpawnFood();
        }
    }
    public void SpawnFood()
    {
        Vector3 randomPos = new Vector3(Random.Range(-15f, 15f), 0.5f, Random.Range(-15f, 15f));
        Instantiate(foodGO, randomPos, Quaternion.identity);
    }
}
