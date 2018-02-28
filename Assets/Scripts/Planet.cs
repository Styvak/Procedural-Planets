using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlanetSettings {
    public int chunkSize;
    public int chunkResolution;
    public int height;
    public int radius;
    public int worldHeight;
    public float surfaceCrossValue;
}

[System.Serializable]
public struct PlanetNoiseSettings {
    public int octaves;
}

public class Planet : MonoBehaviour {

    [SerializeField] private PlanetSettings _planetSettings;
    [SerializeField] private PlanetNoiseSettings _noiseSettings;

    private GameObject[] _faces;
    private PlanetChunk[,,] _chunks;

    public void Build() {
        int nbChunk = _planetSettings.radius / _planetSettings.chunkSize;
        _chunks = new PlanetChunk[nbChunk, nbChunk, nbChunk];

        if (_faces != null) {
            foreach (var face in _faces)
                DestroyImmediate(face);
        }
        _faces = new GameObject[6];

        for (int i = 0; i < 6; i++) {
            _faces[i] = new GameObject("Faces #" + i);
            _faces[i].transform.parent = transform;

            for (int chunkIdx = 0; chunkIdx < nbChunk * nbChunk; chunkIdx++) {
                var x = chunkIdx / nbChunk;
                var z = chunkIdx % nbChunk;
                var chunk = new GameObject("Chunk " + x + " " + z, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider), typeof(PlanetChunk));
                chunk.transform.parent = _faces[i].transform;
                chunk.transform.position = new Vector3(transform.position.x + x * _planetSettings.chunkSize, 0, transform.position.z + z * _planetSettings.chunkSize);

                int y = 0;
                switch (i) {
                    case 0:
                        _chunks[x, 0, z] = chunk.GetComponent<PlanetChunk>();
                        break;
                    case 1:
                        _chunks[nbChunk - 1, x, z] = chunk.GetComponent<PlanetChunk>();
                        x = nbChunk;
                        y = x;
                        break;
                    case 2:
                        _chunks[0, x, z] = chunk.GetComponent<PlanetChunk>();
                        x = 0;
                        y = x;
                        break;
                    case 3:
                        _chunks[x, z, nbChunk - 1] = chunk.GetComponent<PlanetChunk>();
                        y = z;
                        z = nbChunk;
                        break;
                    case 4:
                        _chunks[x, z, nbChunk - 1] = chunk.GetComponent<PlanetChunk>();
                        y = z;
                        z = -nbChunk;
                        break;
                    case 5:
                        _chunks[x, nbChunk - 1, z] = chunk.GetComponent<PlanetChunk>();
                        y = nbChunk;
                        break;
                }

                chunk.GetComponent<PlanetChunk>().GenerateCube(_planetSettings, _noiseSettings, this, x, z, y);
            }
        }
        int size = nbChunk * _planetSettings.chunkSize;
        _faces[1].transform.localPosition = new Vector3(0, -size, 0);
        _faces[1].transform.eulerAngles = new Vector3(-90, 0, 0);
        _faces[2].transform.localPosition = new Vector3(size, 0, 0);
        _faces[2].transform.eulerAngles = new Vector3(90, 0, 0);
        _faces[3].transform.localPosition = new Vector3(0, -size, 0);
        _faces[3].transform.eulerAngles = new Vector3(0, 0, -90);
        _faces[4].transform.localPosition = new Vector3(0, 0, size);
        _faces[4].transform.eulerAngles = new Vector3(0, 0, 90);
        _faces[5].transform.localPosition = new Vector3(0, -size, 0);
        _faces[5].transform.eulerAngles = new Vector3(180, 0, 0);
    }

    public float GetValue(int x, int y, int z)
    {
        int chunkX = (int)(x / _planetSettings.chunkSize);
        int chunkZ = (int)(z / _planetSettings.chunkSize);
        int chunkY = (int)(y / _planetSettings.chunkSize);

        if (chunkX >= 0 && chunkX < _chunks.GetLength(0) &&
            y >= 0 && y < _chunks.GetLength(1) &&
            chunkZ >= 0 && chunkZ < _chunks.GetLength(2))
        {
            return _chunks[chunkX, chunkY, chunkZ].GetValue(
                (int)(x) % _planetSettings.chunkSize,
                (int)(y) % _planetSettings.chunkSize,
                (int)(z) % _planetSettings.chunkSize);
        }
        else
        {
            float height = Noise.Value(x, y, z, _noiseSettings);
            height *= _planetSettings.worldHeight;
            return height;
        }
    }
}
