using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MachineUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public GameObject tabButtonPrefab;
    public GameObject itemIconPrfab;
    public GameObject pricePrefab;
    public GameObject sellButton;
    public GameObject launchButton;
    public GameObject rocketName;
    public GameObject priceContainer;

    private Machine currentMachine;
    private Camera  mainCamera;

    private Transform headerContainer;
    private Transform bufferContainer;
    private Transform outputContainer;

    private Dictionary<string, Transform> tabs;

    private bool isIn;
    private bool locked;
    private Vector3 lockPos;

    private void Start() {
        locked = false;
        lockPos = Vector3.zero;
        GameManager.machineUi = this;
        mainCamera = Camera.main;

        RectTransform canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
    
        tabs = new Dictionary<string, Transform>();
        foreach (Transform tab in transform) {
            if (tab.name.Contains("Tab")) {
                tabs[tab.name] = tab;
            }
        }
        headerContainer = transform.Find("header");
        bufferContainer = tabs["bufferTab"].Find("container");
        outputContainer = tabs["outputTab"].Find("settingsMenu");

        gameObject.SetActive(false);
    }

    public bool getLockState() {
        return locked;
    }

    public void setLockState() {
        locked = !locked;
        if (locked) {
            lockPos = transform.position;
        }
    }

    private Transform lastTab = null;

    public void displayUiForMachine(Machine machine) {
        if (currentMachine) {
            return;
        }
        currentMachine = machine;
        if (locked) {
            setPos(lockPos);
        } else {
            setPos(mainCamera.WorldToScreenPoint(machine.transform.position));
        }
        setHeaderTab(currentMachine);
        setMatsList(machine.getOutputList());
        setBufferList(machine.getBuffer(), machine.getInputLimit());
        setInfoTab(machine);

        if ((lastTab == tabs["pricesTab"] && !machine.GetComponent<Harbour>()) ||
            (lastTab == tabs["launchTab"] && !machine.GetComponent<SpacePort>())) {
            lastTab = null;
        }
        goToTab(lastTab == null ? tabs["infoTab"] : lastTab);
        gameObject.SetActive(true);
    }
    private void goToTab(Transform tab) {
        lastTab = tab;
        foreach (Transform child in tabs.Values) {
            if (child != tab) {
                child.gameObject.SetActive(false);
            } else {
                child.gameObject.SetActive(true);
            }
        }
        if (tab.Equals(outputContainer.parent)) {
            foreach (DropMat dropZone in outputContainer.parent.GetComponentsInChildren<DropMat>()) {
                dropZone.setCurrentMachine(currentMachine);
            }
        }
    }

    private void addNewTab(string name, Transform tab) {
        GameObject tabButton = GameObject.Instantiate(tabButtonPrefab, Vector3.zero, Quaternion.identity, headerContainer);

        tabButton.GetComponentInChildren<Text>().text = name;
        tabButton.GetComponent<Button>().onClick.AddListener(delegate { goToTab(tab); });
    }

    private void setHeaderTab(Machine machine) {
        bool isSpacePort = machine.GetComponent<SpacePort>();
        bool isHarbour = machine.GetComponent<Harbour>();
        
        if (isSpacePort) {
            launchButton.GetComponent<Button>().interactable = machine.GetComponent<SpacePort>().isLaunchable &&
                                                            rocketName.GetComponent<InputField>().text.Trim() != "";
        }
        if (isHarbour) {
            setPriceTab(machine);
        }
        foreach (Transform tabButton in headerContainer) {
            GameObject.Destroy(tabButton.gameObject);
        }
        foreach (Transform tab in tabs.Values) {
            string name = tab.name.Substring(0, tab.name.Length - "Tab".Length);
            
            if (tab.name == "launchTab") {
                if (!isSpacePort) {
                    continue;
                }
            } else if (tab.name == "pricesTab") {
                if (!isHarbour) {
                    continue;
                }
            }
            addNewTab(name, tab);
        }
    }

    private void setPriceTab(Machine machine) {
        foreach (Transform child in priceContainer.transform) {
            GameObject.Destroy(child.gameObject);
        }
        foreach (KeyValuePair<MatType, int> priceInfo in machine.GetComponent<Harbour>().prices) {
            if (priceInfo.Key == MatType.NONE) {
                continue;
            }
            GameObject price = Instantiate(pricePrefab, Vector3.zero, Quaternion.identity, priceContainer.transform);
            price.transform.Find("matImage").GetComponent<Image>().sprite = GameManager.GetMatImage(priceInfo.Key);
            price.GetComponentInChildren<Text>().text = priceInfo.Value.ToString();
        }
    }

    private void setInfoTab(Machine machine) {
        GameObject infoTab = tabs["infoTab"].gameObject;
        infoTab.transform.Find("mainImage").GetComponent<Image>().sprite = machine.getImage();
        infoTab.transform.Find("description").GetComponent<Text>().text = machine.getDescription();
        setOutInItems(infoTab, machine);
    }

    private void setOutInItems(GameObject infoTab, Machine machine) {
        Transform outInContainer = infoTab.transform.Find("outInContainer");
        Transform inputCol = outInContainer.Find("inputCol");
        Transform outputCol = outInContainer.Find("outputCol");

        clearContainer(inputCol);
        clearContainer(outputCol);
        foreach ((MatType mat, int number) in machine.getInputList()) {
            addNewIconToCol(mat, number, inputCol);
        }

        foreach ((MatType mat, int number) in machine.getOutputList()) {
            addNewIconToCol(mat, number, outputCol);
        }
    }

    private void addNewIconToCol(MatType mat, int number, Transform col) {
        GameObject matIcon = Instantiate(itemIconPrfab, col);
        matIcon.GetComponentInChildren<Image>().sprite = GameManager.GetMatImage(mat);
        matIcon.GetComponentInChildren<Text>().text = number.ToString();
    }

    private void setBufferList(List<MatType> buffer, int length) {
        clearContainer(bufferContainer);
        foreach (MatType mat in buffer) {
            GameObject matObj = addMatToContainer(mat, bufferContainer);
            GameObject.Destroy(matObj.GetComponent<DragAndDropMat>());
        }
        for (int count = 0; count <  length - buffer.Count; count++) {
            GameObject matObj = GameObject.Instantiate(GameManager.GetMatPrefab(MatType.NONE), Vector3.zero, Quaternion.identity, bufferContainer);
            GameObject.Destroy(matObj.GetComponent<DragAndDropMat>());
        }
    }

    private void setMatsList(List<(MatType, int)> outputs) {
        clearContainer(outputContainer);
        foreach ((MatType mat, int number) in outputs) {
            if (mat != MatType.NONE) {
                addMatToContainer(mat, outputContainer);
            }
        }
    }

    private void clearContainer(Transform container) {
        foreach (Transform child in container) {
            GameObject.Destroy(child.gameObject);
        }
    }

    private GameObject addMatToContainer(MatType mat, Transform parent) {
        return GameObject.Instantiate(GameManager.GetMatPrefab(mat), Vector3.zero, GameManager.GetMatPrefab(mat).transform.rotation, parent);
    }

    public void hideUi() {
        if (locked) {
            lockPos = transform.position;
        }
        currentMachine = null;
        gameObject.SetActive(false);
    }

    public void sellMachine() {
        currentMachine.destroyMachine();
        hideUi();
    }

    public void launchRocket() {
        PlayerPrefs.SetString("playerName", rocketName.GetComponent<InputField>().text);
        PlayerPrefs.SetString("playerTime", (Time.time - GameManager.Infos.startingTime).ToString());
        PlayerPrefs.Save();
        Debug.Log("YOU WIN IN: " + (Time.time - GameManager.Infos.startingTime) + "s");
        Machine spacePort = currentMachine;
        hideUi();
        GameManager.winGame(spacePort.gameObject, spacePort.GetComponent<SpacePort>().rocket);
    }

    public void setPos(Vector3 pos) {
        Vector2 actualSize = transform.lossyScale * GetComponent<RectTransform>().sizeDelta;
        if (pos.x - actualSize.x / 2 < 0) {
            pos.x = actualSize.x / 2;
        } else if (pos.x + actualSize.x / 2 > Screen.width) {
            pos.x = Screen.width - actualSize.x / 2;
        }
        if (pos.y - actualSize.y / 2 < 0) {
            pos.y = actualSize.y / 2;
        } else if (pos.y + actualSize.y / 2 > Screen.height) {
            pos.y = Screen.height - actualSize.y / 2;
        }
        transform.position = pos;
    }

    public void OnPointerEnter(PointerEventData data) {
        isIn = true;
    }

    public void OnPointerExit(PointerEventData data) {
        isIn = false;
    }

    private void Update() {
        if (currentMachine) {
            setBufferList(currentMachine.getBuffer(), currentMachine.getInputLimit());
        }
        if (Input.GetMouseButtonDown(0) && currentMachine != null && !isIn) {
            hideUi();
        }

        if (currentMachine && currentMachine.GetComponent<SpacePort>()) {
            launchButton.GetComponent<Button>().interactable = currentMachine.GetComponent<SpacePort>().isLaunchable &&
                                                            rocketName.GetComponent<InputField>().text.Trim() != "";
        }
    }
}
