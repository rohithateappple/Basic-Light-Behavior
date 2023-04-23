using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBehavoir : MonoBehaviour
{

    GameObject Laser;
    LineRenderer lineRenderer;

    List<Vector3> laserPoints;

    Ray ray;
    RaycastHit rayHit;

    public Material material;
    public Color color;
    public float width;

    Vector3 rayPos;
    Vector3 rayDir;

    void Update()
    {

        rayPos = transform.position;
        rayDir = transform.forward;

        ray = new Ray(rayPos, rayDir);

        laserPoints = new List<Vector3>();

        laserPoints.Add(transform.position);

        if (Laser != null)
        {
            Destroy(Laser);
        }       

        if (Physics.Raycast(ray, out rayHit, 30, 1))
        {   
            if(rayHit.collider.tag == "Mirror")
            {
                Absorb(rayHit);
                rayPos = rayHit.point;
                rayDir = Vector3.Reflect(rayDir, rayHit.normal);
            }

            else
            {
                Laser = new GameObject();
                Laser.name = "Laser";
                lineRenderer = Laser.AddComponent<LineRenderer>();

                lineRenderer.material = material;
                lineRenderer.startWidth = 0.07f;
                lineRenderer.endWidth = 0.07f;
                lineRenderer.startColor = Color.cyan;
                lineRenderer.endColor = Color.cyan;

                laserPoints.Add(rayHit.point);

                UpdateLaser();
            }
            
        }

        else
        {              
            Laser = new GameObject();
            Laser.name = "Laser";
            lineRenderer = Laser.AddComponent<LineRenderer>();

            lineRenderer.material = material;
            lineRenderer.startWidth = 0.07f;
            lineRenderer.endWidth = 0.07f;
            lineRenderer.startColor = Color.cyan;
            lineRenderer.endColor = Color.cyan;

            laserPoints.Add(ray.GetPoint(40));
            UpdateLaser();
        }
    }
    void UpdateLaser()
    {
        int count = 0;
        lineRenderer.positionCount = laserPoints.Count;

        foreach (Vector3 idx in laserPoints)
        {
            lineRenderer.SetPosition(count, idx);
            count++;
        }
    }

    void Absorb(RaycastHit rayHit)
    {
        Laser = new GameObject();
        Laser.name = "Laser";
        lineRenderer = Laser.AddComponent<LineRenderer>();

        lineRenderer.material = material;
        lineRenderer.startWidth = 0.07f;
        lineRenderer.endWidth = 0.07f;
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;

        laserPoints.Add(rayHit.point);
        UpdateLaser();
    }

}
