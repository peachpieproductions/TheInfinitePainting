using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Main : MonoBehaviour {

    public static Main inst;
    public float drawSpeed = 1f;
    public List<Brush> generatedBrushes = new List<Brush>();
    [Range(0,16)]
    public int spawnBrushesCount;
    public bool mirrorX;
    public float brushHueOffset = .1f;
    public Camera renderCam;
    

    public GameObject background;
    public GameObject brushPrefab;
    public GameObject photoBrushPrefab;
    public GameObject dumbBrushPrefab;

    private void Awake() {
#if !UNITY_EDITOR && UNITY_WEBGL
    WebGLInput.captureAllKeyboardInput = false;
#endif
    }

    private void Start() {
        inst = this;
        StartCoroutine(ColorBackground());
        foreach(Camera cam in FindObjectsOfType<Camera>()) { if (cam != Camera.main) renderCam = cam; }
        for (int i = 0; i < spawnBrushesCount; i++) generatedBrushes.Add(Instantiate(brushPrefab).GetComponent<Brush>());
        int j = 0; foreach (Brush b in generatedBrushes) { b.hue = j * brushHueOffset; b.mirrorX = mirrorX; j++; }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F)) Time.timeScale = drawSpeed * 2;
        if (Input.GetKeyUp(KeyCode.F)) Time.timeScale = drawSpeed;
        //if (Input.GetMouseButtonDown(0)) { Time.timeScale = Time.timeScale > 0 ? 0 : 1; Debug.Log(Time.timeScale); }
        //if (Input.GetMouseButtonDown(1)) StartCoroutine(SaveCameraView());
        //if (Input.GetMouseButtonDown(1)) { SaveWebGLScreenshot(); Debug.Log("screenshot"); }
        
    }

    IEnumerator ColorBackground() {
        yield return new WaitForSeconds(.1f);
        background.SetActive(false);
    }

    /*[DllImport("__Internal")]
    private static extern void DownloadFile(byte[] array, int byteLength, string fileName);

    public void SaveWebGLScreenshot() {
        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] textureBytes = texture.EncodeToPNG();
        DownloadFile(textureBytes, textureBytes.Length, "screenshot.png");
        Destroy(texture);
    }

    public IEnumerator SaveCameraView() {
        yield return new WaitForEndOfFrame();

        // get the camera's render texture
        RenderTexture rendText = RenderTexture.active;
        RenderTexture.active = renderCam.targetTexture;

        // render the texture
        renderCam.Render();

        // create a new Texture2D with the camera's texture, using its height and width
        Texture2D tex = new Texture2D(renderCam.targetTexture.width, renderCam.targetTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, renderCam.targetTexture.width, renderCam.targetTexture.height), 0, 0);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        RenderTexture tempRt = RenderTexture.GetTemporary(1920, 1080);
        tempRt.filterMode = FilterMode.Point;
        RenderTexture.active = tempRt;
        Graphics.Blit(tex, tempRt);
        Texture2D HDtex = new Texture2D(1920, 1080);
        HDtex.filterMode = FilterMode.Point;
        HDtex.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
        HDtex.Apply();

        RenderTexture.active = rendText;

        // store the texture into a .PNG file
        byte[] bytes = HDtex.EncodeToPNG();

        // save the encoded image to a file
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/Screenshots";
        if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
        var fileTotal = System.IO.Directory.GetFiles(path).Length;
        System.IO.File.WriteAllBytes(path + "/screenshot" + fileTotal + ".png", bytes);
        Debug.Log("File saved to " + path + "/screenshot" + fileTotal + ".png");
    }*/


}
