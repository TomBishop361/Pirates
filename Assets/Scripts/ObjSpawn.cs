using System.Collections;
using System.Collections.Generic;
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


    public void SpawnObjs(TerrainType[] regions, MapDisplay map)
    {
        Terrain = map.meshRenderer.gameObject;
        SpawnBeachObjs(regions[1].vertsInRegion);

        SpawnGrassObjs(regions[2].vertsInRegion);

        SpawnWoodsObjs(regions[3].vertsInRegion);
    }

    private void SpawnBeachObjs(List<Vector3> vertsInRegion)
    {
        
        for (int i = 0; i < beachDens; i++)
        {            
            GameObject NewBeachObj = Instantiate(beachObjs[Random.Range(0, beachObjs.Count)], Terrain.transform, false);  //,
            NewBeachObj.transform.localPosition = vertsInRegion[Random.Range(0, vertsInRegion.Count)];
            NewBeachObj.transform.localScale = Vector3.one * 0.5f;
            NewBeachObj.transform.localEulerAngles = new Vector3(0, Random.Range(-360, 360), 0);
        }
    }

    private void SpawnGrassObjs(List<Vector3> vertsInRegion)
    {

        for (int i = 0; i < GrassDens; i++)
        {
            GameObject NewGrassObj = Instantiate(GrassObjs[Random.Range(0, GrassObjs.Count)], Terrain.transform, false);  //,
            NewGrassObj.transform.localPosition = vertsInRegion[Random.Range(0, vertsInRegion.Count)];
            NewGrassObj.transform.localScale = Vector3.one * 0.5f;
            NewGrassObj.transform.localEulerAngles = new Vector3(0, Random.Range(-360, 360), 0);
        }
    }


    private void SpawnWoodsObjs(List<Vector3> vertsInRegion)
    {

        for (int i = 0; i < WoodsDens; i++)
        {
            GameObject NewWoodsObj = Instantiate(WoodsObjs[Random.Range(0, WoodsObjs.Count)], Terrain.transform, false);  //,
            NewWoodsObj.transform.localPosition = vertsInRegion[Random.Range(0, vertsInRegion.Count)];
            NewWoodsObj.transform.localScale = Vector3.one * 0.5f;
            NewWoodsObj.transform.localEulerAngles = new Vector3(0, Random.Range(-360, 360), 0);
        }
    }



}
