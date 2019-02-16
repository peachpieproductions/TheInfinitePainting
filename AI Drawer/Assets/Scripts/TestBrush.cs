using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBrush : MonoBehaviour {

    public Texture2D sourceImg;
    public int brushNumber = 1;
    public float runTime = 10f;
    public float brushSizeMult = 1f;

    private void Start() {
        
        StartCoroutine(Test());
    }


    IEnumerator Test() {

        if (FindObjectsOfType<TestBrush>().Length < brushNumber) Instantiate(gameObject);
        
        float aspect = (float)Screen.width / Screen.height;
        float h = Camera.main.orthographicSize;
        float w = h * aspect;
        float timer = 0;
        runTime += Random.Range(0f, 5f);
        while (timer <= runTime) {
            timer += Time.deltaTime;
            float randomX = Random.value;
            float randomY = Random.value;
            transform.localScale = Vector3.one * Random.Range(.1f, .15f) * brushSizeMult;
            transform.position = new Vector3(-w + randomX * w * 2f, -h + randomY * h * 2f);
            Color colSample = sourceImg.GetPixel((int)(randomX * sourceImg.width), (int)(randomY * sourceImg.height));
            GetComponent<SpriteRenderer>().color = colSample;
            yield return null;
        }
    }



}
