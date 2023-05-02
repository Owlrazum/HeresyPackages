using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float speed;
    void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }
}
