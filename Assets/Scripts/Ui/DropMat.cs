using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropMat : MonoBehaviour, IDropHandler, IPointerDownHandler {
    public OutputDirection dir;
    
    private Machine currentMachine;
    private MatType currentMat;

    public void setCurrentMachine(Machine machine) {
        currentMachine = machine;
        setCurrentMat(machine.getOutputType(dir));
    }

    private void setCurrentMat(MatType mat) {
        currentMat = mat;
        GetComponent<Image>().sprite = GameManager.GetMatImage(mat);
    }

    public void OnDrop(PointerEventData eventData) {
        setCurrentMat(DragAndDropMat.dragging);
        currentMachine.setOutputType(dir, currentMat);
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            setCurrentMat(MatType.NONE);
            currentMachine.setOutputType(dir, currentMat);
        }
    }
}
