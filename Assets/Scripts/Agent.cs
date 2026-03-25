using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Agent : MonoBehaviour
{
    //dynamiskt muterande
    public float baseMutationAmount = 0.5f;
    public float baseMutationChance = 0.2f;
    private float currentMutationAmount;
    private float currentMutationChance;
    public bool poorPerformance = false;
    public int consecutivePoorPerformanceCount = 0;
    private const int maxConsecutivePoorPerformance = 3;
    public float increaseMutationAmount = 0.1f; 
    public float increaseMutationChance = 0.2f;

    //Scripts & Components
    public Material deadMat;
    public FoodSpawner foodSpawner;
    
    public NN nn;
    public Movement movement;

    static public int reproductionCount = 0;
    public bool mutateMutations = true;
    public GameObject agentPrefab;

    //Movement
    public float FB = 0;
    public float LR = 0;

    //Stats
    public bool dead = false;
    public float viewDistance = 20;
    public float energy = 10;
    public float energyGained = 10;
    public float size = 1.0f;
    public float reproductionEnergyGained = 1;
    public float reproductionEnergy = 0;
    public float reproductionEnergyThreshold = 10;
    public float[] distances = new float[9];
    public bool canEat = true;
    float elapsed = 0f;

    //Mutations
    private bool isMutated = false;

    //Old
    //public float mutationAmount = 0.8f;
    //public float mutationChance = 0.2f;
    //private List<GameObject> edibleFoodList = new List<GameObject>();
    //public float lifeSpan = 0f;
    //public bool isUser = false;
    //public int numberOfChildren = 1;
    //float relativeFoodX;
    //float relativeFoodZ;
    private void Start()
    {
       foodSpawner = FindAnyObjectByType<FoodSpawner>();
    }
    private void FixedUpdate()
    {
        //Ifall agenten dör så ska NN reevalueras. Detta görs här
        CheckAndInitializeMutation();
        ManageEnergy();
        distances = Sense();
        ThinkAndMove();
    }

    private float[] Sense()
    {
        return CreateRayCasts(9, 15);
    }

    private void ThinkAndMove()
    {
        float[] outputsFromNN = nn.Brain(distances);
        FB = outputsFromNN[0];
        LR = outputsFromNN[1];
        movement.Move(FB, LR);
    }

    public void ManageEnergy()
    {
        if (dead) return;

        elapsed += Time.fixedDeltaTime;
        if (elapsed >= 1f)
        {
            elapsed = elapsed % 1f;

            energy -= 1f;
        }

        float agentY = this.transform.position.y;
        if (energy <= 0 || agentY < -10)
        {
            consecutivePoorPerformanceCount++;

            if (consecutivePoorPerformanceCount >= maxConsecutivePoorPerformance)
            {
                poorPerformance = true;
            }

            reproductionEnergy = 0;
            CreateAgentFromOld();
            //Destroy(this.gameObject);
            Debug.Log("Reevaluated ANN");
            Debug.Log("Agent: " + reproductionCount);
            //GetComponent<Movement>().enabled = false;
        }


        //if(reproductionEnergy >= reproductionEnergyThreshold)
        //{
        //    reproductionEnergy = 0;
        //    Reproduce();
        //}
    }

    public void CreateAgentFromOld()
    {

        //Skapar en ny agent vars NN kopieras från den gamla.
        GameObject child = Instantiate(agentPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        child.GetComponent<NN>().layers = GetComponent<NN>().CopyLayers();

        //För över poor performance counter till nästa barn
        Agent childAgent = child.GetComponent<Agent>();
        childAgent.consecutivePoorPerformanceCount = consecutivePoorPerformanceCount;
        


        child.name = "Agent" + " " + reproductionCount;
        reproductionCount++;
        reproductionEnergy = 0;
        //Destroy(this.gameObject);


        //Sätt på nytt material för att indikera på död agent
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.material = deadMat;
        }
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Movement>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponent<NN>().enabled = false;
        GetComponent<Agent>().enabled = false;
        dead = true;
    }


    //Skapa raycasts och returnera array av distanser till funna mat objekt
    public float[] CreateRayCasts(int numRaycasts, float angleBetweenRaycasts)
    {
        float[] distances = new float[numRaycasts];

        RaycastHit hit;
        int layerMask = ~LayerMask.GetMask("Agent");

        for (int i = 0; i < numRaycasts; i++)
        {
            float angle = ((2 * i + 1 - numRaycasts) * angleBetweenRaycasts / 2);
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 rayDirection = rotation * transform.forward * -1;

            Vector3 rayStart = transform.position + Vector3.up * 0.1f;
            if (Physics.Raycast(rayStart, rayDirection, out hit, viewDistance, layerMask))
            {
                Debug.DrawRay(rayStart, rayDirection * hit.distance, Color.red);
                if (hit.transform.gameObject.tag == "Food")
                {
                    distances[i] = hit.distance / viewDistance;
                }
                else
                {
                    distances[i] = 1;
                }
            }
            else
            {
                Debug.DrawRay(rayStart, rayDirection * viewDistance, Color.red);
                distances[i] = 1;
            }
        }
        return (distances);
    }
    private void CheckAndInitializeMutation()
    {
        if (!isMutated)
        {
            MutateAgent();
            this.transform.localScale = new Vector3(size, size, size);
            isMutated = true;
            energy = 10;
        }
    }


    private void MutateAgent()
    {
        if (mutateMutations)
        {
            if (poorPerformance)
            {
                currentMutationAmount = baseMutationAmount + increaseMutationAmount;
                currentMutationChance = baseMutationChance + increaseMutationChance;
            }
            else
            {
                currentMutationAmount = baseMutationAmount;
                currentMutationChance = baseMutationChance;
            }


            //mutationAmount += Random.Range(-0.5f, 0.5f) / 100;
            //mutationChance += Random.Range(-0.5f, 0.5f) / 100;
        }

        //mutationAmount = Mathf.Max(mutationAmount, 0);
        //mutationChance = Mathf.Max(mutationChance, 0);

        //nn.MutateNetwork(mutationAmount, mutationChance);

        currentMutationAmount = Mathf.Max(currentMutationAmount, 0);
        currentMutationChance = Mathf.Max(currentMutationChance, 0);
        nn.MutateNetwork(currentMutationChance, currentMutationAmount);

    }
    void OnTriggerEnter(Collider col)
    {
        //agenten äter mat som den kolliderar med. Höjer energin
        if (col.gameObject.tag == "Food" && canEat)
        {
            energy += energyGained;
            reproductionEnergy += reproductionEnergyGained;
            Destroy(col.gameObject);
            foodSpawner.SpawnFood();

            // Resettar poor performance indikatorer
            consecutivePoorPerformanceCount = 0;
            poorPerformance = false;
        }
    }
}
