using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OrbitLine : MonoBehaviour
{
    private int radius;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetRadius (int r)
    {
        this.radius = r;

        LineRenderer lineRenderer = this.GetComponent<LineRenderer>();
        Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));
        lineRenderer.material = whiteDiffuseMat;
        lineRenderer.startWidth = 2f;
        lineRenderer.endWidth = 2f;
        //lineRenderer.numCornerVertices = 5;
        //lineRenderer.numCapVertices = 5;
        int positionCount = 361;
        lineRenderer.positionCount = positionCount;

        double x = 0;
        double z = 0;
        for (int i = 0; i < positionCount; i++)
        {
            double theta = i * Constants.RAD_PER_DEG;
            x = radius * Math.Cos(theta);
            z = radius * Math.Sin(theta);

            Vector3 pos = new Vector3((float)x, 0, (float)z);
            lineRenderer.SetPosition(i, pos);
        }
    }
}
