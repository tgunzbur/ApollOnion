using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnvironmentType
{
    STONE,
    FIELD,
    GRASS,
    SAND,
};

public class Tile : MonoBehaviour
{
    public Vector2Int       Coords  { get; set; }
    public Machine          Machine { get; set; }

    private EnvironmentType type;
    public EnvironmentType Type {
        get { return type; }
        set {
            type = value;
        }
    }

    private List<Mat>   buffer;

    public Tile[] getSurroundings() {
        return GameManager.TileMap.getSurroundings(this);
    }

    void Start() {
        if (buffer == null) {
            buffer = new List<Mat>();
        }
    }

    void FixedUpdate() {
        if (Machine != null) {
            foreach (Mat mat in buffer) {
                Machine.addToInput(mat);
            }
            buffer.Clear();
        } else {
            List<Mat> toRemove = new List<Mat>();
            foreach (Mat mat in buffer) {
                if (mat && !mat.gameObject.activeSelf) {
                    Destroy(mat.gameObject);
                    toRemove.Add(mat);
                }
            }
            foreach (Mat mat in toRemove) {
                buffer.Remove(mat);
            }
        }
    }

    public void catchMat(Mat mat) {
        buffer.Add(mat);
    }

    public void clearBuffer() {
        foreach (Mat mat in buffer) {
            GameObject.Destroy(mat.gameObject);
        }
    }

    public List<Mat> getBuffer() {
        return buffer;
    }

    public void setBuffer(MatType[] newBuffer) {
        buffer = new List<Mat>();
        if (newBuffer != null) {
            foreach (MatType mat in newBuffer) {
                GameObject prefab = GameManager.GetMatPrefab(mat).GetComponent<DragAndDropMat>().matPrefab;
                Mat newMat = Instantiate(prefab, transform.position, prefab.transform.rotation).GetComponent<Mat>();
                newMat.name = prefab.name;
                newMat.gameObject.SetActive(false);
                buffer.Add(newMat);
            }
        }
    }
}
