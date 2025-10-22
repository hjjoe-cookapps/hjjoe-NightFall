using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Billboard : MonoBehaviour
{
    [SerializeField]
    private bool _isContinuous = true;

    // 2d 카메라 이므로 한번만 수행해도 됨
    private void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updateRotation = false;
        }

        transform.rotation = Camera.main.transform.rotation;
    }

    private void LateUpdate()
    {
        if (_isContinuous)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
