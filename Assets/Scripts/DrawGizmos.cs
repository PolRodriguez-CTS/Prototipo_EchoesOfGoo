using UnityEngine;

public class DrawGizmos : MonoBehaviour
{
    [SerializeField] private Transform _trigger;
    [SerializeField] private Vector3 _size;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_trigger.position, _size);
    }
}