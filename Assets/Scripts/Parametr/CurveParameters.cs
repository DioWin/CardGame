using UnityEngine;

[CreateAssetMenu(fileName = "CurveParameters", menuName = "Cards/Curve Parameters", order = 0)]
public class CurveParameters : ScriptableObject
{
    [Header("Positioning")]
    public float curveWidth = 500f;
    public float curveHeight = 100f;
    public AnimationCurve positionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Rotation")]
    public AnimationCurve rotationCurve = AnimationCurve.Linear(0, -1, 1, 1);
    public float rotationInfluence = 10f;
}
