using System.Runtime.CompilerServices;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[, ] _heightMap, float _heightMultiplier, AnimationCurve _meshHeightCurve, int _levelOfDetail)
    {
        int _width = _heightMap.GetLength(0);
        int _height = _heightMap.GetLength(1);
        float _topLeftX = (_width - 1) / -2f;
        float _topLeftZ = (_height - 1) / 2f;

        int _meshSimplificationIncrement = (_levelOfDetail == 0)? 1 : _levelOfDetail * 2;
        int _verticesPerLine = (_width - 1)/_meshSimplificationIncrement + 1;

        MeshData _meshData = new MeshData(_width, _height);
        int _vertexIndex = 0; 

        for (int y = 0; y < _height; y += _meshSimplificationIncrement)
        {
            for(int x = 0; x < _width; x += _meshSimplificationIncrement)
            {
                _meshData._vertices[_vertexIndex] = new Vector3(_topLeftX + x, _meshHeightCurve.Evaluate(_heightMap[x, y]) * _heightMultiplier, _topLeftZ - y);
                _meshData._uvs[_vertexIndex] = new Vector2(x / (float)_width, y / (float)_height);

                if (x < _width - 1 && y < _height - 1)
                {
                    _meshData.AddTriangle(_vertexIndex, _vertexIndex + _verticesPerLine + 1, _vertexIndex + _verticesPerLine);
                    _meshData.AddTriangle(_vertexIndex + _verticesPerLine + 1, _vertexIndex, _vertexIndex + 1);
                }

                _vertexIndex++;
            }
        }

        return _meshData;
    }
}

public class MeshData
{
    public Vector3[] _vertices;
    public int[] _triangles;
    public Vector2[] _uvs;

    private int _triangleIndex;

    public MeshData(int _meshWidth, int _meshHeight)
    {
        _vertices = new Vector3[_meshWidth * _meshHeight];
        _triangles = new int[(_meshWidth - 1) * (_meshHeight - 1) * 6];
        _uvs = new Vector2[_meshWidth * _meshHeight];
    }

    public void AddTriangle(int _a, int _b, int _c)
    {
        _triangles[_triangleIndex] = _a;
        _triangles[_triangleIndex + 1] = _b;
        _triangles[_triangleIndex + 2] = _c;

        _triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh _mesh = new Mesh();
        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;
        _mesh.uv = _uvs;
        _mesh.RecalculateNormals();
        return _mesh;
    }
}