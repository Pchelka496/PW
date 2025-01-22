using System;
using Nenn.InspectorEnhancements.Runtime.Attributes;
using Nenn.InspectorEnhancements.Runtime.Attributes.Conditional;
using UnityEngine;

public class AerodynamicPart : MonoBehaviour
{
    [HideLabel] [SerializeField] AerodynamicPartData _aerodynamicPartData;
    public AerodynamicPartData AerodynamicPartData => _aerodynamicPartData;
    
#if UNITY_EDITOR
    public void SetAerodynamicPartData(AerodynamicPartData value) => _aerodynamicPartData = value;
#endif

    private void Reset()
    {
        _aerodynamicPartData = AerodynamicPartData.DefaultData();
    }
}

[System.Serializable]
public struct AerodynamicPartData
{
    public float liftSlope;
    public float skinFriction;
    [Range(-180f, 180f)] public float zeroLiftAoA;
    [Range(0f, 180f)] public float stallAngleHigh;
    [Range(-180f, 0f)] public float stallAngleLow;
    [Range(1e-3f, float.MaxValue)] public float chord;
    public float span;
    public bool autoAspectRatio;
    public float aspectRatio;

    public AerodynamicPartData(float liftSlope, float skinFriction, float zeroLiftAoA, float stallAngleHigh,
        float stallAngleLow, float chord, float span, bool autoAspectRatio, float aspectRatio = 2)
    {
        this.liftSlope = liftSlope;
        this.skinFriction = skinFriction;
        this.zeroLiftAoA = zeroLiftAoA;
        this.stallAngleHigh = stallAngleHigh;
        this.stallAngleLow = stallAngleLow;
        this.chord = chord;
        this.span = span;
        this.autoAspectRatio = autoAspectRatio;
        this.aspectRatio = aspectRatio;

        if (autoAspectRatio)
        {
            this.aspectRatio = span / chord;
        }

        OnValidate();
    }

    public static AerodynamicPartData DefaultData()
    {
        return new(
            liftSlope: 6.28f,
            skinFriction: 0.02f,
            zeroLiftAoA: 0,
            stallAngleHigh: 15,
            stallAngleLow: -15,
            chord: 1,
            span: 1,
            autoAspectRatio: true
        );
    }

    private void OnValidate()
    {
        zeroLiftAoA = Mathf.Clamp(zeroLiftAoA, -180f, 180f);
        stallAngleHigh = Mathf.Clamp(stallAngleHigh, 0f, 180f);
        stallAngleLow = Mathf.Clamp(stallAngleLow, -180f, 0);
        chord = Mathf.Clamp(chord, 1e-3f, float.MaxValue);

        if (stallAngleHigh < 0) stallAngleHigh = 0;
        if (stallAngleLow > 0) stallAngleLow = 0;
        if (chord < 1e-3f) chord = 1e-3f;
        if (autoAspectRatio) aspectRatio = span / chord;
    }
}