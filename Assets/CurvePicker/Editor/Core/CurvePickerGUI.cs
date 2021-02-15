using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace CurvePickerTool
{
    public class CurvePickerGUI
    {
        private bool isInitialze = false;
        private GridDrawer gridDrawer;
        private CurveDrawer curveDrawer;
        private SwatchEditor swatchEditor;
        private ToolConfig config;
        private Action<List<SwatchEditor.Swatch>> onDragSwatch;

        public SwatchEditor SwatchEditor => swatchEditor;

        public List<SwatchEditor.Swatch> TimeSwatches
        {
            get { return swatchEditor.TimeSwatches; }
        }
        
        private void Initialize(AnimationCurve curve, CurvePicker curvePicker)
        {
            if (isInitialze) return;

            if (curve == null) return;
            
            curvePicker.DoSample(curve);
            
            config = new ToolConfig();
            gridDrawer = new GridDrawer();
            curveDrawer = new CurveDrawer(curve);
            swatchEditor = new SwatchEditor(config);
            swatchEditor.BuildSwatch(curvePicker);
            swatchEditor.SetListenerChangeSwatch(onDragSwatch);

            isInitialze = true;
        }

        public void OnGUI(string label, AnimationCurve curve, CurvePicker curvePicker)
        {

            if (curvePicker.SamplePoints.Length == 0) return;

            Initialize(curve, curvePicker);
            if (!isInitialze) return;

            curveDrawer.Set(curvePicker.Curve);

            var graphRect = EditorGUILayout.GetControlRect(true, height: ToolStyle.GraphSizeY);
            graphRect.x += ToolStyle.GraphPadding.left;
            graphRect.y += ToolStyle.GraphPadding.top;
            graphRect.width -= ToolStyle.GraphPadding.left + ToolStyle.GraphPadding.right;
            graphRect.height -= ToolStyle.GraphPadding.top + ToolStyle.GraphPadding.bottom + ToolStyle.SwatchHeight * 2f;

        
            // draw header
            var headerRect = graphRect;
            headerRect.height = 18;
            DrawHeader(headerRect, label);
            

            // draw graph
            graphRect.y += headerRect.height;
            graphRect.height -= headerRect.height;
            gridDrawer.OnGUI(graphRect, curveDrawer.Bounds);
            curveDrawer.OnGUI(graphRect, swatchEditor, curvePicker);

            // draw swatch
            var swatchRect = graphRect;
            swatchRect.y += graphRect.height;
            swatchRect.height = ToolStyle.SwatchHeight;
            swatchEditor.OnGUI(swatchRect, curvePicker);
        }

        void DrawHeader(Rect pos, string label)
        {
            //if (Event.current.type != EventType.Repaint)
            //    return;

            var defaultColor = GUI.color;
            GUI.color = ToolStyle.graphHeaderColor.color;

            if (Event.current.type == EventType.Repaint)
            {
                EditorStyles.toolbar.Draw(pos, false, false, false, false);
            }
            GUI.color = defaultColor;

            EditorGUI.LabelField(pos, label, ToolStyle.graphHeaderLabel);
        }


        public void SetListenerChangeSwatch(Action<List<SwatchEditor.Swatch>> callback)
        {
            onDragSwatch = callback;
        }

        public void BuildSwatch(CurvePicker samplerData)
        {
            if (!isInitialze) return;
            swatchEditor.BuildSwatch(samplerData);
        }
    }
}