using UnityEngine;

public class Billboard : MonoBehaviour
{
    // 2d 카메라 이므로 한번만 수행해도 됨
    private void Start()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
