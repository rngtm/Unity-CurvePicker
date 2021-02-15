namespace CurvePickerTool.Samples
{
    using UnityEngine;

    [CreateAssetMenu]
    public class CurveData : ScriptableObject
    {
        [SerializeField] private CurvePicker curvePicker1 = new CurvePicker(4);
        [SerializeField] private CurvePicker curvePicker2 = new CurvePicker(6);
        [SerializeField] private AnimationCurve curve1 = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 10f));
        [SerializeField] private AnimationCurve curve2 = new AnimationCurve( new Keyframe(0f, 0f), new Keyframe(1f, 100f) );

        public CurvePicker CurvePicker1 => curvePicker1;
        public CurvePicker CurvePicker2 => curvePicker2;

        public AnimationCurve Curve1 => curve1;
        public AnimationCurve Curve2 => curve2;
    }
}