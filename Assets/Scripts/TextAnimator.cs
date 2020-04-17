using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAnimator : MonoBehaviour {

    private Text text;

    private void Awake() {
        this.text = GetComponent<Text>();
    }

    private void Start() {
        StartCoroutine("Animation");
    }

    public void SetText(string s) {
        text.text = s;
    }

    private IEnumerator Animation() {
        yield return new WaitForSecondsRealtime(1);
        Destroy(gameObject);
    }
}
