using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [System.Serializable]
    public enum DrawMode { NoiseMap, ColorMap, Mesh };
    [SerializeField] private DrawMode _drawMode;

    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private float _scale;
    [SerializeField] private int _octaves;
    [Range(0,1)]
    [SerializeField] private float _persistance;
    [SerializeField] private float _lacunarity;
    [SerializeField] private int _seed;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private float _meshHeightMultiplier;
    [SerializeField] private AnimationCurve _meshHeightCurve;

    [SerializeField] private TerrainType[] _regions;

    [field: SerializeField]
    public bool AutoUpdate {
        get;
        private set;
    }

    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(_width, _height, _seed, _scale, _octaves, _persistance, _lacunarity, _offset);

        Color[] colorMap = new Color[_width * _height];

        for (int y = 0; y < _height; y++) {
            for (int x = 0; x < _width; x++) {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < _regions.Length; i++) {
                    if (currentHeight <= _regions[i].height) {
                        colorMap[y * _width + x] = _regions[i].color;
                        break;
                    }
                }
            }
        }
        MapDisplay mapDisplay = MapDisplay.Instance;
        if (_drawMode == DrawMode.NoiseMap) {
            mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (_drawMode == DrawMode.ColorMap) {
            mapDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, _width, _height));
        }
        else if (_drawMode == DrawMode.Mesh) {
            mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, _meshHeightMultiplier, _meshHeightCurve), TextureGenerator.TextureFromColorMap(colorMap, _width, _height));
        }
    }
    private void OnValidate() {
        if (_width < 1) {
            _width = 1;
        }
        if (_height < 1) {
            _height = 1;
        }
        if (_lacunarity < 1) {
            _lacunarity = 1;
        }
        if (_octaves < 0) {
            _octaves = 0;
        }

    }
}
[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
}
