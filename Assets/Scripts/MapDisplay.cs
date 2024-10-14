using UnityEngine;
using System.Collections;

public class MapDisplay : MonoBehaviour
{
    public Texture2D MapTexture;
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshData meshData;
    public Color[] ColourMap;

    public void DrawTexture(Texture2D texture)
    {
        //textureRender.sharedMaterial.mainTexture = texture;
        //textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
        MapTexture = texture;
        
    }

    public void DrawMesh(GameObject MeshObj, MeshData meshData, Texture2D texture)
    {
        meshFilter = MeshObj.GetComponent<MeshFilter>();
        meshRenderer = MeshObj.GetComponent<MeshRenderer>();
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.material.mainTexture = texture;
    }

}