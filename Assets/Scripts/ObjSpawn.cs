using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjSpawn : MonoBehaviour
{
    

    public List<GameObject> beachObjs;
    public int beachDens;

    public List<GameObject> GrassObjs;
    public int GrassDens;

    public List<GameObject> WoodsObjs;
    public int WoodsDens;

    GameObject Terrain;

    public List<GameObject> rocksObjs;
    public int rocksDens;

    public GameObject DigSpot;
    public const int DigSpotDens = 1;
    List<Vector3> unusedVerts = new List<Vector3>();
    Vector3[] FreeVerts;
    public void SpawnObjs(TerrainType[] regions, MapDisplay map)
    {

        Terrain = map.meshRenderer.gameObject;
        unusedVerts = new List<Vector3>();

        unusedVerts.AddRange(SpawnObjs(regions[1].vertsInRegion,beachDens,beachObjs));

        unusedVerts.AddRange(SpawnObjs(regions[2].vertsInRegion,GrassDens,GrassObjs));

        unusedVerts.AddRange(SpawnObjs(regions[3].vertsInRegion,WoodsDens,WoodsObjs));      

        
    }

    private List<Vector3> SpawnObjs(List<Vector3> vertsInRegion, int objDense, List<GameObject> spawnobjs)
    {

        for (int i = 0; i < objDense; i++)
        {
            GameObject NewBeachObj = Instantiate(spawnobjs[Random.Range(0, spawnobjs.Count)], Terrain.transform, false);  //,
            int random = Random.Range(0, vertsInRegion.Count);
            NewBeachObj.transform.localPosition = vertsInRegion[random];
            NewBeachObj.transform.localScale = Vector3.one * 0.5f;
            NewBeachObj.transform.localEulerAngles = new Vector3(0, Random.Range(-360, 360), 0);
            vertsInRegion.Remove(vertsInRegion[random]);
        }
        return vertsInRegion;

    }

    //spawn in a digspot, Store what vert it spawned at so that i can add to texture map
    private void SpawnDigSpot(TerrainType[] regions)
    {
        Vector3[] Verts = new Vector3[regions[1].vertsInRegion.Count+ regions[2].vertsInRegion.Count + regions[3].vertsInRegion.Count];
    }
}
