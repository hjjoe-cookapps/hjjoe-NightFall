using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class MonsterBehaviour : MonoBehaviour
{
    private NavMeshAgent _agent;

    [SerializeReference]
    private GameObject _target;

    private Coroutine moveCoroutine;

    private void Start()
    {
        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        moveCoroutine = StartCoroutine(MoveToTarget());
    }

    private IEnumerator MoveToTarget()
    {
        while (true)
        {
            _agent.SetDestination(_target.transform.position);
            yield return CoroutineManager.WaitForSeconds(0.2f);
        }
    }
}
