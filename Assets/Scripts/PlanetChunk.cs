using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class PlanetChunk : MonoBehaviour {

    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;

    private PlanetSettings _planetSettings;
    private PlanetNoiseSettings _noiseSettings;

    private float[,,] _data;

    private int _chunkX;
    private int _chunkZ;
    private int _chunkY;

    public void GenerateCube(PlanetSettings planetSettings, PlanetNoiseSettings noiseSettings, Planet planet, int chunkX, int chunkZ, int chunkY) {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();

        _planetSettings = planetSettings;
        _noiseSettings = noiseSettings;
        _chunkX = chunkX;
        _chunkZ = chunkZ;
        _chunkY = chunkY;
        _data = new float[_planetSettings.chunkResolution, _planetSettings.worldHeight, _planetSettings.chunkResolution];

        float size = (float)_planetSettings.chunkSize / _planetSettings.chunkResolution;
        for (int x = 0; x < _planetSettings.chunkResolution; x++) {
            for (int z = 0; z < _planetSettings.chunkResolution; z++) {
                float posX = x * size;
                float posZ = z * size;
                float posY = 0;

                Vector3 sphereCoordinates = new Vector3(posX, posZ, posY) + transform.localPosition;
                sphereCoordinates.Normalize();
                Vector3 pos = sphereCoordinates * planetSettings.radius;

                float height = Noise.Value(pos.x, pos.y, pos.z, noiseSettings);
                height *= _planetSettings.worldHeight;

                for (int h = 0; h < (int)height; h++) {
                    _data[x, h, z] = 1;
                }
            }
        }
        GenerateMesh(_planetSettings.surfaceCrossValue, planet, _planetSettings.chunkResolution);
    }

    public void GenerateMesh(float surfaceCrossValue, Planet planet, int size)
    {
        Mesh mesh = new Mesh();
        WorldMeshGenerator.FillMesh(ref mesh, _chunkX, _chunkY, _chunkZ, planet, size, _planetSettings.worldHeight, surfaceCrossValue);
        _meshFilter.mesh = mesh;
        _meshCollider.sharedMesh = mesh;
    }

    public float GetValue(int x, int y, int z) {
        return _data[x, y, z];
    }
}
