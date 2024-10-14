using UnityEngine;
using System.Collections;

public static class TextureGenerator
{

    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }


    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColourMap(colourMap, width, height);
    }

    public static Texture2D EditTexture(MeshData meshdata, Vector3 digSpot, Color[] colourmap)
    {
        int indx = 0;        
        for (int i = 0; i < meshdata.vertices.Length-1; i++)
        {            
            if (meshdata.vertices[i] == digSpot)
            {                
                indx = i;                
            }
        }
        //Create Cross
        colourmap[indx] = Color.red;
        colourmap[indx + 242] = Color.red;
        colourmap[indx + 240] = Color.red;
        colourmap[indx - 242] = Color.red;
        colourmap[indx - 240] = Color.red;

        Texture2D newMap = TextureFromColourMap(colourmap, 241, 241);
        return newMap;
    }

}