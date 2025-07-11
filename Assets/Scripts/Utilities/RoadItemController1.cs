using System.Collections;
using UnityEngine;

public class RoadItemController1 : MonoBehaviour
{
    private float timer;

    private bool animating;
    private float offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = Random.Range(5f, 10f);
        timer = -offset;
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer < 10)
        {
            if (timer >= 0)
            {
                transform.localScale = new Vector3(scaleFunc(timer), scaleFunc(timer), scaleFunc(timer));
            }
        }
        else
        {
            offset = Random.Range(5f, 10f);
            timer = -offset;
        }
    }

    IEnumerator Animate ()
    {
        yield return new WaitForSeconds(Random.Range(5,10.0f));
        timer = 0;
        while (timer < 10)
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
