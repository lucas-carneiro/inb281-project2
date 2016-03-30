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

        //Time using Minutes:Seconds:Milliseconds
        string m = format(Mathf.FloorToInt(time / 60).ToString());
        string s = format((Mathf.Floor(time) % 60).ToString());
        string ms = time.ToString("F2").Split('.')[1];

        timeText.text = m + ":" + s + ":" +  ms;
    }

    string format(string s) {
        if (s.Length < 2) {
            s = "0" + s;
        }
        return s;
    }
}
