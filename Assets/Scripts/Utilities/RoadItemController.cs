using System.Collections;
using UnityEngine;

public class RoadItemController : MonoBehaviour
{

    public Sprite[] waves;
    private float timer;

    private bool animating;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;


    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 10)
        {
            timer += Time.deltaTime;
            transform.localScale = new Vector3(scaleFunc(timer), scaleFunc(timer), scaleFunc(timer));
        }
    }

    void newWave(int waveNum) // 1,2,3
    {
        GetComponent<SpriteRenderer>().sprite = waves[waveNum - 1];
        StartCoroutine("Animate");
    }

    IEnumerator Animate ()
    {
        timer = 0;
        if (timer < 10)
        {
            timer += Time.deltaTime;
            transform.localScale = new Vector3(scaleFunc(timer), scaleFunc(timer), scaleFunc(timer));
        }
        yield return new WaitForSeconds(Random.Range(0,10.0f));
        animating = false;
        yield return null;
    }

    float scaleFunc(float timepass)
    {
        return 0.0709628f * ((float)Mathf.Pow(4.23425f, timepass)) - 0.0588889f;
        
    }
}
