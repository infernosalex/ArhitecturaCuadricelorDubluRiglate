using UnityEngine;

public class GeneratorLine : MonoBehaviour {
    public float fadeTime;
    public Ellipse ellipse;
    public LineRenderer lr;

    float startTime;

    float initialWidth;

    private void Start() {
        startTime = Time.time;
        initialWidth = lr.widthMultiplier;
    }

    Vector3 CalculatePointOnCircle(float angle, float radius) {
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
    }
    Vector3 CalculatePointOnEllipse(float angle, float minorRadius, float majorRadius) {
        float t = 1f / Mathf.Sqrt(
                (Mathf.Cos(angle) * Mathf.Cos(angle)) / (majorRadius * majorRadius) +
                (Mathf.Sin(angle) * Mathf.Sin(angle)) / (minorRadius * minorRadius)
            );
        return transform.position + t * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
    }
    Vector3 CalculateProjectedPoint(Vector3 point, float minorRadius, float majorRadius) {
        float x = point.x;

        float xRatioSquared = (x * x) / (majorRadius * majorRadius);

        float z = minorRadius * Mathf.Sqrt(1f - xRatioSquared);
        if(point.z < 0) z *= -1;

        return new Vector3(x, 0, z);
    }

    float angleSet;

    public void SetShape(float angle) {
        angleSet = angle;

        Vector3 ellipsePoint = CalculatePointOnEllipse(angle, ellipse.minorRadius, ellipse.circleRadius);
        Vector3 circlePoint = CalculatePointOnCircle(angle + ellipse.angleSeparation * Mathf.Deg2Rad, ellipse.circleRadius);
        Vector3 projectedPoint = CalculateProjectedPoint(circlePoint, ellipse.minorRadius, ellipse.circleRadius);
        Vector3 perpendicularPoint = projectedPoint + Vector3.up * ellipse.height;

        lr.SetPosition(0, ellipsePoint);
        lr.SetPosition(1, perpendicularPoint);
    }

    private void Update() {
        if(Time.time - startTime >= fadeTime) {
            Destroy(gameObject);
            return;
        }

        SetShape(angleSet);

        float t = 1f - Mathf.InverseLerp(startTime, startTime + fadeTime, Time.time);

        lr.widthMultiplier = Mathf.Lerp(0, initialWidth, t);
    }
}
