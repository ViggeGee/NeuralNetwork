using UnityEngine;

public class KillCamera : MonoBehaviour
{
    [SerializeField]Agent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (agent.dead)
            Destroy(this.gameObject);
    }
}
