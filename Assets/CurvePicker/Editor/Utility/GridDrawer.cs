using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Profiling;

namespace CurvePickerTool
{
    using GraphGUI = GridDrawer;

    public class GridDrawer
    {
        private static readonly Color kGridMinorColorDark = new Color(0.0f, 0.0f, 0.0f, 0.18f);
        private static readonly Color kGridMajorColorDark = new Color(0.0f, 0.0f, 0.0f, 0.28f);
        private static readonly Color kGridMinorColorLight = new Color(0.0f, 0.0f, 0.0f, 0.1f);
        private static readonly Color kGridMajorColorLight = new Color(0.0f, 0.0f, 0.0f, 0.15f);
        
        public void OnGUI(Rect position, Bounds bounds)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Styles.graphBackground.Draw(position, false, false, false, false);
            }

            var gridRect = position;
            gridRect.x += ToolStyle.GridPadding.left;
            gridRect.width -= ToolStyle.GridPadding.left + ToolStyle.GridPadding.right;
            gridRect.y += ToolStyle.GridPadding.top;
            gridRect.height -= ToolStyle.GridPadding.top + ToolStyle.GridPadding.bottom;
            DrawGrid(gridRect, 1f);

            DrawAxisLabel(position, bounds);
        }
        
        private void DrawGrid(Rect gridRect, float zoomLevel)
        {
            if (Event.current.type != UnityEngine.EventType.Repaint)
                return;
            Profiler.BeginSample(nameof(DrawGrid));
            // HandleUtility.ApplyWireMaterial();
            GL.PushMatrix();
            GL.Begin(1);
            float t = Mathf.InverseLerp(0.1f, 1f, zoomLevel);
            DrawGridLines(gridRect, ToolStyle.AixsNumberX, ToolStyle.AixsNumberY, UnityEngine.Color.Lerp(GraphGUI.gridMinorColor, GraphGUI.gridMajorColor, t));


            GL.End();
            GL.PopMatrix();
            Profiler.EndSample();
        }
        private void DrawGridLines(Rect gridRect, float gridSize, UnityEngine.Color gridColor)
        {
            GL.Color(gridColor);
            // for (float x = gridRect.xMin - gridRect.xMin % gridSize; (double) x < (double) gridRect.xMax; x += gridSize)
            for (float x = gridRect.x; (double) x < (double) gridRect.xMax; x += gridSize)
                this.DrawLine(new Vector2(x, gridRect.yMin), new Vector2(x, gridRect.yMax));
            
            GL.Color(gridColor);
            // for (float y = gridRect.yMin - gridRect.yMin % gridSize; (double) y < (double) gridRect.yMax; y += gridSize)
            for (float y = gridRect.y; (double) y < (double) gridRect.yMax; y += gridSize)
                this.DrawLine(new Vector2(gridRect.xMin, y), new Vector2(gridRect.xMax, y));
        }
        
        private void DrawGridLines(Rect gridRect, int gridX, int gridY, UnityEngine.Color gridColor)
        {
            float xMin = gridRect.center.x - gridRect.width / 2;
            float xMax = gridRect.center.x + gridRect.width / 2;
            float yMin = gridRect.center.y - gridRect.height / 2;
            float yMax = gridRect.center.y + gridRect.height / 2;
            float deltaX = gridRect.width / (gridX - 1);
            float deltaY = gridRect.height / (gridY - 1);
            
            GL.Color(gridColor);
            float x = xMin;
            for (int i = 0; i < gridX; i++)
            {
                this.DrawLine(new Vector2(x, gridRect.yMin), new Vector2(x, gridRect.yMax));
                x += deltaX;
            }

            float y = yMin;
            for (int i = 0; i < gridY; i++)
            {
                this.DrawLine(new Vector2(gridRect.xMin, y), new Vector2(gridRect.xMax, y));
                y += deltaY;
            }
            
        }

        private void DrawLine(Vector2 p1, Vector2 p2)
        {
            GL.Vertex((Vector3) p1);
            GL.Vertex((Vector3) p2);
        }
        
        //         // The return value for each axis can be -1, if so then we do not have any proper value
        // // to use.
        // private Vector2 GetAxisUiScalars(List<CurveWrapper> curvesWithSameParameterSpace)
        // {
        //     // If none or just one selected curve then use top most rendered curve value
        //     if (selectedCurves.Count <= 1)
        //     {
        //         if (m_DrawOrder.Count > 0)
        //         {
        //             CurveWrapper cw = GetCurveWrapperFromID(m_DrawOrder[m_DrawOrder.Count - 1]);
        //             if (cw != null && cw.getAxisUiScalarsCallback != null)
        //             {
        //                 // Save list
        //                 if (curvesWithSameParameterSpace != null)
        //                     curvesWithSameParameterSpace.Add(cw);
        //                 return cw.getAxisUiScalarsCallback();
        //             }
        //         }
        //         return Vector2.one;
        //     }
        //
        //     // If multiple curves selected we have to check if they are in the same value space
        //     Vector2 axisUiScalars = new Vector2(-1, -1);
        //     if (selectedCurves.Count > 1)
        //     {
        //         // Find common axis scalars if more than one key selected
        //         bool xAllSame = true;
        //         bool yAllSame = true;
        //         Vector2 scalars = Vector2.one;
        //         for (int i = 0; i < selectedCurves.Count; ++i)
        //         {
        //             CurveWrapper cw = GetCurveWrapperFromSelection(selectedCurves[i]);
        //             if (cw == null)
        //                 continue;
        //
        //             if (cw.getAxisUiScalarsCallback != null)
        //             {
        //                 Vector2 temp = cw.getAxisUiScalarsCallback();
        //                 if (i == 0)
        //                 {
        //                     scalars = temp; // init scalars
        //                 }
        //                 else
        //                 {
        //                     if (Mathf.Abs(temp.x - scalars.x) > 0.00001f)
        //                         xAllSame = false;
        //                     if (Mathf.Abs(temp.y - scalars.y) > 0.00001f)
        //                         yAllSame = false;
        //                     scalars = temp;
        //                 }
        //
        //                 // Save list
        //                 if (curvesWithSameParameterSpace != null)
        //                     curvesWithSameParameterSpace.Add(cw);
        //             }
        //         }
        //         if (xAllSame)
        //             axisUiScalars.x = scalars.x;
        //         if (yAllSame)
        //             axisUiScalars.y = scalars.y;
        //     }
        //
        //     return axisUiScalars;
        // }

        
        private void DrawAxisLabel(Rect pos, Bounds bounds)
        {
            // Draw value labels
            // GUI.color = vTickStyle.labelColor;
            GUI.color = ToolStyle.axisLabelColor.color;
            // int labelLevel = vTicks.GetLevelWithMinSeparation(vTickStyle.distLabel);
        
            // float[] ticks = vTicks.GetTicksAtLevel(labelLevel, false);
            // float[] ticksPos = (float[]) ticks.Clone();
        
            // // Calculate how many decimals are needed to show the differences between the labeled ticks
            // int decimals = MathUtils.GetNumberOfDecimalsForMinimumDifference(vTicks.GetPeriodOfLevel(labelLevel));
            // string format = "n" + decimals;
            // m_AxisLabelFormat = format;
            //
            // // Calculate the size of the biggest shown label
            // float labelSize = 35;
            // if (!settings.vTickStyle.stubs && ticks.Length > 1)
            // {
            //     float min = ticks[1];
            //     float max = ticks[ticks.Length - 1];
            //     string minNumber = min.ToString(format) + settings.vTickStyle.unit;
            //     string maxNumber = max.ToString(format) + settings.vTickStyle.unit;
            //     labelSize = Mathf.Max(
            //         MyStyles.labelTickMarksY.CalcSize(new GUIContent(minNumber)).x,
            //         MyStyles.labelTickMarksY.CalcSize(new GUIContent(maxNumber)).x
            //     ) + 6;
            // }
            
            int tickCount = ToolStyle.AixsNumberY;
            float yStart = pos.y - pos.height / 2f + ToolStyle.GridPadding.top; // label y
            float yEnd = pos.y + pos.height / 2f - ToolStyle.GridPadding.bottom;
            float yDelta = (yEnd - yStart - ToolStyle.axisLabelPadding.top  - ToolStyle.axisLabelPadding.bottom) / (tickCount - 1);
            Rect labelRect = new Rect(
                pos.xMin - pos.width + ToolStyle.GridPadding.left - ToolStyle.axisLabelPadding.right, 
                yStart + ToolStyle.axisLabelPadding.top, pos.width, pos.height);
            
            for (int i = 0; i < tickCount; i++)
            {
                float t = (float) i / (tickCount - 1);
                
                GUI.Label(labelRect, Mathf.Lerp(bounds.max.y, bounds.min.y, t).ToString("N1"), ToolStyle.yAxisLabel);
                labelRect.y += yDelta;
            }
        }
        
        Vector2 m_Scale => new Vector2(1, 1);
        Vector2 m_Translation => new Vector2(0, 0);
        public Vector2 DrawingToViewTransformPoint(Vector2 lhs)
        { return new Vector2(lhs.x * m_Scale.x + m_Translation.x, lhs.y * m_Scale.y + m_Translation.y); }

        private static Color gridMinorColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? kGridMinorColorDark : kGridMinorColorLight;
            }
        }

        private static Color gridMajorColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? kGridMajorColorDark : kGridMajorColorLight;
            }
        }
        
        //private static class MyStyles
        //{
        //    public static GUIStyle none = new GUIStyle();
        //    //public static GUIStyle labelTickMarksY = new GUIStyle(EditorStyles.label)
        //    //{
        //    //    alignment = TextAnchor.MiddleRight,
        //    //};

        //    public static GUIStyle labelTickMarksX = "CurveEditorLabelTickmarksOverflow";
        //    public static GUIStyle selectionRect = "SelectionRect";

        //    public static GUIStyle dragLabel = "ProfilerBadge";
        //    public static GUIStyle axisLabelNumberField = "AxisLabelNumberField";
        //    public static GUIStyle rightAlignedLabel = "CurveEditorRightAlignedLabel";
        //}
    }
}