using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTileMap : MonoBehaviour {
    public GameObject tilePrefab;
    public Material   grassMaterial;
    public Material   fieldMaterial;
    public Material   rockMaterial;
    public Material   sandMaterial;

    public int          nbTiles;
    public float        zoom;
    public float        waterDeepness;

    private Tile[,]     grid;
    private Camera[]    cameras = null;
    private Vector2     noseOffset;
    private Tile        center;

    private Vector2Int[] surroundings = { 
        new Vector2Int(-1, +1),
        new Vector2Int( 0, +1),
        new Vector2Int(+1,  0),
        new Vector2Int(+1, -1),
        new Vector2Int( 0, -1),
        new Vector2Int(-1,  0)};

    public void createNewMap() {
        clearMap();
        generateIsland();
        moveCamera();
        setEnvironment();
    }

    private void setEnvironment() {
        for (int i = 0; i < nbTiles; i++) {
            for (int j = 0; j < nbTiles; j++) {
                Tile tile = grid[i, j];
                if (tile != null && isCoast(tile) && tile.transform.localScale.y - waterDeepness < 1.8f) {
                    tile.Type = EnvironmentType.SAND;
                    tile.GetComponent<Renderer>().material = sandMaterial;
                }
            }
        }
    }

    public void loadMap(List<TileInfo> tiles) {
        center = null;

        clearMap();
        grid = new Tile[tiles.Count, tiles.Count];
        foreach (TileInfo tile in tiles) {
            tile.height -= waterDeepness;
            Tile newTile = createTile(tile.x, tile.y, tile.height, tile.type, tile.buffer);
            if (!center) {
                center = newTile;
            }
            if (tile.machine != null && tile.machine.machineType != "") {
                if (tile.machine.machineType.EndsWith("_launchable")) {
                    tile.machine.machineType = tile.machine.machineType.Substring(0, tile.machine.machineType.Length - "_launchable".Length);
                    newTile.Machine = createMachine(GameManager.getMachinePrefab(tile.machine.machineType), tile, newTile, 2);
                    
                } else if (tile.machine.machineType.EndsWith("_rocket")) {
                    tile.machine.machineType = tile.machine.machineType.Substring(0, tile.machine.machineType.Length - "_rocket".Length);
                    newTile.Machine = createMachine(GameManager.getMachinePrefab(tile.machine.machineType), tile, newTile, 1);
                    
                } else {
                    newTile.Machine = createMachine(GameManager.getMachinePrefab(tile.machine.machineType), tile, newTile);
                }
            }
        }
        moveCamera();
    }

    public void clearMap() {
        foreach (Transform tile in transform) {
            if (tile.GetComponent<Tile>().Machine) {
                tile.GetComponent<Tile>().Machine.destroyMachine();
            }
            tile.GetComponent<Tile>().clearBuffer();
            GameObject.Destroy(tile.gameObject);
        }
        foreach (Transform mat in GameManager.matsContainer) {
            GameObject.Destroy(mat.gameObject);
        }
        GameManager.Infos.clear();
        gameObject.transform.position = Vector3.zero;
    }

    private void generateIsland() {
        float prcNewTile = 0.5f;
        noseOffset = new Vector2(UnityEngine.Random.value * nbTiles, UnityEngine.Random.value * nbTiles);
        grid = new Tile[nbTiles, nbTiles];
        center = createTile(nbTiles / 2, nbTiles / 2);
        List<Tile> tiles = new List<Tile>() { center };
        int tileCount = 1;

        List<Tile> newTiles = new List<Tile>();
        List<Tile> toRemove = new List<Tile>();
        while (tileCount < nbTiles) {
            newTiles.Clear();
            toRemove.Clear();
            foreach (Tile tile in tiles) {
                bool isCoast = false;
                foreach (Vector2Int surrounding in surroundings) {
                    Vector2Int coord = tile.Coords + surrounding;
                    if (getAtCoord(coord) == null) {
                        if (tileCount < nbTiles && UnityEngine.Random.value >= prcNewTile) {
                            Tile newTile = createTile(coord);
                            newTiles.Add(newTile);
                            tileCount++;
                        } else {
                            isCoast = true;
                        }
                    }
                }
                if (!isCoast) {
                    toRemove.Add(tile);
                }
            }
            tiles.AddRange(newTiles);
            bool needRemove(Tile tile) { return toRemove.Contains(tile); }
            tiles.RemoveAll(needRemove);
        }
    }

    private bool isCoast(Tile tile) {
        foreach (Tile surrounding in getSurroundings(tile)) {
            if (surrounding == null) {
                return true;
            }
        }
        return false;
    }

    public Tile[] getSurroundings(Tile tile) {
        Vector2Int coords = new Vector2Int(tile.Coords.x, tile.Coords.y);
        Tile[] surr = new Tile[6];
        for (int i = 0; i < 6; i++) {
            surr[i] = getAtCoord(coords + surroundings[i]);
        }
        return surr;
    }

    public Tile getAtCoord(Vector2Int vec) { return getAtCoord(vec.x, vec.y); }

    public Tile getAtCoord(int x, int y) {
        if (x >= 0 && y >= 0 && x < grid.GetLength(0) && y < grid.GetLength(1)) {
            return grid[x, y];
        } else {
            return null;
        }
    }

    private Tile createTile(Vector2Int c) {
        return createTile(c.x, c.y);
    }

    private Tile createTile(int x, int y) {
        float height = Mathf.PerlinNoise(noseOffset.x + x / zoom, noseOffset.y + y / zoom) * zoom / 2;
        EnvironmentType type = (EnvironmentType)(int)Mathf.Round(UnityEngine.Random.Range(0, 3));
        Tile myTile = createTile(x, y, height, type);
        return myTile;
    }

    private Tile createTile(int x, int y, float height, EnvironmentType type, MatType[] buffer = null) {
        Transform tilemapParent = this.gameObject.transform;
        Quaternion rotation = tilePrefab.transform.rotation;
        Vector3 position = new Vector3(x + (y * 0.5f), -waterDeepness / 2, y * (Mathf.Tan(Mathf.PI / 6) + 0.25f));
        Vector3 scale = Vector3.one;

        scale.y = height + waterDeepness;
        Tile newTile = Instantiate(tilePrefab, position, rotation, tilemapParent).GetComponent<Tile>();
        newTile.transform.localScale = scale;
        newTile.gameObject.name = "Tile" + x + "_" + y;
        newTile.Coords = new Vector2Int(x, y);
        newTile.Type = type;
        grid[x, y] = newTile;
        newTile.setBuffer(buffer);
        GameManager.Infos.addTile(newTile);

        switch (type) {
            case EnvironmentType.FIELD:
                newTile.GetComponent<Renderer>().material = fieldMaterial;
            break;
            case EnvironmentType.GRASS:
                newTile.GetComponent<Renderer>().material = grassMaterial;
            break;
            case EnvironmentType.STONE:
                newTile.GetComponent<Renderer>().material = rockMaterial;
            break;
            case EnvironmentType.SAND:
                newTile.GetComponent<Renderer>().material = sandMaterial;
            break;
        }
        return newTile;
    }

    private Machine createMachine(GameObject prefab, TileInfo infos, Tile tile, int isLaunchableSpacePort = 0) {
        if (!prefab) {
            return null;
        }
        GameObject machine = Instantiate(prefab,
        tile.transform.position + new Vector3(0, tile.transform.localScale.y / 2, 0),
        prefab.transform.rotation, GameManager.machinesContainer);

        machine.GetComponent<Machine>().loadInfos(infos.machine, tile);
        if (isLaunchableSpacePort == 1) {
            machine.GetComponent<SpacePort>().addRocket();
        } else if (isLaunchableSpacePort == 2) {
            machine.GetComponent<SpacePort>().addRocket();
            machine.GetComponent<SpacePort>().isLaunchable = true;
        }
        return machine.GetComponent<Machine>();
    }

    private void moveCamera() {
        if (cameras == null) {
            cameras = Camera.allCameras;
        }
        Tile down = center;
        Tile up = center;
        Tile left = center;
        Tile right = center;
        
        Vector3 centerPos = new Vector3(center.transform.position.x, 0, center.transform.position.z);
        gameObject.transform.position = -centerPos;
        foreach (Transform machine in GameManager.machinesContainer) {
            machine.position -= centerPos;
        }
        foreach (Tile tile in grid) {
            if (!tile) {
                continue;
            }
            if (tile.transform.position.x < left.transform.position.x) {
                left = tile;
            } if (tile.transform.position.z < down.transform.position.z) {
                down = tile;
            } if (tile.transform.position.x > right.transform.position.x) {
                right = tile;
            } if (tile.transform.position.z > up.transform.position.z) {
                up = tile;
            }
        }
        Vector3 waterDeepnessVec = new Vector3(0, waterDeepness, 0);
        Vector3 minX = new Vector3(left.transform.position.x, 1.5f, center.transform.position.z) - new Vector3(left.transform.localScale.x, 0, left.transform.localScale.z) / 2;
        Vector3 maxX = new Vector3(right.transform.position.x, 1.5f, center.transform.position.z) + new Vector3(right.transform.localScale.x, 0, right.transform.localScale.z) / 2;
        Vector3 minY = new Vector3(center.transform.position.x, 0.5f, down.transform.position.z) - new Vector3(down.transform.localScale.x, 0, down.transform.localScale.z) / 2;
        Vector3 maxY = new Vector3(center.transform.position.x, 1.5f, up.transform.position.z) + new Vector3(up.transform.localScale.x, 0, up.transform.localScale.z) / 2;

        cameras[0].transform.position = center.transform.position;
        
        StartCoroutine(moveCam(minX, maxX, minY, maxY));
    }

    private IEnumerator moveCam(Vector3 minX, Vector3 maxX, Vector3 minY, Vector3 maxY) {
        float step = 0.1f;

        Vector3 screenMinX, screenMaxX, screenMinY, screenMaxY;
        bool minXBool = false, minYBool = false, maxXBool = false, maxYBool = false;
        while (!(minXBool && minYBool && maxXBool && maxYBool)) {
            screenMinX = cameras[0].WorldToScreenPoint(minX);
            screenMaxX = cameras[0].WorldToScreenPoint(maxX);
            screenMinY = cameras[0].WorldToScreenPoint(minY);
            screenMaxY = cameras[0].WorldToScreenPoint(maxY);

            minXBool = isInGameScreen(screenMinX, cameras[0]);
            maxXBool = isInGameScreen(screenMaxX, cameras[0]);
            minYBool = isInGameScreen(screenMinY, cameras[0]);
            maxYBool = isInGameScreen(screenMaxY, cameras[0]);

            if (!minXBool && !maxXBool) {
                cameras[0].transform.position += new Vector3(0, step, -step);
            }
            if (!minYBool && !maxYBool) {
                cameras[0].transform.position += new Vector3(0, step, -step);
            }
            if (!minXBool && maxXBool) {
                cameras[0].transform.position += new Vector3(-step, step / 10, -step / 10);
            } else if (!maxXBool && minXBool) {
                cameras[0].transform.position += new Vector3(step, step / 10, -step / 10);
            }
            if (!minYBool && maxYBool) {
                cameras[0].transform.position += new Vector3(0, step / 10, -step + step / 10);
            } else if (!maxYBool && minYBool) {
                cameras[0].transform.position += new Vector3(0, step / 10, step + step / 10);
            }
        }
        foreach (Camera camera in cameras) {
            camera.transform.position = cameras[0].transform.position;
        }
        yield return null;
    }

    private bool isInGameScreen(Vector3 pos, Camera cam) {
        int border = 50;
        Vector2 ratio = new Vector2(cameras[0].pixelWidth / Screen.width, cameras[0].pixelHeight / Screen.height);
        if (pos.x < border * ratio.x) {
            return false;
        } else if (pos.y < (border + 175) * ratio.y) {
            return false;
        } else if (pos.x > cameras[0].pixelWidth - border * ratio.x) {
            return false;
        } else if (pos.y > cameras[0].pixelHeight - border * ratio.y) {
            return false;
        } else if (pos.z < 0) {
            return false;
        }
        return true;
    }
}
