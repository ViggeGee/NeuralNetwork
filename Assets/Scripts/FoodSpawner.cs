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
        Vector3 randomPos = new Vector3(Random.Range(-20f, 20f), 0.5f, Random.Range(-20f, 20f));
        Instantiate(foodGO, randomPos, Quaternion.identity);
    }
}
