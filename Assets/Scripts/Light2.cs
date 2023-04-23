using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light2 : MonoBehaviour
{
    GameObject Laser;
    GameObject Laser2;
    LineRenderer lineRenderer;

    [SerializeField] Material material;
    public float width;

    List<Vector3> laserPoints;
    List<Vector3> laserPoints2;
    List<Vector3> laserPoints3;

    public bool absorb = false;
    public bool refract = false;
    public bool mirrorAbsorb = false;
    private Vector3 savedCoord;

    [SerializeField] int hitCounter = 0;
    [SerializeField] Color absorbColor;
    [SerializeField] string reflectedPlane = "null";

    void LaserSetup(Vector3 addPoint, Color color, List<Vector3> list)
    {

        Laser = new GameObject();
        Laser.name = "Laser";
        lineRenderer = Laser.AddComponent<LineRenderer>();

        lineRenderer.material = material;
        lineRenderer.startWidth = 0.07f;
        lineRenderer.endWidth = 0.07f;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        list.Add(addPoint);
        UpdateLaser(list, Laser);
    }

    void LaserSetup2(Vector3 addPoint, Color color, List<Vector3> list)
    {

        Laser2 = new GameObject();
        Laser2.name = "Laser2";
        lineRenderer = Laser2.AddComponent<LineRenderer>();

        lineRenderer.material = material;
        lineRenderer.startWidth = 0.07f;
        lineRenderer.endWidth = 0.07f;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
 
        list.Add(addPoint);
        UpdateLaser(list, Laser2);

    }

    void CastRay(Vector3 position, Vector3 direction, Color color, List<Vector3> list, GameObject laserObject)
    {
        if (Laser != null)
        {
            Destroy(Laser);
        }

        if (Laser2 != null)
        {
            Destroy(Laser2);
        }


        Ray ray = new Ray(position, direction);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 30, 1))
        {   
           CheckHit(hit, direction, color, list, laserObject);
        }

        else
        {   
            if(absorb == true)
            {
                Laser2 = new GameObject();
                Laser2.name = "Laser2";
                lineRenderer = Laser2.AddComponent<LineRenderer>();
                lineRenderer.useWorldSpace = false;

                lineRenderer.material = material;
                lineRenderer.startWidth = width;
                lineRenderer.endWidth = width;
                lineRenderer.startColor = color;
                lineRenderer.endColor = color;

                reflectedPlane = "null";

                laserPoints2.Add(ray.GetPoint(12));
                UpdateLaser(laserPoints2, Laser2);
            }

            else if(hitCounter == 0)
            {
                Laser = new GameObject();
                Laser.name = "Laser";
                lineRenderer = Laser.AddComponent<LineRenderer>();
                lineRenderer.useWorldSpace = false;

                lineRenderer.material = material;
                lineRenderer.startWidth = width;
                lineRenderer.endWidth = width;
                lineRenderer.startColor = color;
                lineRenderer.endColor = color;

                reflectedPlane = "null";

                list.Add(ray.GetPoint(12));
                UpdateLaser(list, Laser);
            }
            
        }
    }

    void UpdateLaser(List<Vector3> pointList, GameObject gameObject)
    {
        int count = 0;
        LineRenderer lr;
        lr = gameObject.GetComponent<LineRenderer>();
        lr.positionCount = pointList.Count;

        foreach (Vector3 idx in pointList)
        {
            lr.SetPosition(count, idx);
            count++;
        }
    }

    void CheckHit(RaycastHit raycast, Vector3 dir, Color color2, List<Vector3> list, GameObject laserObj)
    {

        if (raycast.collider.tag == "Mirror")
        {

            if (Laser != null)
            {
                Destroy(Laser);
            }
            
            LaserSetup(raycast.point, color2, list);            
            CastRay(raycast.point, Vector3.Reflect(dir, raycast.normal), color2, list, laserObj);

            reflectedPlane = "Mirror";

            if (absorb)
            {
                laserPoints2.Add(raycast.point);
                mirrorAbsorb = true;
            }
        }

        else if (raycast.collider.tag == "ColorAbsorb")
        {
            hitCounter++;
            absorbColor = raycast.collider.GetComponent<Renderer>().material.color;
            absorb = true;

            if (absorbColor == Color.black)
            {
                absorbColor = Color.clear;
            }

            if (mirrorAbsorb && laserPoints2.Count > 0)
            {
                //laserPoints2.RemoveAt(laserPoints2.Count - 1);
            }          
          
            if (Laser2 != null)
            {
                Destroy(Laser2);
            }

            LaserSetup2(raycast.point, absorbColor, laserPoints2);

            

            CastRay(raycast.point, Vector3.Reflect(dir, raycast.normal), absorbColor, laserPoints2, Laser2);

            if (Laser != null)
            {
                Destroy(Laser);
            }
            LaserSetup(raycast.point, Color.white, laserPoints);


            laserPoints.RemoveAt(laserPoints.Count - 1);

        }

        else if(raycast.collider.tag == "Refract")
        {
            hitCounter++;

            list.Add(raycast.point);
            
            Vector3 pos = raycast.point;
            refract = true;            

            Vector3 newPos1 = new Vector3(Mathf.Abs(dir.x) / (dir.x + 0.0001f) * 0.001f + pos.x, Mathf.Abs(dir.y) / (dir.y + 0.0001f) * 0.001f + pos.y, Mathf.Abs(dir.z) / (dir.z + 0.0001f) * 0.001f + pos.z);

            float n1 = 1.0f;
            float n2 = 1.4f;

            Vector3 norm = raycast.normal;
            Vector3 incident = dir;

            Vector3 refractedVector = RefractFunc(n1, n2, norm, incident);

            Ray ray1 = new Ray(newPos1, refractedVector);
            Vector3 newRayStartPos = ray1.GetPoint(1.78f);

            Ray ray2 = new Ray(newRayStartPos, -refractedVector);
            RaycastHit hit2;

            if (Physics.Raycast(ray2, out hit2, 1.9f, 1))
            {
                list.Add(hit2.point);
            }

            UpdateLaser(list, laserObj);

            Vector3 refractedVector2 = RefractFunc(n2, n1, -hit2.normal, refractedVector);

            Ray ray3 = new Ray(hit2.point, refractedVector2);

            CastRay(hit2.point, refractedVector2, color2, list, laserObj);
        }

        else
        {
            hitCounter++;
            if (laserObj != null)
            {
                Destroy(laserObj);
            }

            LaserSetup(raycast.point, color2, list);
        }
    }

    void Update()
    {
        hitCounter = 0;

        if(hitCounter == 0)
        {
            absorb = false;
        }

        if (Laser != null)
        {
            Destroy(Laser);
        }

        if (Laser2 != null)
        {
            Destroy(Laser2);
        }

        laserPoints = new List<Vector3>();
        laserPoints2 = new List<Vector3>();
        laserPoints.Add(transform.position);

        CastRay(transform.position, transform.forward, Color.white, laserPoints, Laser);
    }

    Vector3 RefractFunc(float RI1, float RI2, Vector3 surfNorm, Vector3 incident)
    {

        surfNorm.Normalize();
        incident.Normalize();

        return (RI1 / RI2 * Vector3.Cross(surfNorm, Vector3.Cross(-surfNorm, incident)) - surfNorm * Mathf.Sqrt(1 - Vector3.Dot(Vector3.Cross(surfNorm, incident) * (RI1 / RI2 * RI1 / RI2), Vector3.Cross(surfNorm, incident)))).normalized;
    }
}
