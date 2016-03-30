using UnityEngine;
using UnityEngine.UI;

public class TimeBar : MonoBehaviour {

    public Text timeText;
    private float time;

	// Use this for initialization
	void Start () {
        time = 0f;
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;
        //Time using format Minutes:Seconds:Milliseconds
        timeText.text = "" + Mathf.FloorToInt(time / 60) + ":" + Mathf.Round(time) + ":" +  time.ToString("F2").Split('.')[1];
    }
}
