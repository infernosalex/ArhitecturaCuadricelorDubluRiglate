using Unity.VisualScripting;
using UnityEngine;

public class Ellipse : MonoBehaviour {
    public Transform ellipsePoint, circlePoint, projectedPoint, perpendicularPoint, thetaText;
    public float angleSeparation, height, speed;
    public float minorRadius;
    public float circleRadius;
    public float transparencyMul;
    public int circleRes, ellipseRes, arcRes;
    public float intermediaryAngleDelta, intermediaryLinesFadeTime;
    public GameObject intermediaryLinePrefab;

    public MeshRenderer coneRenderer;
    public ProceduralCylinder cone;
    Material coneMat;

    public LineRenderer ellipseLine;
    public LineRenderer circleLine;
    public LineRenderer otherLine, arcLine, projectionLine;
    public LineRenderer perpendicularLine;
    public LineRenderer connectionLine;

    float theta;


    Vector3[] ellipsePoints, circlePoints, arcPoints;

    private void Awake() {
        ellipsePoints = new Vector3[ellipseRes];
        circlePoints = new Vector3[circleRes];
        arcPoints = new Vector3[arcRes];
        ellipseLine.positionCount = ellipseRes;
        circleLine.positionCount = circleRes;
        arcLine.positionCount = arcRes;

        coneMat = coneRenderer.material;
        coneMat.SetFloat("_AlphaMul", transparencyMul);
    }


    Vector3 CalculatePointOnEllipse(float angle) {
        float t = 1f / Mathf.Sqrt(
                (Mathf.Cos(angle) * Mathf.Cos(angle)) / (circleRadius * circleRadius) +
                (Mathf.Sin(angle) * Mathf.Sin(angle)) / (minorRadius * minorRadius)
            );
        return transform.position + t * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
    }


    float prevArcAngle;
    void UpdateArcPoints() {
        arcLine.transform.rotation = Quaternion.AngleAxis(-theta * Mathf.Rad2Deg, Vector3.up);
        thetaText.transform.localRotation = Quaternion.AngleAxis(-theta * Mathf.Rad2Deg, Vector3.forward);

        if(prevArcAngle == angleSeparation && prevCircleRadius == circleRadius && minorRadius == prevMinorRadius) return;
        prevArcAngle = angleSeparation;


        float arcRadius = Mathf.Min(circleRadius, circleRadius, minorRadius) / 2;
        float dr = (angleSeparation * Mathf.Deg2Rad % (2 * Mathf.PI)) / arcRes;
        float rot = 0;

        thetaText.localPosition = new Vector3(Mathf.Cos(angleSeparation * Mathf.Deg2Rad / 2), Mathf.Sin(angleSeparation * Mathf.Deg2Rad / 2), 0) * arcRadius / 2;

        for(int i = 0; i < arcRes; i++) {
            arcPoints[i] = transform.position + new Vector3(arcRadius * Mathf.Cos(rot), 0, arcRadius * Mathf.Sin(rot));
            rot += dr;
        }

        arcLine.SetPositions(arcPoints);
    }


    float prevCircleRadius;
    void UpdateCirclePoints() {
        if(prevCircleRadius == circleRadius) return;
        prevCircleRadius = circleRadius;

        float dr = Mathf.PI * 2.0f / (circleRes - 1);
        float rot = 0;

        for(int i = 0; i < circleRes; i++) {
            circlePoints[i] = transform.position + new Vector3(circleRadius * Mathf.Cos(rot), 0, circleRadius * Mathf.Sin(rot));
            rot += dr;
        }

        circleLine.SetPositions(circlePoints);
    }

    Vector3 CalculateProjectedPoint(Vector3 point) {
        float x = point.x;

        float xRatioSquared = (x * x) / (circleRadius * circleRadius);

        float z = minorRadius * Mathf.Sqrt(1f - xRatioSquared);
        if(point.z < 0) z *= -1;

        return new Vector3(x, 0, z);
    }

    float prevMinorRadius;
    void UpdateEllipsePoints() {
        if(prevMinorRadius == minorRadius && prevCircleRadius == circleRadius) return;
        prevMinorRadius = minorRadius;

        float dr = Mathf.PI * 2.0f / ellipseRes;
        float rot = 0;

        for(int i = 0; i < ellipseRes; i++) {
            ellipsePoints[i] = transform.position + new Vector3(circleRadius * Mathf.Cos(rot), 0, minorRadius * Mathf.Sin(rot));
            rot += dr;
        }

        ellipseLine.SetPositions(ellipsePoints);
    }


    float interTheta;
    float prevHeight, prevSeparationAngle;
    bool animating = false;
    private void Update() {
        if(prevMinorRadius != minorRadius || prevCircleRadius != circleRadius || prevSeparationAngle != angleSeparation || prevHeight != height) {
            prevSeparationAngle = angleSeparation;
            prevHeight = height;

            cone.UpdateShape(minorRadius, circleRadius, angleSeparation * Mathf.Deg2Rad, height);
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            animating = !animating;
        }

        if(animating) {
            interTheta += Time.deltaTime * speed;
            theta = (theta + Time.deltaTime * speed) % (2 * Mathf.PI);
        }

        if(interTheta > intermediaryAngleDelta * Mathf.Deg2Rad) {
            interTheta = 0;
            var gl = Instantiate(intermediaryLinePrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<GeneratorLine>();
            gl.ellipse = this;
            gl.fadeTime = intermediaryLinesFadeTime;
            gl.SetShape(theta);
        }

        coneMat.SetFloat("_AlphaMul", transparencyMul);

        ellipsePoint.position = CalculatePointOnEllipse(theta);
        circlePoint.position = transform.position + new Vector3(circleRadius * Mathf.Cos(theta + angleSeparation * Mathf.Deg2Rad), 0, circleRadius * Mathf.Sin(theta + angleSeparation * Mathf.Deg2Rad));
        
        projectedPoint.position = CalculateProjectedPoint(circlePoint.position);
        projectionLine.SetPosition(0, circlePoint.position);
        projectionLine.SetPosition(1, projectedPoint.position);

        perpendicularPoint.position = projectedPoint.position + new Vector3(0, height, 0);
        perpendicularLine.SetPosition(0, projectedPoint.position);
        perpendicularLine.SetPosition(1, perpendicularPoint.position);
        connectionLine.SetPosition(0, ellipsePoint.position);
        connectionLine.SetPosition(1, perpendicularPoint.position);


        UpdateArcPoints();
        UpdateEllipsePoints();
        UpdateCirclePoints();

        otherLine.SetPosition(2, ellipsePoint.position);
        otherLine.SetPosition(0, circlePoint.position);
    }
}
