using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    float time;
    public float speed;
    void Update()
    {
        if (time < 4)
        { 
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
            time += Time.deltaTime;
        }
        else
        {
            transform.Translate(-transform.forward * speed * Time.deltaTime, Space.World);
            time += Time.deltaTime;
            if (time > 8)
            {
                time = 0;
            }
        }
    }
}
