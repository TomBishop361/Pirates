using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.Rendering;

public class MapGenerator : MonoBehaviour
{

    MeshData _meshData;
    ObjSpawn spawnObjs;
    public GameObject MeshObj;

    public enum DrawMode { NoiseMap, ColourMap, Mesh };
    public DrawMode drawMode;

    const int mapChunkSize = 241;
    [Range(0, 6)]
    public int levelOfDetail;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainType[] regions;

    public bool useFallOffMap;

    float[,] falloffMap;

    [Range(0,10)]
    public float a;
    [Range(0,3)]
    public float b;

    Vector3 spawnOffset;
    public int IslandIntencity;
    public Vector2 SpawnBounds;

    private void Awake()
    {
        falloffMap = FallOffGenerator.GenerateFallOffMap(mapChunkSize,a,b);
    }

    private void Start()
    {
        
        spawnObjs = GetComponent<ObjSpawn>();
        for (int i = 0; i < IslandIntencity; i++)
        {
            seed = UnityEngine.Random.Range(0, int.MaxValue);
            MapDisplay map = GenerateMap();
            spawnObjs.SpawnObjs(regions, map);
            for (int j = 0; j < regions.Length; j++)
            {
                regions[j].vertsInRegion.Clear();
            }
        }
       
    }

    public MapDisplay GenerateMap()
    {
        float[,] noiseMap = perlinNoise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                if (useFallOffMap)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;

                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            _meshData = MeshGen.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail);

            for (int x = 0; x < mapChunkSize * mapChunkSize; x++)
            {
                for (int i = 0; i < regions.Length; i++)
                {
                    if (_meshData.height[x] <= regions[i].height && _meshData.height[x] > 0.3f)
                    {
                        regions[i].vertsInRegion.Add(_meshData.vertices[x]);
                        break;
                    }
                }
            }


            //Validates Spawn Location before spawning in Island
            Collider[] hit = new Collider[1];
            int timeOut = 0;
            bool isValidSpawn = false;
            while (!isValidSpawn)
            {
                spawnOffset = new Vector3(UnityEngine.Random.Range(SpawnBounds.x, -SpawnBounds.x), 0, UnityEngine.Random.Range(SpawnBounds.y, -SpawnBounds.y));
                int objInArea = Physics.OverlapSphereNonAlloc(spawnOffset, mapChunkSize * 2, hit);
                Debug.Log(hit[0]);
                timeOut++;
                if (hit[0] == null)
                {
                    isValidSpawn = true;
                }//Prevents Infinate While Loop
                else if (timeOut >= 1000)
                {
                    Debug.LogError("Failed To Validate Spawn");
                    break;
                }
            }


            GameObject NewIsland = Instantiate(MeshObj, Vector3.zero + spawnOffset, quaternion.identity);

            display.DrawMesh(NewIsland, _meshData, TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));


        }
        return display;

    }

    void OnValidate()
    {
       
        falloffMap = FallOffGenerator.GenerateFallOffMap(mapChunkSize, a, b);
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(SpawnBounds.x, 0, 0), new Vector3(-SpawnBounds.x,0,0));
        Gizmos.DrawLine(new Vector3(0, 0, SpawnBounds.y), new Vector3(0, 0, -SpawnBounds.y));
        


    }
}



[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
    [HideInInspector]
    public List<Vector3> vertsInRegion;
}