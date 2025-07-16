using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[] _colorMap, int _mapWidth, int _mapHeight)
    {
        Texture2D texture = new Texture2D(_mapWidth, _mapHeight);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(_colorMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromNoiseMap(float[,] _noiseMap)
    {
        int _mapWidth = _noiseMap.GetLength(0);
        int _mapHeight = _noiseMap.GetLength(1);

        Color[] _colorMap = new Color[_mapWidth * _mapHeight];

        for (int y = 0; y < _mapHeight; y++)
        {
            for (int x = 0; x < _mapWidth; x++)
                _colorMap[y * _mapWidth + x] = Color.Lerp(Color.black, Color.white, _noiseMap[x, y]);
        }

        return TextureFromColorMap(_colorMap, _mapWidth, _mapHeight);
    }
}
