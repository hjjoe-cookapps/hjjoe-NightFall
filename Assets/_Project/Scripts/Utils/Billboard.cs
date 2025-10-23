using UnityEngine;
using UnityEngine.AI;

public class Billboard : MonoBehaviour
{
    // 2d 카메라 이므로 한번만 수행해도 됨
    private void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }

        transform.rotation = Camera.main.transform.rotation;
    }
}
