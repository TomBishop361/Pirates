using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System.Runtime.CompilerServices;

public class MapGenerator : MonoBehaviour
{
    public Mesh mesh;
    MeshData _meshData;
    ObjSpawn spawnObjs;
    public GameObject MeshObj;

    public LocalVolumetricFog fog;

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

    [Range(0, 10)]
    public float a;
    [Range(0, 3)]
    public float b;

    Vector3 spawnOffset;
    public int IslandIntencity;
    public Vector2 SpawnBounds;


    public Dictionary<GameObject, Texture2D> IslandInfo = new Dictionary<GameObject, Texture2D>();
    private List<GameObject> Maps = new List<GameObject>();

    public int successfulIslandSpawns = 0;

    private void Awake()
    {
        falloffMap = FallOffGenerator.GenerateFallOffMap(mapChunkSize, a, b);
    }

    private void Start()
    {

        spawnObjs = GetComponent<ObjSpawn>();
        int dictionaryIndex = 0;
        for (int i = 0; i < IslandIntencity; i++)
        {
            seed = UnityEngine.Random.Range(0, int.MaxValue);
            MapDisplay map = GenerateMap(i);
            if (map != null)
            {
                
                Vector3 digSpot = spawnObjs.SpawnObjs(regions, map);
                for (int j = 0; j < regions.Length; j++)
                {
                    regions[j].vertsInRegion.Clear();
                }

                IslandInfo[IslandInfo.ElementAt(dictionaryIndex).Key] = TextureGenerator.EditTexture(map.meshData, digSpot, map.ColourMap);
                dictionaryIndex++;
            }
        }
        CreateIslandMap2D();


    }




    public void CreateIslandMap2D()//GameObject island
    {
        foreach (Texture2D IslandMap in IslandInfo.Values)
        {

            GameObject Map = new GameObject();
            MeshRenderer meshRenderer = Map.AddComponent<MeshRenderer>();
            MeshFilter mFilter = Map.AddComponent<MeshFilter>();
            mFilter.mesh = mesh;
            meshRenderer.material.mainTexture = IslandMap;//IslandInfo[island];
            //Transforms to align with island facing north
            Map.transform.localScale = new Vector3(-1, 1, 0.06f);
            Map.transform.localEulerAngles = new Vector3(0, 0, 180f);

            Maps.Add(Map);

        }
    }

    public MapDisplay GenerateMap(int p)
    {

        //Validates Spawn Location before spawning in Island
        Collider[] hit = new Collider[1];
        int timeOut = 0;
        bool isValidSpawn = false;
        while (!isValidSpawn)
        {
            spawnOffset = new Vector3((int)UnityEngine.Random.Range(SpawnBounds.x, -SpawnBounds.x), 0, (int)UnityEngine.Random.Range(SpawnBounds.y, -SpawnBounds.y));
            int objInArea = Physics.OverlapSphereNonAlloc(spawnOffset, (mapChunkSize * 5), hit);
            timeOut++;

            if (hit[0] == null)
            {
                isValidSpawn = true;
                successfulIslandSpawns++;
                break;

            }//Prevents Infinate While Loop
            else if (timeOut >= 100000)
            {
                Debug.LogError("Failed To Validate Spawn. Too many attempts");
                return null;
            }
        }


        float[,] noiseMap = perlinNoise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        Color[] MeshColourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                if (useFallOffMap)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }
                float currentHeight = noiseMap[x, y];
                bool MapHeight = false;
                bool MeshMapHeight = false;
                //Builds colour map based on Height for Both Map Texure and Mesh Texture
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height && !MapHeight)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;

                        MapHeight = true;
                    }
                    if(currentHeight <= regions[i].meshHeight && !MeshMapHeight)
                    {
                        MeshColourMap[y * mapChunkSize + x] = regions[i].Meshcolour;
                        MeshMapHeight = true;
                        
                    }
                    if (MeshMapHeight && MapHeight) break;
                }
            }
        }
        
        MapDisplay display = FindObjectOfType<MapDisplay>();
        

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

        //
        display.meshData = _meshData;
        display.ColourMap = colourMap;
        GameObject NewIsland = Instantiate(MeshObj, Vector3.zero + spawnOffset, quaternion.identity);
        NewIsland.gameObject.name = p.ToString();
        display.DrawMesh(NewIsland, _meshData, TextureGenerator.TextureFromColourMap(MeshColourMap, mapChunkSize, mapChunkSize));
        IslandInfo.Add(NewIsland, TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));

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
    public float meshHeight;
    public Color colour;
    public Color Meshcolour;
    [HideInInspector]
    public List<Vector3> vertsInRegion;
}