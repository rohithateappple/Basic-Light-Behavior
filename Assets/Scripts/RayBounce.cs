using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayBounce : MonoBehaviour
{
    public int rayBounces;

    void Update()
    {
        CastRay(transform.position, transform.forward);
    }

    void CastRay(Vector3 pos, Vector3 dir)
    {
        for(int i = 0; i < rayBounces; i++)
        {
            Ray ray = new Ray(pos, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10, 1))
            {
                Debug.DrawLine(pos, hit.point, Color.red);
                pos = hit.point;
                dir = hit.normal;
            }

            else if (hit.collider.tag == "Mirror")
            {
                Debug.DrawLine(pos, dir * 10, Color.green);
            }

            else
            {
                Debug.DrawLine(pos, dir * 10, Color.blue);
                break;
            }
        }
    }
}
