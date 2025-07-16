using UnityEngine;

public static class Noise
{
    public static float[, ] GenerateNoiseMap(int _width, int _height, int _seed, float _scale, int _octaves, float _persistance, float _lacunarity, Vector2 _offset)
    {
        float[, ] _noiseMap = new float[_width, _height];

        System.Random _prng = new System.Random(_seed);
        Vector2[] _octaveOffsets = new Vector2[_octaves];

        for(int i = 0; i < _octaves; i++)
        {
            float _offsetX = _prng.Next(-100000, 100000) + _offset.x;
            float _offsetY = _prng.Next(-100000, 100000) + _offset.y;
            _octaveOffsets[i] = new Vector2(_offsetX, _offsetY);
        }

        float _maxNoiseHeight = float.MinValue;
        float _minNoiseHeight = float.MaxValue;

        float _halfWidth = _width / 2f;
        float _halfHeight = _height / 2f;

        for (int y = 0; y < _height; y++)
        {
            for(int x = 0; x < _width; x++)
            {
                float _amplitude = 1;
                float _frequency = 1;
                float _noiseHeight = 0;

                for (int i = 0; i < _octaves; i++)
                {
                    float _sampleX = ((x - _halfWidth) / _scale - _octaveOffsets[i].x) * _frequency;
                    float _sampleY = ((y - _halfHeight) / _scale - _octaveOffsets[i].y) * _frequency;

                    float _noiseValue = Mathf.PerlinNoise(_sampleX, _sampleY) * 2 - 1;
                    _noiseHeight += _noiseValue * _amplitude;

                    _amplitude *= _persistance;
                    _frequency *= _lacunarity;
                }

                if(_noiseHeight > _maxNoiseHeight)
                    _maxNoiseHeight = _noiseHeight;
                else if(_noiseHeight < _minNoiseHeight)
                    _minNoiseHeight = _noiseHeight;

                _noiseMap[x, y] = Mathf.InverseLerp(_minNoiseHeight, _maxNoiseHeight, _noiseHeight);
            }
        }

        return _noiseMap;
    }
}
