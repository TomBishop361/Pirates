using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))] 
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;


    Vector3[] Verts;
    int[] triangles;
    Color[] colors;

    public int xSize = 20;
    public int zSize = 20;


    public float scale = 20;

    public float offSetX = 100;
    public float offSetY = 100;

    public float mag = 1;

    public Gradient gradient;

    float minTerrainHeight;
    float maxTerrainHeight;

    





    // Start is called before the first frame update
    void Start()
    {
        offSetX = UnityEngine.Random.Range(0f, 99999f);
        offSetY = UnityEngine.Random.Range(0f, 99999f);

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //createShape();
        //updateMesh();
    }
   

    private void updateMesh()
    {
        mesh.Clear();

        mesh.vertices = Verts;
        mesh.triangles = triangles;
        mesh.colors = colors;


        mesh.RecalculateNormals();
    }

    private void createShape()
    {
        //Verticies
        Verts = new Vector3[(xSize + 1) * (zSize + 1)];


        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                //float[,] noiseMap = perlinNoise.GenerateNoiceMap(xSize, zSize, scale);
                float y =  CalcNoise(x, z)*mag; 
                Verts[i] = new Vector3(x, y, z);

                if (y > maxTerrainHeight) maxTerrainHeight = y;

                if (y < minTerrainHeight) minTerrainHeight = y;

                i++;
            }
        }


        //Triangles
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;


        for (int z= 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {

                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        //UVS

        colors = new Color[Verts.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight,maxTerrainHeight , Verts[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }


    }

    float CalcNoise(int x, int y)
    {
        float xCoord = (float)x / xSize * scale + offSetX;
        float yCoord = (float)y / zSize * scale + offSetY;



        return Mathf.PerlinNoise(xCoord, yCoord);
        
    }

//#if UNITY_EDITOR

//    private void OnDrawGizmos()
//    {
//        if (Verts == null) return;

//        for (int i = 0; i < Verts.Length; i++)
//        {
//            Gizmos.DrawSphere(Verts[i], .1f);
//        }
//    }


//#endif


    // Update is called once per frame
    void Update()
    {
        createShape();
        updateMesh();
    }
}
