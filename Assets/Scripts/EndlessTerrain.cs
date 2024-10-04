//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//public class EndlessTerrain : MonoBehaviour
//{
//    const float Scale = 5f;

//    const float viewMoveThresholdForChunkUpdate = 25f;
//    const float sqrViewMoveThresholdForChunkUpdate = viewMoveThresholdForChunkUpdate* viewMoveThresholdForChunkUpdate;


//    public static float MaxViewDst;
//    public LODInfo[] DetailLevels;

//    public Transform viewer;
//    public Material mapMaterial;

//    public static Vector2 ViewerPosition;
//    Vector2 viewerPositionOld;
//    public static MapGenerator mapGenerator;
//    int chunkSize;
//    int chunkVisibleInViewDst;

//    Dictionary<Vector2, terrainChunk> terrainChunkDictionary = new Dictionary<Vector2, terrainChunk>();
//    static List<terrainChunk> terrainChunksVisibleLastUpdate = new List<terrainChunk>();

//    private void Start()
//    {
//        mapGenerator = FindObjectOfType<MapGenerator>();

//        MaxViewDst = DetailLevels[DetailLevels.Length - 1].visibleDstThreshold;
//        chunkSize = MapGenerator.mapChunkSize - 1;
//        chunkVisibleInViewDst = Mathf.RoundToInt(MaxViewDst / chunkSize);
//        updateVisibleChunks();
//    }

//    private void Update()
//    {
//        ViewerPosition = new Vector2(viewer.position.x, viewer.position.z)/ Scale;

//        if ((viewerPositionOld - ViewerPosition).sqrMagnitude > sqrViewMoveThresholdForChunkUpdate)
//        {
//            viewerPositionOld = ViewerPosition;
//            updateVisibleChunks();
//        }
//    }

//    void updateVisibleChunks()
//    {

//        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
//        {
//            terrainChunksVisibleLastUpdate[i].setVisible(false);
//        }
//        terrainChunksVisibleLastUpdate.Clear();

//        int currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / chunkSize);
//        int currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / chunkSize);


//        for (int yOffset = -chunkVisibleInViewDst; yOffset <= chunkVisibleInViewDst; yOffset++) {
//            for (int xOffset = -chunkVisibleInViewDst; xOffset <= chunkVisibleInViewDst; xOffset++) {
//                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

//                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
//                {
//                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();                    
//                }
//                else
//                {
//                    terrainChunkDictionary.Add(viewedChunkCoord, new terrainChunk(viewedChunkCoord, chunkSize,DetailLevels, this.transform, mapMaterial));//Random A b
//                }
//            }
//        }
//    }

//    public class terrainChunk
//    {
//        GameObject meshObject;
//        Vector2 position;
//        Bounds bounds;

//        MeshRenderer meshRenderer;
//        MeshFilter meshFilter;

//        LODInfo[] detailLevels;
//        LODMesh[] lodMeshes;

//        MapData mapData;
//        bool mapDataReceived;
//        int previousLODIndex = -1;

        

//        public terrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform Parent, Material material)
//        {                       

//            this.detailLevels = detailLevels;
//            position = coord * size;
//            bounds = new Bounds(position, Vector2.one * size);
//            Vector3 positionv3 = new Vector3(position.x, 0, position.y);

//            meshObject = new GameObject("Terrain Chunk");
//            meshRenderer = meshObject.AddComponent<MeshRenderer>();
//            meshFilter = meshObject.AddComponent<MeshFilter>();
//            meshRenderer.material = material;

//            meshObject.transform.position = positionv3 * Scale;
//            meshObject.transform.parent = Parent;
//            meshObject.transform.localScale = Vector3.one * Scale;
//            setVisible(false);

//            lodMeshes = new LODMesh[detailLevels.Length];
//            for (int i = 0; i < detailLevels.Length; i++)
//            {
//                lodMeshes[i] = new LODMesh(detailLevels[i].lod,UpdateTerrainChunk);
//            }

//            mapGenerator.RequestMapData(position, OnMapDataReceived);
//        }


//        void OnMapDataReceived(MapData mapData)
//        {
//            this.mapData = mapData;
//            mapDataReceived = true;

//            Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colourMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
//            meshRenderer.material.mainTexture = texture;

//            UpdateTerrainChunk();
//        }

//        void OnMeshDataReceived(MeshData meshData)
//        {
//            meshFilter.mesh = meshData.CreateMesh();
//        }

//        public void UpdateTerrainChunk()
//        {
//            if (mapDataReceived)
//            {
//                float viewerDistFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(ViewerPosition));
//                bool visible = viewerDistFromNearestEdge <= MaxViewDst;

//                if (visible)
//                {
//                    int lodindex = 0;
//                    for (int i = 0; i < detailLevels.Length; i++)
//                    {
//                        if (viewerDistFromNearestEdge > detailLevels[i].visibleDstThreshold)
//                        {
//                            lodindex = i + 1;
//                        }
//                        else
//                        {
//                            break;
//                        }
//                    }
//                    if (lodindex != previousLODIndex)
//                    {
//                        LODMesh lODMesh = lodMeshes[lodindex];
//                        if (lODMesh.hasMesh)
//                        {
//                            previousLODIndex = lodindex;
//                            meshFilter.mesh = lODMesh.mesh;
//                        }
//                        else if (!lODMesh.hasRequestedMesh)
//                        {
//                            lODMesh.RequestMesh(mapData);
//                        }
//                    }

//                    terrainChunksVisibleLastUpdate.Add(this);

//                }

//                setVisible(visible);
//            }

//        }
//        public void setVisible(bool visible)
//        {
//            meshObject.SetActive(visible);
//        }

//        public bool IsVisible()
//        {
//            return meshObject.activeSelf;
//        }
//    }


//    class LODMesh
//    {
//        public Mesh mesh;
//        public bool hasRequestedMesh;
//        public bool hasMesh;
//        int lod;
//        Action updateCallBack;

//        public LODMesh(int lod, Action updateCallBack)
//        {
//            this.lod = lod;
//            this.updateCallBack = updateCallBack;
//        }

//        void OnMeshDataRecieved(MeshData meshData)
//        {
//            mesh = meshData.CreateMesh();
//            hasMesh = true;

//            updateCallBack();
//        }

//        public void RequestMesh(MapData mapData)
//        {
//            hasRequestedMesh = true;
//            mapGenerator.RequestMeshData(mapData,lod, OnMeshDataRecieved);
                
//        }        
//    }

//    [Serializable]
//    public struct LODInfo
//    {
//        public int lod;
//        public float visibleDstThreshold;
//    }

//}
