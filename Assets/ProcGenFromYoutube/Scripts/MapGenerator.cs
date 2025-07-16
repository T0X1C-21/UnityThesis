using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct TerrainType
    {
        public string _name;
        public float _height;
        public Color _color;
    }

    public enum DrawMode
    {
        NoiseMap,
        ColorMap,
        Mesh
    }
    [SerializeField] private DrawMode _drawMode;

    public const int _mapChunkSize = 241;
    [Range(0, 6)] [SerializeField] private int _levelOfDetail;
    [SerializeField] private float _noiseScale;

    [SerializeField] private int _octaves;
    [Range(0, 1)] [SerializeField] private float _persistance;
    [SerializeField] private float _lacunarity;

    [SerializeField] private int _seed;
    [SerializeField] private Vector2 _offset;

    [SerializeField] private float _heightMulitplier;
    [SerializeField] private AnimationCurve _meshHeightCurve;

    public bool _autoUpdate;

    [SerializeField] private TerrainType[] _regions;

    public void GenerateMap()
    {
        float[, ] _noiseMap = Noise.GenerateNoiseMap(_mapChunkSize, _mapChunkSize, _seed, _noiseScale, _octaves, _persistance, _lacunarity, _offset);

        Color[] _colorMap = new Color[_mapChunkSize * _mapChunkSize];
        for(int y = 0; y < _mapChunkSize; y++)
        {
            for(int x = 0; x < _mapChunkSize; x++)
            {
                float _currentHeight = _noiseMap[x, y];

                for(int i = 0; i < _regions.Length; i++)
                {
                    if(_currentHeight <= _regions[i]._height)
                    {
                        _colorMap[y * _mapChunkSize + x] = _regions[i]._color;
                        break;
                    }
                }
            }
        }

        MapDisplay _mapDisplay = FindFirstObjectByType<MapDisplay>();
        if(_drawMode == DrawMode.NoiseMap)
            _mapDisplay.DrawTexture(TextureGenerator.TextureFromNoiseMap(_noiseMap));
        else if(_drawMode == DrawMode.ColorMap)
            _mapDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(_colorMap, _mapChunkSize, _mapChunkSize));
        else if(_drawMode == DrawMode.Mesh)
            _mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(_noiseMap, _heightMulitplier, _meshHeightCurve, _levelOfDetail), TextureGenerator.TextureFromColorMap(_colorMap, _mapChunkSize, _mapChunkSize));
    }

    private void OnValidate()
    {
        if (_noiseScale <= 0)
            _noiseScale = 0.01f;

        if(_lacunarity < 1)
            _lacunarity = 1;

        if(_octaves < 0)
            _octaves = 0;
    }
}
