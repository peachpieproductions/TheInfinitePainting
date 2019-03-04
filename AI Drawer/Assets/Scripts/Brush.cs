using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush : MonoBehaviour {

    public enum BrushType { RANDOMIZE, RandomSketch, Spiral, Stripes, Lines, SquareSpiral, SinWave,
        Splatter, Target, Heart, TOTAL_NUM }

    public BrushType type;
    public bool greyscale;
    public bool mirrorX;
    public float alpha = 1;
    public float oscSpdMult = 1f;
    public float hueSpeedMult = 1f;
    public float turnAngle;
    public float hue;
    public float scaleAmount;
    public float forwardSpeed;
    public float waitTime;

    
    Transform mirrorXBrush;
    float scaleTarget = .2f;
    SpriteRenderer sprRen;

    private void Awake() {
        sprRen = GetComponent<SpriteRenderer>();
        sprRen.enabled = false;
        scaleTarget = transform.localScale.x;
    }

    private void OnEnable() {
        StartCoroutine(Init());
    }

    private void OnDisable() {
        if (mirrorXBrush) Destroy(mirrorXBrush.gameObject);
    }

    private void Update() {

        //Color
        hue = (hue + Time.deltaTime * .01f * hueSpeedMult) % 1;
        var col = Color.HSVToRGB(hue, 1, 1);
        if (greyscale) col = Color.white * hue;
        col.a = alpha;
        sprRen.color = col;

        //Mirror X
        if (mirrorX) {
            UpdateMirrorBrush(col);
        }

        //Bouce Off Walls
        /*if (transform.position.x < -9 || transform.position.x > 9 || transform.position.y < -5 || transform.position.y > 5) {
            transform.Rotate(0, 0, 180);
        }*/

        //Wrap Edges
        if (transform.position.x < -9.25f) transform.position = new Vector3(9.25f, transform.position.y, 0);
        if (transform.position.x > 9.25f) transform.position = new Vector3(-9.25f, transform.position.y, 0);
        if (transform.position.y > 5.25f) transform.position = new Vector3(transform.position.x, -5.25f, 0);
        if (transform.position.y < -5.25f) transform.position = new Vector3(transform.position.x, 5.25f, 0);

        //Brush Adjustments
        transform.position += transform.right * Time.deltaTime * forwardSpeed;
        scaleTarget = Mathf.Clamp(scaleTarget + scaleAmount, .2f, 2);
        transform.localScale = Vector3.Lerp(transform.localScale, scaleTarget * Vector3.one, Time.deltaTime * 3f);
        transform.Rotate(0, 0, turnAngle);
    }

    IEnumerator Init() {
        yield return new WaitForSeconds(1f);
        transform.position = new Vector3(Random.Range(-7, 7), Random.Range(-4, 4));
        ResetBrush();
        StartBrushing();
        sprRen.enabled = true;
        if (mirrorX) UpdateMirrorBrush(sprRen.color);
    }
    public void UpdateMirrorBrush(Color col) {
        if (mirrorXBrush) {
            mirrorXBrush.position = new Vector3(-transform.position.x, transform.position.y, 0);
            mirrorXBrush.localScale = transform.localScale;
            mirrorXBrush.GetComponent<SpriteRenderer>().color = col;
        }
    }

    public void ResetBrush() {
        forwardSpeed = 5;
        turnAngle = 0;
        scaleAmount = 0;

        if (mirrorX) {
            if (mirrorXBrush == null) mirrorXBrush = Instantiate(Main.inst.dumbBrushPrefab, new Vector3 (-transform.position.x, transform.position.y,0), Quaternion.identity).transform;
        } else {
            if (mirrorXBrush) Destroy(mirrorXBrush.gameObject);
        }
    }

    public Coroutine StartBrushing() {
        switch (type) {
            case BrushType.RANDOMIZE:
                return StartCoroutine(RandomBrush());
            case BrushType.RandomSketch:
                return StartCoroutine(RandomSketching());
            case BrushType.Spiral:
                return StartCoroutine(Spiral());
            case BrushType.Stripes:
                return StartCoroutine(Stripes());
            case BrushType.Lines:
                return StartCoroutine(Lines());
            case BrushType.SquareSpiral:
                return StartCoroutine(SquareSpiral());
            case BrushType.SinWave:
                return StartCoroutine(SinWave());
            case BrushType.Splatter:
                return StartCoroutine(Splatter());
            case BrushType.Target:
                return StartCoroutine(Target());
            case BrushType.Heart:
                return StartCoroutine(Heart());
            default:
                return null;
        }
        
    }

    public IEnumerator RandomBrush() {

        Coroutine activeRoutine = null;
        while (true) {

            if (activeRoutine != null) StopCoroutine(activeRoutine);

            ResetBrush();
            waitTime = Random.Range(10f, 25f);
            type = (BrushType)Random.Range(1,(int)BrushType.TOTAL_NUM);
            if (Random.value < .2f) type = BrushType.RandomSketch;
            activeRoutine = StartBrushing();

            yield return new WaitForSeconds(waitTime);
        }

    }

    public IEnumerator RandomSketching() {
        while (true) {
            turnAngle = Mathf.Clamp(turnAngle + Random.Range(-.5f, .5f), -3, 3);
            scaleAmount = Mathf.Clamp(scaleAmount + Random.Range(-1f, 1f) * Time.deltaTime * .2f, -.02f, .02f);
            yield return null;
        }
    }

    public IEnumerator Spiral() {
        turnAngle = 8f;
        scaleAmount = .00025f;
        scaleTarget = .4f;
        float angle = 0;
        Vector3 centerPoint = transform.position;
        bool flipped = Random.value > .5f;
        while (true) {
            if (flipped) angle -= Time.deltaTime * 3f;
            else angle += Time.deltaTime * 3f;
            transform.position = centerPoint + new Vector3(.1f * angle * Mathf.Cos(angle), .1f * angle * Mathf.Sin(angle), 0);
            yield return null;
        }
    }

    public IEnumerator Stripes() {
        scaleTarget = Random.Range(.4f, 1.2f);
        if (Random.value < .5f) transform.eulerAngles = new Vector3(0, 0, Random.Range(260f, 280f));
        else transform.eulerAngles = new Vector3(0, 0, Random.Range(170f, 190f));
        yield return null;
    }

    public IEnumerator Lines() {
        scaleTarget = Random.Range(.4f, .8f);
        while (true) {
            transform.Rotate(0, 0, 30 * Random.Range(-3,3));
            yield return new WaitForSeconds(Random.Range(.5f,5f));
        }
    }

    public IEnumerator SquareSpiral() {
        scaleTarget = Random.Range(.1f,.4f);
        float wait = .1f + Random.Range(0f,.2f);
        float waitAddEachFrame = .03f + Random.Range(0f, .05f);
        int edges = Random.Range(3, 8);
        if (Random.value < .5f) edges = 4;
        forwardSpeed = 9 - edges;
        while (true) {
            transform.Rotate(0, 0, 360 / edges);
            yield return new WaitForSeconds(wait);
            wait += waitAddEachFrame;
        }
    }

    public IEnumerator SinWave() {
        scaleTarget = Random.Range(.4f, 1.2f);
        oscSpdMult = Random.Range(1f, 7f);
        while (true) {
            turnAngle = Mathf.Sin(Time.time * oscSpdMult); 
            yield return null;
        }
    }

    public IEnumerator Splatter() {
        waitTime *= .3f;
        scaleTarget = Random.Range(.4f, 1.5f);
        transform.Rotate(0,0,Random.Range(0, 359));
        forwardSpeed = 0;
        float spacing = Random.Range(2f, 6f);
        bool taper = Random.value > .5f;
        while (true) {
            if (taper) { scaleTarget *= .9f; transform.localScale = scaleTarget * Vector3.one; }
            transform.position += transform.right * scaleTarget * spacing;
            yield return new WaitForSeconds(.6f);
        }
    }

    public IEnumerator Target() {
        scaleTarget = Random.Range(.2f, .5f);
        forwardSpeed = 0;
        float theta = 0;
        float radius = 1;
        Vector3 center = transform.position;
        bool dotVersion = Random.value < .5f;
        float radiusMult = 2.5f + Random.Range(-1, 1);
        while (true) {
            if (dotVersion) {
                while (theta <= 270) {
                    theta += Time.deltaTime * 105;
                    transform.position = center + new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta));
                    yield return null;
                }
            } else {
                while (theta <= 360) {
                    theta += Time.deltaTime * 105;
                    float radians = (Mathf.PI / 180) * theta;
                    transform.position = center + new Vector3(radius * Mathf.Cos(radians), radius * Mathf.Sin(radians));
                    yield return null;
                }
            }
            
            radius += scaleTarget * radiusMult;
            theta = 0;
            yield return null;
        }
    }

    IEnumerator Heart() {
        waitTime *= 1.5f;
        scaleTarget = Random.Range(.235f, .4f);
        forwardSpeed = 0;
        float time = 0;
        float size = .05f + Random.Range(0,.1f);
        float angle = Random.Range(0, 359);
        float radians = (Mathf.PI / 180) * angle;
        Vector3 upOffset = new Vector2(0, Mathf.Sin(radians));
        Vector3 startPos = transform.position;
        while (true) {
            time += Time.deltaTime * 2f;
            float xPos = 16 * Mathf.Pow(Mathf.Sin(time), 3);
            float yPos = 13 * Mathf.Cos(time) - 5 * Mathf.Cos(2 * time) - 2 * Mathf.Cos(3 * time) - Mathf.Cos(4 * time);
            float newX = xPos * Mathf.Cos(angle) - yPos * Mathf.Sin(angle);
            float newY = xPos * Mathf.Sin(angle) + yPos * Mathf.Cos(angle);
            transform.position = startPos + new Vector3(newX, newY, 0) * size;
            if (time > 6.1f) { time = 0; size += .02f; startPos += upOffset * .05f; }
            yield return null;
        }
    }

    




}
