namespace CurvePickerTool.Samples
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(CurveData))]
    public class CurveDataInspector : Editor
    {
        private CurveData component;
        private CurvePickerGUI curve1;
        private CurvePickerGUI curve2;

        private void OnEnable()
        {
            component = target as CurveData;

            curve1 = new CurvePickerGUI();
            curve1.SetListenerChangeSwatch(list => OnDragSwatch(component.CurvePicker1, component.Curve1, list));

            curve2 = new CurvePickerGUI();
            curve2.SetListenerChangeSwatch(list => OnDragSwatch(component.CurvePicker2, component.Curve2, list));

            component.CurvePicker1.DoSample(component.Curve1);
            component.CurvePicker2.DoSample(component.Curve2);
        }

        public override void OnInspectorGUI()
        {
            if (component == null) component = target as CurveData;

            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                // GUIの更新
                component.CurvePicker1.DoSample(component.Curve1);
                component.CurvePicker2.DoSample(component.Curve2);
                curve1.BuildSwatch(component.CurvePicker1);
                curve2.BuildSwatch(component.CurvePicker2);
                Repaint();
            }

            // GUI表示
            curve1.OnGUI("Curve 1", component.Curve1, component.CurvePicker1);
            curve2.OnGUI("Curve 2", component.Curve2, component.CurvePicker2);
        }

        private void OnDragSwatch(CurvePicker curvePicker, AnimationCurve curve, List<SwatchEditor.Swatch> list)
        {
            RebuildPicker(curvePicker, curve, list);
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