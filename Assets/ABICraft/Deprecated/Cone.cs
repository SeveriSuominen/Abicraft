using AbicraftMonos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class Cone : MonoBehaviour
{

   // public TextMesh fmprefab;
    public AbicraftObject obj;
    public GameObject cursor;

    public MeshFilter   coneFilter;
    public MeshCollider coneCollider;

    public readonly List<AbicraftObject> collisions = new List<AbicraftObject>();

    private const int CircleSegmentCount = 8;
    private const int CircleVertexCount = CircleSegmentCount + 2;
    private const int CircleIndexCount = CircleSegmentCount * 6;

    [HideInInspector]
    public Vector3 position, direction;

    [Range(0f, 100f)]
    public float radiusInner = .5f, radiusOuter = .30f;
    [Range(1, 360f)]
    public float angle;
    float lastAngle, radiusInnerLast, radiusOuterLast;

    void Awake()
    {
        coneFilter = GetComponent<MeshFilter>();
        coneCollider = GetComponent<MeshCollider>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        var abj = collider.GetComponent<AbicraftObject>();

        if (abj && !collisions.Contains(abj))
        {
            collisions.Add(abj);
        }
    }

    void Update()
    {
        if (cursor)
        {
            if (radiusInnerLast != radiusInner || radiusOuterLast != radiusOuter || angle != lastAngle)
            {
                Destroy(coneFilter.sharedMesh);

                coneFilter.sharedMesh = tube();
                coneCollider.sharedMesh = coneFilter.sharedMesh;

                radiusInnerLast = radiusInner;
                lastAngle = angle;
                radiusOuterLast = radiusOuter;
            }

            //transform.rotation = Quaternion.Euler(obj.transform.rotation.eulerAngles + new Vector3(0, angle * 0.5f -90f, 0));
            Vector3 look;

            if (cursor)
            {
                look = (cursor.transform.position - obj.transform.position).normalized;
            }
            else
            {
                look = direction;
            }

            look.y = 0;

            transform.rotation = Quaternion.Euler(Quaternion.LookRotation(look).eulerAngles + new Vector3(0, angle * 0.5f - 90f, 0));
            transform.position = obj.transform.position;
        }
    }

    public void Create()
    {
        Destroy(coneFilter.sharedMesh);

        coneFilter.sharedMesh = tube();
        coneCollider.sharedMesh = coneFilter.sharedMesh;

        Vector3 look;

        look = direction;
        look.y = 0;

        transform.rotation = Quaternion.Euler(Quaternion.LookRotation(look).eulerAngles + new Vector3(0, angle * 0.5f - 90f, 0));
        transform.position = position;
    }

    private Mesh tube()
    {
        Mesh mesh = new Mesh();

        float height = 0.5f;
        int nbSides = 32;

        //transform.rotation = (Quaternion.LookRotation(obj.transform.forward));
     

        // Outter shell is at radius1 + radius2 / 2, inner shell at radius1 - radius2 / 2
        float bottomRadius1 = ( radiusOuter * 0.5f + radiusInner) * 0.1f;
        float bottomRadius2 = (radiusOuter - radiusInner * 2f) * 0.1f;
        float topRadius1 = (radiusOuter * 0.5f + radiusInner) * 0.1f;
        float topRadius2 =( radiusOuter - radiusInner * 2f) * 0.1f;

        int nbVerticesCap   = (nbSides * 2 + 2);
        int nbVerticesSides = (nbSides * 2 + 2);

        int circle = (int)(angle < 360f - nbSides ? 1 : 0);

        //nbSides /= 2;
        #region Vertices

        // bottom + top + sides
        Vector3[] vertices = new Vector3[nbVerticesCap * 2 + nbVerticesSides * 2];
        int vert = 0;
        float _2pi = Mathf.PI * (circle == 1 ? (angle) / 180f : 2f);

        // Bottom cap
        int sideCounter = 0;
        while (vert < nbVerticesCap)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);
            vertices[vert] = new Vector3(cos * (bottomRadius1 - bottomRadius2 * .5f), 0f, sin * (bottomRadius1 - bottomRadius2 * .5f));
            vertices[vert + 1] = new Vector3(cos * (bottomRadius1 + bottomRadius2 * .5f), 0f, sin * (bottomRadius1 + bottomRadius2 * .5f));
            vert += 2;
        }

        // Top cap
        sideCounter = 0;
        while (vert < nbVerticesCap * 2)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);
            vertices[vert] = new Vector3(cos * (topRadius1 - topRadius2 * .5f), height, sin * (topRadius1 - topRadius2 * .5f));
            vertices[vert + 1] = new Vector3(cos * (topRadius1 + topRadius2 * .5f), height, sin * (topRadius1 + topRadius2 * .5f));
            vert += 2;
        }

        // Sides (out)
        sideCounter = 0;
        while (vert < nbVerticesCap * 2 + nbVerticesSides)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);

            vertices[vert] = new Vector3(cos * (topRadius1 + topRadius2 * .5f), height, sin * (topRadius1 + topRadius2 * .5f));
            vertices[vert + 1] = new Vector3(cos * (bottomRadius1 + bottomRadius2 * .5f), 0, sin * (bottomRadius1 + bottomRadius2 * .5f));
            vert += 2;
        }

        // Sides (in)
        sideCounter = 0;
        while (vert < vertices.Length)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);

            vertices[vert] = new Vector3(cos * (topRadius1 - topRadius2 * .5f), height, sin * (topRadius1 - topRadius2 * .5f));
            vertices[vert + 1] = new Vector3(cos * (bottomRadius1 - bottomRadius2 * .5f), 0, sin * (bottomRadius1 - bottomRadius2 * .5f));
            vert += 2;
        }
        #endregion

        #region Normales

        // bottom + top + sides
        Vector3[] normales = new Vector3[vertices.Length];
        vert = 0;

        // Bottom cap
        while (vert < nbVerticesCap)
        {
            normales[vert++] = Vector3.down;
        }

        // Top cap
        while (vert < nbVerticesCap * 2)
        {
            normales[vert++] = Vector3.up;
        }

        // Sides (out)
        sideCounter = 0;
        while (vert < nbVerticesCap * 2 + nbVerticesSides)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;

            normales[vert] = new Vector3(Mathf.Cos(r1), 0f, Mathf.Sin(r1));
            normales[vert + 1] = normales[vert];
            vert += 2;
        }

        // Sides (in)
        sideCounter = 0;
        while (vert < vertices.Length)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;

            normales[vert] = -(new Vector3(Mathf.Cos(r1), 0f, Mathf.Sin(r1)));
            normales[vert + 1] = normales[vert];
            vert += 2;
        }
        #endregion

        #region UVs
        Vector2[] uvs = new Vector2[vertices.Length];

        vert = 0;
        // Bottom cap
        sideCounter = 0;
        while (vert < nbVerticesCap)
        {
            float t = (float)(sideCounter++) / nbSides;
            uvs[vert++] = new Vector2(0f, t);
            uvs[vert++] = new Vector2(1f, t);
        }

        // Top cap
        sideCounter = 0;
        while (vert < nbVerticesCap * 2)
        {
            float t = (float)(sideCounter++) / nbSides;
            uvs[vert++] = new Vector2(0f, t);
            uvs[vert++] = new Vector2(1f, t);
        }

        // Sides (out)
        sideCounter = 0;
        while (vert < nbVerticesCap * 2 + nbVerticesSides)
        {
            float t = (float)(sideCounter++) / nbSides;
            uvs[vert++] = new Vector2(t, 0f);
            uvs[vert++] = new Vector2(t, 1f);
        }

        // Sides (in)
        sideCounter = 0;
        while (vert < vertices.Length)
        {
            float t = (float)(sideCounter++) / nbSides;
            uvs[vert++] = new Vector2(t, 0f);
            uvs[vert++] = new Vector2(t, 1f);
        }
        #endregion

        #region Triangles
        int nbFace = nbSides * 4;
        int nbTriangles = nbFace * 2;
        int nbIndexes = nbTriangles * 3;
        int[] triangles = new int[nbIndexes];

        // Bottom cap
        int i = 0, step;
        sideCounter = 0;

        while (sideCounter < nbSides - circle)
        {
            int current = sideCounter * 2;
            int next = sideCounter * 2 + 2;

            triangles[i++] = next + 1;
            triangles[i++] = next;
            triangles[i++] = current;

            triangles[i++] = current + 1;
            triangles[i++] = next + 1;
            triangles[i++] = current;

            sideCounter++;
        }

        step = sideCounter;
        // Top cap
        while (sideCounter < nbSides * 2 - circle)
        {
            int current = sideCounter * 2 + 2;
            int next = sideCounter * 2 + 4;

            triangles[i++] = current;
            triangles[i++] = next;
            triangles[i++] = next + 1;

            if(sideCounter < nbSides * 2)
            {
                triangles[i++] = current;
                triangles[i++] = next + 1;
                triangles[i++] = current + 1;

            }

            sideCounter++;
        }

        step = sideCounter;
        // Sides (out)
        while (sideCounter < nbSides * 3 - circle)
         {
             int current = sideCounter * 2 + 4;
             int next = sideCounter * 2 + 6;

             triangles[i++] = current;
             triangles[i++] = next;
             triangles[i++] = next + 1;

             triangles[i++] = current;
             triangles[i++] = next + 1;
             triangles[i++] = current + 1;

             sideCounter++;
         }

        step = sideCounter;
        // Sides (in)
        while (sideCounter < (nbSides) * 4)
        {
            int current = sideCounter * 2 + 6;
            int next = sideCounter * 2 + 8;

            if((sideCounter != (nbSides) * 4 - 18 || sideCounter != (nbSides) * 4 - 19) || circle == 0)
            {
                if (sideCounter < nbSides * 4 - circle)
                {
                    triangles[i++] = next + 1;
                    triangles[i++] = next;
                    triangles[i++] = current;

                    triangles[i++] = current + 1;
                    triangles[i++] = next + 1;
                    triangles[i++] = current;
                }
                else
                {
                    triangles[i++] = 194;
                    triangles[i++] = 62;
                    triangles[i++] = 260;

                    triangles[i++] = 62;
                    triangles[i++] = 63;
                    triangles[i++] = 194;

                }
            }
            sideCounter++;
        }
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        //mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        //mesh.Optimize();
        
        return mesh;

        /*int x = 0;
        foreach (Vector3 pos in vertices)
        {
            TextMesh tm = GameObject.Instantiate(fmprefab, pos, Quaternion.identity) as TextMesh;
            tm.name = (x).ToString();
            tm.text = (x++).ToString();
            tm.transform.parent = transform;
        }*/
    }
}
 