using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralTextAnimator : MonoBehaviour {

    public GameObject objRef;
    public RectTransform parentRef;

    private static GeneralTextAnimator instance;

    private void Awake() {
        instance = this;
    }

    private void spawn(string text) {
        var obj = Instantiate(objRef, parentRef) as GameObject;
        obj.SetActive(true);
        obj.GetComponent<TextAnimator>().SetText(text);
    }

    public static void Spawn(string text) {
        if (instance != null)
            instance.spawn(text);
    }
}
