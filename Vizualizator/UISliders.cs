using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISliders : MonoBehaviour {
    public Ellipse ellipse;
    public Slider theta, radius, height, transparency, spawnInterval, fadeTime;
    public TextMeshProUGUI thetaText, radiusText, heightText, transparencyText, spawnIntervalText, fadeTimeText;

    private void Awake() {
        UpdateValues();
    }

    public void UpdateValues() {
        ellipse.angleSeparation = theta.value;
        ellipse.minorRadius = radius.value;
        ellipse.height = height.value;
        ellipse.transparencyMul = transparency.value;
        ellipse.intermediaryAngleDelta = spawnInterval.value;
        ellipse.intermediaryLinesFadeTime = fadeTime.value;

        thetaText.text = $"{theta.value.ToString("0.0")}°";
        radiusText.text = $"{radius.value.ToString("0.0")}";
        heightText.text = $"{height.value.ToString("0.0")}";
        transparencyText.text = $"{transparency.value.ToString("0.0")}";
        spawnIntervalText.text = $"{spawnInterval.value.ToString("0.0")}";
        fadeTimeText.text = $"{fadeTime.value.ToString("0.0")}";
    }
}
