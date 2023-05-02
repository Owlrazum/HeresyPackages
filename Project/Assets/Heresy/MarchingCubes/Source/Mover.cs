using UnityEngine;

public class Mover : MonoBehaviour
{
    const float TAU = Mathf.PI * 2;

    [SerializeField]
    float radianSpeed = 1;

    [SerializeField]
    float periodOffset = TAU / 3;

    [SerializeField]
    float radius = 2;

    Transform[] toMove;
    float[] periods;

    void Awake()
    {
        toMove = new Transform[transform.childCount];
        periods = new float[toMove.Length];
        for (int i = 0; i < toMove.Length; i++)
        {
            toMove[i] = transform.GetChild(i);
            periods[i] = periodOffset * i;
        }
    }

    void Update()
    {
        for (int i = 0; i < toMove.Length; i++)
        {
            if (i % 2 == 1)
            {
                toMove[i].localPosition = new Vector3(Mathf.Cos(periods[i]) * radius, Mathf.Sin(periods[i]) * radius, 0);
            }
            else
            { 
                toMove[i].localPosition = new Vector3(Mathf.Cos(periods[i]) * radius, 0, Mathf.Sin(periods[i]) * radius);
            }
            periods[i] += Time.deltaTime * radianSpeed;
        }
    }
}
