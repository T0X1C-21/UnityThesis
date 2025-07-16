using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static MapGenerator;

public enum DrawMode
{
    NoiseMap,
    ColorMap,
    Mesh
}

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private DrawMode _drawMode;

    public const int _mapChunkSize = 241;
    [Range(0, 6)] [SerializeField] private int _levelOfDetail;
    [SerializeField] private float _noiseScale;

    [SerializeField] private int _octaves;
    [Range(0, 1)] [SerializeField] private float _persistance;
    [SerializeField] private float _lacunarity;

    [SerializeField] private int _seed;
    [SerializeField] private Vector2 _offset;

    [SerializeField] private float _heightMultiplier;
    [SerializeField] private AnimationCurve _meshHeightCurve;

    public bool _autoUpdate;

    private Queue<MapThreadInfo<MapData>> _mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    private Queue<MapThreadInfo<MeshData>> _meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    [SerializeField] private TerrainType[] _regions;

    public void DrawMapInEditor()
    {
        MapData _mapData = GenerateMapData();

        MapDisplay _mapDisplay = FindFirstObjectByType<MapDisplay>();
        if (_drawMode == DrawMode.NoiseMap)
            _mapDisplay.DrawTexture(TextureGenerator.TextureFromNoiseMap(_mapData._heightMap));
        else if (_drawMode == DrawMode.ColorMap)
            _mapDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(_mapData._colorMap, _mapChunkSize, _mapChunkSize));
        else if (_drawMode == DrawMode.Mesh)
            _mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(_mapData._heightMap, _heightMultiplier, _meshHeightCurve, _levelOfDetail), TextureGenerator.TextureFromColorMap(_mapData._colorMap, _mapChunkSize, _mapChunkSize));
    }

    public void RequestMapData(Action<MapData> _callback)
    {
        ThreadStart _threadStart = delegate
        {
            MapDataThread(_callback);
        };

        new Thread(_threadStart).Start();
    }

    public void MapDataThread(Action<MapData> _callback)
    {
        MapData _mapData = GenerateMapData();
        
        lock(_mapDataThreadInfoQueue)
            _mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(_callback, _mapData));
    }

    public void RequestMeshData(MapData _mapData, Action<MeshData> _callback)
    {
        ThreadStart _threadStart = delegate
        {
            MeshDataThread(_mapData, _callback);
        };
        new Thread(_threadStart).Start();
    }

    public void MeshDataThread(MapData _mapData, Action<MeshData> _callback)
    {
        MeshData _meshData = MeshGenerator.GenerateTerrainMesh(_mapData._heightMap, _heightMultiplier, _meshHeightCurve, _levelOfDetail);

        lock(_meshDataThreadInfoQueue)
            _meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(_callback, _meshData));
    }

    private void Update()
    {
        if(_mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < _mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> _threadInfo = _mapDataThreadInfoQueue.Dequeue();
                _threadInfo._callback(_threadInfo._parameter);
            }
        }

        if (_meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < _meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> _threadInfo = _meshDataThreadInfoQueue.Dequeue();
                _threadInfo._callback(_threadInfo._parameter);
            }
        }
    }

    private MapData GenerateMapData()
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

        return new MapData(_noiseMap, _colorMap);
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

    public struct MapThreadInfo<T>
    {
        public readonly Action<T> _callback;
        public readonly T _parameter;

        public MapThreadInfo(Action<T> _callback, T _parameter)
        {
            this._callback = _callback;
            this._parameter = _parameter;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string _name;
    public float _height;
    public Color _color;
}

public struct MapData
{
    public readonly float[,] _heightMap;
    public readonly Color[] _colorMap;

    public MapData(float[,] _heightMap, Color[] _colorMap)
    {
        this._heightMap = _heightMap;
        this._colorMap = _colorMap;
    }
}
