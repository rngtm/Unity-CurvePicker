namespace CurvePickerTool.Samples
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(CurveComponent))]
    public class CurveComponentInspector : Editor
    {
        private CurveComponent component;
        private CurvePickerGUI curveGUI;

        private void OnEnable()
        {
            component = target as CurveComponent;
            curveGUI = new CurvePickerGUI();
            curveGUI.SetListenerChangeSwatch(OnDragSwatch);
            component.CurvePicker.DoSample(component.Curve);
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                // GUIの更新
                component.CurvePicker.DoSample(component.Curve);
                curveGUI.BuildSwatch(component.CurvePicker);
                Repaint();
            }

            // GUI表示
            curveGUI.OnGUI("Position Y", component.Curve, component.CurvePicker);
        }

        private void OnDragSwatch(List<SwatchEditor.Swatch> list)
        {
            RebuildPicker(component.CurvePicker, component.Curve, list);
            EditorUtility.SetDirty(component);
        }

        private void RebuildPicker(CurvePicker curvePicker, AnimationCurve curve, List<SwatchEditor.Swatch> list)
        {
            for (int i = 0; i < curvePicker.Count; i++)
            {
                curvePicker.SetSamplePoint(i, list[i].time);
            }
            curvePicker.DoSample(curve); // GUI更新
        }
    }
}