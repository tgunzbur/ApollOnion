using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MachineInfo {
    public MatType[]        inputs;
    public MatType[]    outputsType;
    public string       machineType;
}

[System.Serializable]
public class TileInfo {
    public int x;
    public int y;
    public float height;
    public EnvironmentType type;
    public MatType[]  buffer;
    public MachineInfo  machine;

    public TileInfo() {
        x = 0;
        y = 0;
        height = 1;
        type = EnvironmentType.GRASS;
        machine = null;
    }
}

[System.Serializable]
public class SaveInfo {
    public int money;
    public float timeSpent;
    public List<TileInfo> tiles;

    public SaveInfo(GameInfo infos) {
        money = infos.Money;
        timeSpent = Time.time - infos.startingTime;
        tiles = new List<TileInfo>();
        foreach (Tile tile in infos.tiles) {
            TileInfo tileInfos = new TileInfo();
            tileInfos.x = tile.Coords.x;
            tileInfos.y = tile.Coords.y;
            tileInfos.height = tile.transform.localScale.y;
            tileInfos.type = tile.Type;

            List<Mat> bufferTmp = tile.getBuffer();
            tileInfos.buffer = new MatType[bufferTmp.Count];
            for (int count = 0; count < bufferTmp.Count; count++) {
                tileInfos.buffer[count] = bufferTmp[count].getMatType();
            }
            if (tile.Machine != null) {
                tileInfos.machine = new MachineInfo();
                tileInfos.machine.machineType = tile.Machine.getName();
                
                if (tile.Machine.GetComponent<SpacePort>() && tile.Machine.GetComponent<SpacePort>().isLaunchable) {
                    tileInfos.machine.machineType += "_launchable";
                } else if (tile.Machine.GetComponent<SpacePort>() && tile.Machine.GetComponent<SpacePort>().rocket) {
                    tileInfos.machine.machineType += "_rocket";
                }

                Mat[] inputsTmp = tile.Machine.getInputs().ToArray();
                tileInfos.machine.inputs = new MatType[inputsTmp.Length];
                for (int count = 0; count < inputsTmp.Length; count++) {
                    tileInfos.machine.inputs[count] = inputsTmp[count].getMatType();
                }

                tileInfos.machine.outputsType = tile.Machine.getOutputsType();
            } else {
                tileInfos.machine = null;
            }
            tiles.Add(tileInfos);
        }
    }

}

public class GameInfo {
    public float  startingTime;
    public string saveName;
    public int Money { get; set; }
    public List<Tile> tiles;

    public GameInfo(string name) {
        startingTime = Time.time;
        saveName = name;
        Money = 3000;
        tiles = new List<Tile>();
    }

    public void addTile(Tile tile) {
        tiles.Add(tile);
    }

    public void clear() {
        tiles.Clear();
    }
}

public class GameManager : MonoBehaviour {
    public float camSpeed;

    public GameObject   tileMapGo;
    public GameObject   machinesGo;
    public GameObject   matsGo;
    public GameObject[] matsUiPrefab;
    public GameObject[] machinesPrefab;
    public GameObject   warningUiGo;
    public GameObject   rocketParticlePrefab;
    public GameObject   mainMenuGo;

    public static GameObject   rocketParticle;
    public static mainMenuHandler   mainMenu;

    private static Dictionary<string, GameObject>  machinesPrefabDic;
    public static MyTileMap                        TileMap;
    private static Dictionary<MatType, GameObject> matsUi;
    public static GameInfo                         Infos;
    public static MachineUI                        machineUi;
    public static MachineInfoUi                    machineInfosUi;
    public static Transform                       machinesContainer;
    public static Transform                       matsContainer;

    public static string savesFolder = "saves";
    public static string savesPreviewFolder = "savesPreview";

    public static string websiteUrl = "http://www.abruptgames.fr";

    private static Camera mainCamera;
    private static GameObject warningUi;
    public static GameObject canvas;
    private static Transform warningsContainer;

    private static Vector3 camCenter;

    private void Start() {
        savesFolder = Path.Combine(Application.persistentDataPath, savesFolder);
        savesPreviewFolder = Path.Combine(Application.persistentDataPath, savesPreviewFolder);

        TileMap = tileMapGo.GetComponent<MyTileMap>();
        machinesContainer = machinesGo.transform;
        matsContainer = matsGo.transform;
        warningUi = warningUiGo;

        mainCamera = Camera.main;
        canvas = transform.parent.Find("Canvas").gameObject;
        warningsContainer = canvas.transform.Find("warningsContainer");

        matsUi = new Dictionary<MatType, GameObject>();
        foreach (GameObject matPrefab in matsUiPrefab) {
            MatType mat = matPrefab.GetComponent<DragAndDropMat>().matPrefab.GetComponent<Mat>().getMatType();
            matsUi[mat] = matPrefab;
        }
        machinesPrefabDic = new Dictionary<string, GameObject>();
        foreach (GameObject machinePrefab in machinesPrefab) {
            machinesPrefabDic[machinePrefab.GetComponent<Machine>().getName()] = machinePrefab;
        }
        rocketParticle = rocketParticlePrefab;
        mainMenu = mainMenuGo.GetComponent<mainMenuHandler>();
    }

    public static void startNewGame(string name) {
        Infos = new GameInfo(name);
        TileMap.createNewMap();
        camCenter = mainCamera.transform.position;
    }

    public static void saveGame() {
        SaveInfo saveInfo = new SaveInfo(Infos);

        if (!Directory.Exists(savesFolder)) {
            Directory.CreateDirectory(savesFolder);
        }
        File.WriteAllText(Path.Combine(savesFolder, Infos.saveName + ".save"), JsonUtility.ToJson(saveInfo));
    }

    public static void loadGame(string name) {
        Infos = new GameInfo(name);
        
        string jsonTxt = File.ReadAllText(Path.Combine(savesFolder, name + ".save"));
        SaveInfo saveInfo = JsonUtility.FromJson<SaveInfo>(jsonTxt);

        Infos.Money = saveInfo.money;
        Infos.startingTime = Time.time - saveInfo.timeSpent;
        TileMap.loadMap(saveInfo.tiles);
        camCenter = mainCamera.transform.position;
    }

    public static void winGame(GameObject spacePort, GameObject rocket) {
        addHighScore(PlayerPrefs.GetString("playerName", "Unkown"), Mathf.RoundToInt(float.Parse(PlayerPrefs.GetString("playerTime", "0")) * 1000));
        canvas.SetActive(false);
        rocket.GetComponent<MonoBehaviour>().StartCoroutine(winAnimation(rocket));
    }

    private static IEnumerator winAnimation(GameObject rocket) {
        Instantiate(rocketParticle, rocket.transform.position - Vector3.up * 0.7f, rocketParticle.transform.rotation, rocket.transform);
        yield return new WaitForSeconds(2);

        float speed = 0.05f;
        while (rocket.transform.position.y < 50) {
            rocket.transform.position += Vector3.up * Time.deltaTime * speed;
            speed *= 1.01f;
            yield return new WaitForEndOfFrame();
        }
        SceneManager.LoadScene("WinScene");
        yield return null;
    }

    public static List<string> loadExistingSaves() {
        if (!Directory.Exists(savesFolder)) {
            Directory.CreateDirectory(savesFolder);
        }
        return new List<string>(Directory.GetFiles(savesFolder));
    }

    public static void quitGame() {
        TileMap.clearMap();
        machineUi.gameObject.SetActive(false);
        machineInfosUi.gameObject.SetActive(false);
        Infos = null;
    }

    public static void addHighScore(string pseudonym, int score) {
        mainMenu.addHighScore(pseudonym, score);
    }

    public static Sprite GetMatImage(MatType mat) {
        return matsUi[mat].GetComponent<Image>().sprite;
    }

    public static GameObject GetMatPrefab(MatType mat) {
        return matsUi[mat];
    }

    public static GameObject getMachinePrefab(string name) {
        if (!machinesPrefabDic.ContainsKey(name)) {
            return null;
        }
        return machinesPrefabDic[name];
    }

    public static void spawnWarningForMachine(Machine machine) {
        Vector3 pos = machine.transform.position;
        pos.y += machine.transform.localScale.y;

        GameObject newWarning = Instantiate(warningUi);
        newWarning.transform.parent = warningsContainer;
        newWarning.GetComponent<RectTransform>().position = pos;
        machine.warning = newWarning;
    }

    private void Update() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 newDir = new Vector3(x, 0, y) * camSpeed * Time.deltaTime;
        if (Mathf.Abs(mainCamera.transform.position.x - camCenter.x + newDir.x) < 10 && Mathf.Abs(mainCamera.transform.position.z - camCenter.z + newDir.z) < 10) {
            mainCamera.transform.position += newDir;
        }
    }
}
