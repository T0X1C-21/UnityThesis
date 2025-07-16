using UnityEngine;
using System.Collections.Generic;

public class EndlessTerrain : MonoBehaviour
{
    [SerializeField] private Transform _viewer;
    [SerializeField] private Material _material;
    public const float _maxViewDistance = 450;
    public static Vector2 _viewerPosition;

    private static MapGenerator _mapGenerator;

    private int _chunkSize;
    private int _chunksVisibleInViewDistance;

    private Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    private List<TerrainChunk> _terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    private void Start()
    {
        _mapGenerator = FindFirstObjectByType<MapGenerator>();
        _chunkSize = MapGenerator._mapChunkSize - 1;
        _chunksVisibleInViewDistance = Mathf.RoundToInt(_maxViewDistance / _chunkSize);
    }

    private void Update()
    {
        _viewerPosition = new Vector2(_viewer.position.x, _viewer.position.z);
        UpdateVisibleChunks();
    }

    private void UpdateVisibleChunks()
    {
        for(int i = 0; i < _terrainChunksVisibleLastUpdate.Count; i++)
        {
            _terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        _terrainChunksVisibleLastUpdate.Clear();

        int _currentChunkCoordX = Mathf.RoundToInt(_viewerPosition.x / _chunkSize);
        int _currentChunkCoordY = Mathf.RoundToInt(_viewerPosition.y / _chunkSize);

        for(int yOffset = -_chunksVisibleInViewDistance; yOffset <= _chunksVisibleInViewDistance; yOffset++)
        {
            for(int xOffset = -_chunksVisibleInViewDistance; xOffset <= _chunksVisibleInViewDistance; xOffset++)
            {
                Vector2 _viewedChunkCoord = new Vector2(_currentChunkCoordX + xOffset, _currentChunkCoordY + yOffset);

                if(_terrainChunkDictionary.ContainsKey(_viewedChunkCoord))
                {
                    _terrainChunkDictionary[_viewedChunkCoord].UpdateTerrainChunk();
                    if(_terrainChunkDictionary[_viewedChunkCoord].IsVisible())
                    {
                        _terrainChunksVisibleLastUpdate.Add(_terrainChunkDictionary[_viewedChunkCoord]);
                    }
                }
                else
                {
                    _terrainChunkDictionary.Add(_viewedChunkCoord, new TerrainChunk(_viewedChunkCoord, _chunkSize, transform, _material));
                }
            }
        }
    }

    public class TerrainChunk
    {
        GameObject _meshObject;
        Vector2 _position;
        Bounds _bounds;

        MeshRenderer _meshRenderer;
        MeshFilter _meshFilter;

        public TerrainChunk(Vector2 _coord, int _size, Transform _parent, Material _material)
        {
            _position = _coord * _size;
            _bounds = new Bounds(_position, Vector2.one * _size);
            Vector3 _positionV3 = new Vector3(_position.x, 0, _position.y);

            _meshObject = new GameObject("Terrain Chunk");
            _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
            _meshFilter = _meshObject.AddComponent<MeshFilter>();
            _meshRenderer.material = _material;

            _meshObject.transform.position = _positionV3;
            _meshObject.transform.parent = _parent;
            SetVisible(false);

            _mapGenerator.RequestMapData(OnMapDataReceived);
        }

        public void OnMapDataReceived(MapData _mapData)
        {
            _mapGenerator.RequestMeshData(_mapData, OnMeshDataReceived);
        }

        public void OnMeshDataReceived(MeshData _meshData)
        {
            _meshFilter.mesh = _meshData.CreateMesh();
        }

        public void UpdateTerrainChunk()
        {
            float _viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(_viewerPosition));
            bool _isVisible = _viewerDistanceFromNearestEdge <= _maxViewDistance;
            SetVisible(_isVisible);
        }

        public void SetVisible(bool _isVisible)
        {
            _meshObject.SetActive(_isVisible);
        }

        public bool IsVisible()
        {
            return _meshObject.activeSelf;
        }
    }
}
