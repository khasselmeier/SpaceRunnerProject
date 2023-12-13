using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Begin : MonoBehaviour
{
    private float startTime;
    private float timeTaken;

    public TextMeshProUGUI curTimeText;

    private bool isPlaying;

    void Update()
    {
        if (!isPlaying)
            return;

        curTimeText.text = (Time.time - startTime).ToString("F2");
    }
}