using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    [SerializeField] private Renderer _textureRenderer;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;

    public void DrawTexture(Texture2D _texture)
    {
        _textureRenderer.sharedMaterial.mainTexture = _texture;
        _textureRenderer.transform.localScale = new Vector3(_texture.width, 1, _texture.height);
    }

    public void DrawMesh(MeshData _meshData, Texture2D _texture)
    {
        _meshFilter.sharedMesh = _meshData.CreateMesh();
        _meshRenderer.sharedMaterial.mainTexture = _texture;
    }
}
