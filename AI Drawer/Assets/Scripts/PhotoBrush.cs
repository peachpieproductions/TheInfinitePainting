using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoBrush : MonoBehaviour {


    public Texture2D[] sourceImgs;
    public Texture2D currentImg;
    public int brushNumber = 1;
    public float runTime = 10f;
    public float brushSizeMult = 1f;

    bool mainBrush = true;

    private void Start() {
        if (FindObjectsOfType<PhotoBrush>().Length > 1) mainBrush = false;
        StartCoroutine(DrawPhoto());
    }


    IEnumerator DrawPhoto() {

        if (mainBrush) {
            currentImg = sourceImgs[Random.Range(0, sourceImgs.Length)];
        }

        var otherBrushes = new List<PhotoBrush>();
        while (mainBrush && otherBrushes.Count < brushNumber - 1)
            otherBrushes.Add(Instantiate(gameObject).GetComponent<PhotoBrush>());

        foreach(PhotoBrush b in otherBrushes) {
            b.currentImg = this.currentImg;
        }

        float aspect = (float)Screen.width / Screen.height;
        float h = Camera.main.orthographicSize;
        float w = h * aspect;
        float timer = 0;
        runTime += Random.Range(0f, 5f);
        while (timer <= runTime && currentImg) {
            timer += Time.deltaTime;
            float randomX = Random.value;
            float randomY = Random.value;
            transform.localScale = Vector3.one * Random.Range(.1f, .15f) * brushSizeMult;
            transform.position = new Vector3(-w + randomX * w * 2f, -h + randomY * h * 2f);
            Color colSample = currentImg.GetPixel((int)(randomX * currentImg.width), (int)(randomY * currentImg.height));
            GetComponent<SpriteRenderer>().color = colSample;
            yield return null;
        }

        if (mainBrush) {
            for (int i = otherBrushes.Count - 1; i >= 0; i--) {
                Destroy(otherBrushes[i].gameObject);
            }
            Destroy(gameObject);
        }
    }




}
