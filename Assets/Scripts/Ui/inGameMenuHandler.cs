using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class inGameMenuHandler : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject diagram;
    public GameObject screenShotCameraGo;

    private Camera screenShotCamera;
    private Texture2D screenShotTexture;
    private int screenShotWidth;
    private int screenShotHeight;

    private void Start() {
        screenShotCamera = screenShotCameraGo.GetComponent<Camera>();

        screenShotWidth = Screen.width;
        screenShotHeight = Screen.height;
        RenderTexture renderTexture = new RenderTexture(screenShotWidth, screenShotHeight, 24);
        screenShotCamera.targetTexture = renderTexture;
        screenShotTexture = new Texture2D(screenShotWidth, screenShotHeight, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;

        gameObject.SetActive(false);
        diagram.SetActive(false);
    }

    public void saveGame() {
        savePreview();
        hideMenu();
        hideDiagram();
        GameManager.saveGame();
    }

    private void savePreview() {
        if (!Directory.Exists(GameManager.savesPreviewFolder)) {
            Directory.CreateDirectory(GameManager.savesPreviewFolder);
        }
        string path = Path.Combine(GameManager.savesPreviewFolder, GameManager.Infos.saveName + ".png");
        
        Camera tmp = Camera.main;
        Camera.SetupCurrent(screenShotCamera);
        screenShotCamera.Render();
        screenShotTexture.ReadPixels(new Rect(0, 0, screenShotWidth, screenShotHeight), 0, 0);
        Camera.SetupCurrent(tmp);

        byte[] bytesScreenShot = screenShotTexture.EncodeToPNG();
        File.WriteAllBytes(path, bytesScreenShot);
    }

    public void saveAndQuitGame() {
        saveGame();
        GameManager.quitGame();
        hideMenu();
        mainMenu.GetComponent<mainMenuHandler>().goToMainMenu();
    }

    public void quitGame() {
        GameManager.quitGame();
        hideMenu();
        mainMenu.GetComponent<mainMenuHandler>().goToMainMenu();
    }

    public void hideMenu() {
        gameObject.SetActive(false);
    }

    public void showMenu() {
        gameObject.SetActive(true);
    }

    public void showDiagram() {
        diagram.SetActive(true);
    }

    public void hideDiagram() {
        diagram.SetActive(false);
    }
}
