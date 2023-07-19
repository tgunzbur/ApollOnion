using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum OutputDirection {
    TOPLEFT,
    TOPRIGHT,
    RIGHT,
    DOWNRIGHT,
    DOWNLEFT,
    LEFT
};

public class Machine : MonoBehaviour {
    private static int n_machine = 0;
    protected      int id;

    protected Queue<Mat>  inputs;
    protected MatType[]   outputsType;

    public Tile         Tile { get; set; }
    [HideInInspector]
    public GameObject   warning;
    public Sprite       image;

    private static Dictionary<OutputDirection, Vector2Int> coordsDirections;
    private static Dictionary<OutputDirection, Vector3> transformDirections;

    
    private Dictionary <MatType, int> outputPositions;

    public Machine() {
        id = n_machine;
        n_machine++;

        warning = null;
        inputs = new Queue<Mat>();
        outputsType = new MatType[6] {
            MatType.NONE, MatType.NONE, MatType.NONE,
            MatType.NONE, MatType.NONE, MatType.NONE
        };


        outputPositions = new Dictionary<MatType, int>();
        foreach ((MatType type, int number) in getOutputList()) {
            if (!outputPositions.ContainsKey(type)) {
                outputPositions.Add(type, 0);
            }
        }

        if (coordsDirections == null) {
            coordsDirections = new Dictionary<OutputDirection, Vector2Int>();
            coordsDirections[OutputDirection.TOPLEFT] = new Vector2Int(-1 , 1);
            coordsDirections[OutputDirection.TOPRIGHT] = new Vector2Int(0, 1);
            coordsDirections[OutputDirection.RIGHT] = new Vector2Int(1, 0);
            coordsDirections[OutputDirection.DOWNRIGHT] = new Vector2Int(1, -1);
            coordsDirections[OutputDirection.DOWNLEFT] = new Vector2Int(0, -1);
            coordsDirections[OutputDirection.LEFT] = new Vector2Int(-1, 0);
        }
        if (transformDirections == null) {
            transformDirections = new Dictionary<OutputDirection, Vector3>();
            transformDirections[OutputDirection.TOPLEFT] = new Vector3(-0.5f, 0, 0.75f);
            transformDirections[OutputDirection.TOPRIGHT] = new Vector3(0.5f, 0, 0.75f);
            transformDirections[OutputDirection.RIGHT] = new Vector3(1, 0, 0);
            transformDirections[OutputDirection.DOWNRIGHT] = new Vector3(0.5f, 0, -0.75f);
            transformDirections[OutputDirection.DOWNLEFT] = new Vector3(-0.5f, 0, -0.75f);
            transformDirections[OutputDirection.LEFT] = new Vector3(-1, 0, 0);
        }
    }

    /* *********************** */
    /* **                   ** */
    /* **   TO OVERWRITE    ** */
    /* **                   ** */
    /* *********************** */

    public virtual int getInputLimit() {
        throw new NotImplementedException();
    }

    public virtual List<(MatType, int)> getInputList() {
        throw new NotImplementedException();
    }
    
    public virtual List<(MatType, int)> getOutputList() {
        throw new NotImplementedException();
    }

    public virtual int getPrice() {
        throw new NotImplementedException();
    }

    public virtual string getName() {
        throw new NotImplementedException();
    }

    public virtual string getDescription() {
        return "No Description";
    }

    public virtual bool canBeBuiltOn(EnvironmentType type) {
        return true;
    }

    /* *********************** */
    /* **                   ** */
    /* **      GENERIC      ** */
    /* **                   ** */
    /* *********************** */

    public virtual List<Mat> execute() {
        List<Mat>     outputs = new List<Mat>();
        foreach ((MatType output, int number) in getOutputList()) {
            GameObject prefab = GameManager.GetMatPrefab(output).GetComponent<DragAndDropMat>().matPrefab;
            Mat mat = Instantiate(prefab, transform.position, prefab.transform.rotation, GameManager.matsContainer).GetComponent<Mat>();
            mat.name = prefab.name;
            outputs.Add(mat);
        }
        return outputs;
    }

    List<MatType> inputsType = null;

    protected List<MatType> getInputType() {
        if (inputsType != null) {
            return inputsType;
        }
        inputsType = new List<MatType>();
        foreach ((MatType mat, int number) in getInputList()) {
            for (int count = 0; count < number; count++) {
                inputsType.Add(mat);
            }
        }
        return inputsType;
    }

    public void FixedUpdate() {
        if (!outputDirectionsAreSet()) {
            if (!warning) {
                GameManager.spawnWarningForMachine(this);
            }
            Debug.LogWarning("connot execute Machine " + this.id + " because output directions are not set");
        } else {
            removeWarning();
            if (!hasEnoughInputs()) {
                Debug.LogWarning("connot execute Machine " + this.id + " because has not enough inputs");
            } else {
                destroyInputs();
                sendOutputs(execute());
            }
        }
    }

    private Dictionary<MatType, List<OutputDirection>> getOuputsDict() {
        Dictionary <MatType, List<OutputDirection>> dict = new Dictionary<MatType, List<OutputDirection>>();

        foreach (OutputDirection dir in System.Enum.GetValues(typeof(OutputDirection))) {
            MatType output = this.outputsType[(int)dir];
            if (dict.ContainsKey(output)) {
                dict[output].Add(dir);
            } else {
                dict.Add(output, new List<OutputDirection>{dir});
            }
        }
        return dict;
    }

    private bool hasEnoughInputs(){
        List<MatType> blueprint = new List<MatType>(getInputType());
        foreach (Mat element in this.inputs) {
            if (blueprint.Contains(element.getMatType())) {
                MatType usedMat = blueprint.Find(x => x == element.getMatType());
                blueprint.Remove(usedMat);
            }
        }
        return blueprint.Count == 0;
    }

    private bool outputDirectionsAreSet() {
        foreach ((MatType type, int number) in getOutputList()) {
            if (!outputDirectionIsSetForType(type)) {
                return false;
            }
        }
        return true;
    }

    private bool outputDirectionIsSetForType(MatType type) {
        foreach (OutputDirection dir in System.Enum.GetValues(typeof(OutputDirection))) {
            if (outputsType[(int)dir] == type) {
                return true;
            }
        }
        return false;
    }

    private void destroyInputs() {
        Queue<Mat>    newQueue = new Queue<Mat>();
        List<MatType> blueprint = new List<MatType>(getInputType());
        List<GameObject> toDestroy = new List<GameObject>();

        foreach (Mat element in inputs) {
            MatType usedMat = blueprint.Find(x => x == element.getMatType());
            if (usedMat != MatType.NONE) {
                blueprint.Remove(usedMat);
                if (element) {
                    toDestroy.Add(element.gameObject);
                }
            } else {
                newQueue.Enqueue(element);
            }
        }
        if (blueprint.Count == 0) {
            inputs = newQueue;
            foreach (GameObject item in toDestroy) {
                GameObject.Destroy(item);
            }
        } else {
            throw new GameException("Machine " + this.id + " tries to execute without having the right inputs");
        }
    }

    private void sendOutputs(List<Mat> outputs) {
        Tile[] surroundings = GameManager.TileMap.getSurroundings(this.Tile);
        List<Mat> outputsCopy = new List<Mat>(outputs);
        foreach (Mat mat in outputsCopy) {
            OutputDirection newdir = sendOneOutput(mat, outputPositions[mat.getMatType()]);
            outputs.Remove(mat);
            animOutput(newdir, mat);
            if (surroundings[(int)newdir]) {
                surroundings[(int)newdir].catchMat(mat);
            }
            outputPositions[mat.getMatType()] = ((int)newdir + 1) % 6;
        }
    }

    private OutputDirection sendOneOutput(Mat mat, int a) {
        for (int i = 0; i < 6; i++) {
            if (outputsType[(a + i) % 6] == mat.getMatType()) {
                return (OutputDirection)((a + i) % 6);
            }
        }
        throw new GameException("Machine " + this.id + " tries send output not set");
    }
    
    public void addToInput(Mat newMat) {
        if (inputs.Count >= getInputLimit()) {
            Debug.LogWarning("Machine " + this.id + " : Overflow");
            if (inputs.Peek()) {
                GameObject.Destroy(inputs.Dequeue().gameObject);
            } else {
                inputs.Dequeue();
            }
        }
        inputs.Enqueue(newMat);
    }

    public Mat getFromInput() {
        if (inputs.Count > 0) {
            return inputs.Dequeue();
        } else {
            return null;
        }
    }

    public MatType[] getOutputsType() {
        return outputsType;
    }

    public MatType getOutputType(OutputDirection dir) {
        return outputsType[(int)dir];
    }

    public List<OutputDirection> getOutputDir(MatType type) {
        List<OutputDirection> outputDir = new List<OutputDirection>();
        foreach (OutputDirection dir in System.Enum.GetValues(typeof(OutputDirection))) {
            if (getOutputType(dir) == type) {
                outputDir.Add(dir);
            }
        }
        return outputDir;
    }

    public List<MatType>    getBuffer() {
        List<MatType> bufferList = new List<MatType>();

        foreach (Mat mat in inputs) {
            bufferList.Add(mat.getMatType());
        }
        return bufferList;
    }

    public void setOutputType(OutputDirection dir, MatType type) {
        outputsType[(int)dir] = type;
    }

    public void setOutputsType(MatType[] types) {
        outputsType = types;
    }

    public void animOutput(OutputDirection direction, Mat mat) {
        Tile targetTile = GameManager.TileMap.getAtCoord(Tile.Coords + coordsDirections[direction]);
        Vector3 targetPos = transform.position + transformDirections[direction];

        float matHeight = mat.transform.localScale.y;
        if (targetTile) {
            if (targetTile.Machine) {
                targetPos.y = targetTile.Machine.transform.position.y;
            } else {
                targetPos.y = targetTile.transform.position.y * 2 + targetTile.transform.localScale.y - matHeight;
            }
        } else {
            targetPos.y = -matHeight;
        }
        mat.moveTo(targetPos);
    }

    public Queue<Mat> getInputs() {
        return inputs;
    }

    public  Sprite getImage() {
        return image;
    }

    public void loadInfos(MachineInfo infos, Tile tile) {
        inputs = new Queue<Mat>();
        foreach(MatType mat in infos.inputs) {
            GameObject prefab = GameManager.GetMatPrefab(mat).GetComponent<DragAndDropMat>().matPrefab;
            Mat newMat = Instantiate(prefab, transform.position, prefab.transform.rotation, GameManager.matsContainer).GetComponent<Mat>();
            newMat.name = prefab.name;
            newMat.gameObject.SetActive(false);
            inputs.Enqueue(newMat);
        }
        infos.outputsType.CopyTo(outputsType, 0);
        this.Tile = tile;
    }

    public void destroyMachine() {
        foreach (Mat mat in inputs) {
            GameObject.Destroy(mat.gameObject);
        }
        inputs.Clear();
        removeWarning();
        Tile.Machine = null;
        GameManager.Infos.Money += Mathf.RoundToInt(getPrice() * 0.8f);
        GameObject.Destroy(gameObject);
    }

    public void removeWarning() {
        if (warning) {
            GameObject.Destroy(warning);
            warning = null;
        }
    }
}