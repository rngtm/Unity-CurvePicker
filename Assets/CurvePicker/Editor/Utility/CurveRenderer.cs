using UnityEngine;

namespace CurvePickerTool
{
    internal interface CurveRenderer
    {
        void DrawCurve(float minTime, float maxTime, Color color, Matrix4x4 transform, Color wrapColor);
        AnimationCurve GetCurve();
        Bounds GetBounds(float start, float end);
    }
}