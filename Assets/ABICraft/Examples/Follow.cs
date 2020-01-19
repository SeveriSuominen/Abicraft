using AbicraftMonos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    // Start is called before the first frame update
    public AbicraftObject abicraftObject;
    public SkinnedMeshRenderer renderer;
    public Vector3 offset;

    private ParticleSystem ps;
    private ParticleSystemRenderer psr;
    public Vector3 pivot;
    public float rateSeconds;
    float rateSecondsElapsed;

    Mesh mesh;

    List<Mesh> meshes = new List<Mesh>();

    void Start()
    {

        ps  = GetComponent<ParticleSystem>();
        psr = GetComponent<ParticleSystemRenderer>();


        //var rotation = ps.rotationOverLifetime;
        //rotation.enabled = true;
        //rotation.zMultiplier = 180.0f;  // spin so we can see the pivot more easily
    }

    float rotatedAngleY = 1f;
    public int detail = 5;
    // Update is called once per frame
    void FixedUpdate()
    {
        rateSecondsElapsed += Time.deltaTime;

        if (rateSecondsElapsed < rateSeconds)
            return;

        rateSecondsElapsed = 0;

        mesh = new Mesh();
        renderer.BakeMesh(mesh);

        Vector3[] verteces = mesh.vertices;

        Quaternion angle = Quaternion.AngleAxis(rotatedAngleY, Vector3.up);

        rotatedAngleY = abicraftObject.transform.rotation.eulerAngles.y;
        
        if ((this.ps != null) && (this.ps.particleCount != 0))
        {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[this.ps.particleCount];

            int count = mesh.vertexCount;// this.ps.GetParticles(particles);
            var emitParams = new ParticleSystem.EmitParams();

            for (int i = 0; i + detail < count; i += detail)
            {
                //emitParams.particle.me
                emitParams.position = (angle * verteces[i] )+ abicraftObject.transform.position;
                //Debug.Log(verteces[i].ToString("F4"));  
                ps.Emit(emitParams, 1);

            }
            emitParams.applyShapeToPosition = true;
        }

        Destroy(mesh);
    }



}
