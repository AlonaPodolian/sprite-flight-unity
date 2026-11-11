using UnityEngine;

public class MoveRight : MonoBehaviour
{
    [SerializeField] private float speed = 1f;

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}
