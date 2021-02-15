using UnityEngine;
using UnityEditor;

namespace CurvePickerTool
{
    using CurveEditor = CurveDrawer;

    public class CurveDrawer
    {
        private Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
        private float animationTimeStart => m_AnimationCurve.curve.length != 0
            ? m_AnimationCurve.curve[0].time 
            : 0f;
        private float animationTimeEnd => m_AnimationCurve.curve.length != 0 
            ? m_AnimationCurve.curve[m_AnimationCurve.curve.length - 1].time
            : 0f;
        private float animationTimeRange => Mathf.Abs(animationTimeEnd - animationTimeStart);
        private float animationTimeMin => Mathf.Min(animationTimeStart, animationTimeEnd);
        private float animationTimeMax => Mathf.Max(animationTimeStart, animationTimeEnd);
        private float animationValueMin => bounds.min.y;
        private float animationValueRange => bounds.max.y - bounds.min.y;

        private float epsilon = 0.0001f;

        public Bounds Bounds
        {
            get
            {
                return bounds;
            }
        }

        private Vector3 m_Translation
        {
            get
            {
                if (animationValueRange > epsilon)
                {
                    return new Vector3(
                        shownArea.position.x - animationTimeMin * m_Scale.x + ToolStyle.CurvePadding.left,
                        shownArea.position.y - animationValueMin * m_Scale.y + shownArea.height - ToolStyle.CurvePadding.bottom,
                        0);
                }
                else
                {
                    return new Vector3(
                        shownArea.position.x - animationTimeMin * m_Scale.x + ToolStyle.CurvePadding.left,
                        shownArea.position.y + shownArea.height - ToolStyle.CurvePadding.bottom,
                        0);

                }
            }
        }

        private Vector2 m_Scale =>
            new Vector2(
                SafeDivide(shownArea.width - ToolStyle.CurvePadding.left - ToolStyle.CurvePadding.right, animationTimeRange),
                -SafeDivide(shownArea.height - ToolStyle.CurvePadding.top - ToolStyle.CurvePadding.bottom, animationValueRange));

        private float SafeDivide(float a, float b)
        {
            if (Mathf.Abs(b) < epsilon)
                return a;
            else
                return a / Mathf.Max(b, 1e-5f);
        }

        private Matrix4x4 drawingToViewMatrix =>
            Matrix4x4.TRS(m_Translation, Quaternion.identity, m_Scale);

        private Vector2 DrawingToViewTransformPoint(Vector2 lhs) =>
            new Vector2(lhs.x * m_Scale.x + m_Translation.x, lhs.y * m_Scale.y + m_Translation.y);

        class CurveControlPointRenderer
        {
            // Control point mesh renderers.
            private ControlPointRenderer m_UnselectedPointRenderer;
            private ControlPointRenderer m_SelectedPointRenderer;
            private ControlPointRenderer m_SelectedPointOverlayRenderer;
            private ControlPointRenderer m_SemiSelectedPointOverlayRenderer;
            private ControlPointRenderer m_WeightedPointRenderer;

            public CurveControlPointRenderer()
            {
                m_UnselectedPointRenderer = new ControlPointRenderer(CurveEditor.MyStyles.pointIcon);
                m_SelectedPointRenderer = new ControlPointRenderer(CurveEditor.MyStyles.pointIconSelected);
                m_SelectedPointOverlayRenderer = new ControlPointRenderer(CurveEditor.MyStyles.pointIconSelectedOverlay);
                m_SemiSelectedPointOverlayRenderer =
                    new ControlPointRenderer(CurveEditor.MyStyles.pointIconSemiSelectedOverlay);
                m_WeightedPointRenderer = new ControlPointRenderer(CurveEditor.MyStyles.pointIconWeighted);
            }

            public void FlushCache()
            {
                m_UnselectedPointRenderer.FlushCache();
                m_SelectedPointRenderer.FlushCache();
                m_SelectedPointOverlayRenderer.FlushCache();
                m_SemiSelectedPointOverlayRenderer.FlushCache();
                m_WeightedPointRenderer.FlushCache();
            }

            public void Clear()
            {
                m_UnselectedPointRenderer.Clear();
                m_SelectedPointRenderer.Clear();
                m_SelectedPointOverlayRenderer.Clear();
                m_SemiSelectedPointOverlayRenderer.Clear();
                m_WeightedPointRenderer.Clear();
            }

            public void Render()
            {
                //m_UnselectedPointRenderer.Render();
                m_SelectedPointRenderer.Render();
                //m_SelectedPointOverlayRenderer.Render();
                //m_SemiSelectedPointOverlayRenderer.Render();
                //m_WeightedPointRenderer.Render();
            }

            public void AddPoint(Rect rect, Color color)
            {
                m_UnselectedPointRenderer.AddPoint(rect, color);
            }

            public void AddSelectedPoint(Rect rect, Color color)
            {
                m_SelectedPointRenderer.AddPoint(rect, color);
                m_SelectedPointOverlayRenderer.AddPoint(rect, Color.white);
            }

            public void AddSemiSelectedPoint(Rect rect, Color color)
            {
                m_SelectedPointRenderer.AddPoint(rect, color);
                m_SemiSelectedPointOverlayRenderer.AddPoint(rect, Color.white);
            }

            public void AddWeightedPoint(Rect rect, Color color)
            {
                m_WeightedPointRenderer.AddPoint(rect, color);
            }
        }

        internal class CurveWrapper
        {
            public delegate void PreProcessKeyMovement(ref Keyframe key);

            internal enum SelectionMode
            {
                None = 0,
                Selected = 1,
            }

            // Curve management
            private CurveRenderer m_Renderer;

            public CurveRenderer renderer
            {
                get { return m_Renderer; }
                set { m_Renderer = value; }
            }

            public AnimationCurve curve
            {
                get { return renderer.GetCurve(); }
            }


            // Input - should not be changed by curve editor
            public EditorCurveBinding binding;
            public Color color;
            public Color wrapColorMultiplier = Color.white;

            public PreProcessKeyMovement
                preProcessKeyMovementDelegate; // Delegate used limit key manipulation to fit curve constraints

            // An additional vertical min / max range clamp when editing multiple curves with different ranges
            public float vRangeMin = -Mathf.Infinity;
            public float vRangeMax = Mathf.Infinity;

            public bool useScalingInKeyEditor = false;
            public string xAxisLabel = null;
            public string yAxisLabel = null;

            internal Bounds ComputeBoundsBetweenTime(float start, float end) => renderer.GetBounds(start, end);
        }

        CurveControlPointRenderer m_PointRenderer;
        CurveRenderer m_Renderer;

        Rect shownArea;
        CurveWrapper m_AnimationCurve;

        public CurveDrawer(AnimationCurve curve)
        {
            Set(curve);
        }
        
        public void Set(AnimationCurve curve)
        {
            m_AnimationCurve = GetCurveWrapper(curve);
        }

        public void OnGUI(Rect rect, SwatchEditor swatch, CurvePicker curvePicker)
        {
            if (m_PointRenderer == null)
                m_PointRenderer = new CurveControlPointRenderer();

            m_PointRenderer.Clear();
            m_AnimationCurve = GetCurveWrapper(curvePicker.Curve);
            
            bounds = m_AnimationCurve.renderer.GetBounds(animationTimeMin, animationTimeMax);

            shownArea = new Rect(rect);

            DrawCurveAndPoints(m_AnimationCurve, swatch, curvePicker);

            //if (swatch.IsDragging)
            if (swatch.SelectedSwatch != null)
                DrawLabelOnCurvePoint(swatch.SelectedSwatch, curvePicker);
        }

        private void DrawLabelOnCurvePoint(SwatchEditor.Swatch swatch, CurvePicker curvePicker)
        {
            float time = curvePicker.GetSampleTime(swatch.time);
            float value = curvePicker.SampleValue(swatch.time);

            Vector2 p = DrawingToViewTransformPoint(new Vector2(time, value));
            GUIContent content = new GUIContent(string.Format("{0:N}", value));
            Vector2 size = MyStyles.dragLabel.CalcSize(content);

            EditorGUI.LabelField(new Rect(p.x - size.x / 2f, p.y - size.y - 8f, size.x, size.y), content,
                MyStyles.dragLabel);
        }

        void DrawCurveAndPoints(CurveWrapper cw, SwatchEditor swatch, CurvePicker curvePicker)
        {
            DrawCurve(cw);
            DrawPointsOnCurve(cw, swatch, curvePicker);
        }

        void DrawPointsOnCurve(CurveWrapper cw, SwatchEditor swatch, CurvePicker curvePicker)
        {
            var swatches = swatch.TimeSwatches;
            Color keyColor = Color.Lerp(cw.color, Color.white, .2f);
            GUI.color = keyColor;

            for (int i = 0; i < swatches.Count; i++)
            {
                var s = swatches[i];
                // float time = s.m_ActualTime;
                float time = curvePicker.GetSampleTime(i);
                // float value = s.m_ActualValue;
                float value = curvePicker.GetSampleValue(i);

                if (swatch.IsDragging && swatch.SelectedSwatch == s)
                {
                    DrawPoint(DrawingToViewTransformPoint(new Vector2(time, value)),
                        CurveWrapper.SelectionMode.Selected);
                }
                else
                {
                    DrawPoint(DrawingToViewTransformPoint(new Vector2(time, value)),
                        CurveWrapper.SelectionMode.None);
                }
            }

            m_PointRenderer.Render();

            GUI.color = Color.white;
        }

        void DrawPoint(Vector2 viewPos, CurveWrapper.SelectionMode selected)
        {
            DrawPoint(viewPos, selected, MouseCursor.Orbit);
        }

        void DrawPoint(Vector2 viewPos, CurveWrapper.SelectionMode selected, MouseCursor mouseCursor)
        {
            Vector2 pointSize;
            if (selected == CurveWrapper.SelectionMode.Selected)
                pointSize = MyStyles.SelectedPointIconSize;
            else
                pointSize = MyStyles.PointIconSize;

            var rect = new Rect(Mathf.Floor(viewPos.x) - pointSize.x / 2f, Mathf.Floor(viewPos.y) - pointSize.y / 2f,
                pointSize.x, pointSize.y);
            m_PointRenderer.AddSelectedPoint(rect, Color.white);
        }

        CurveWrapper GetCurveWrapper(AnimationCurve curve)
        {
            CurveWrapper cw = new CurveWrapper();
            cw.color = new Color(1f, 0f, 0f); // curve color
            cw.renderer = new NormalCurveRenderer(curve);
            return cw;
        }

        void DrawCurve(CurveWrapper cw)
        {
            CurveRenderer renderer = cw.renderer;
            // Color color = Color.red;
            Color color = cw.color;
            Rect framed = shownArea;
            renderer.DrawCurve(framed.xMin, framed.xMax, color, drawingToViewMatrix, cw.wrapColorMultiplier);
        }

        static class MyStyles
        {
            public static Texture2D pointIcon = EditorGUIUtility.whiteTexture;
            public static Texture2D pointIconWeighted = EditorGUIUtility.whiteTexture;
            public static Texture2D pointIconSelected = EditorGUIUtility.whiteTexture;
            public static Texture2D pointIconSelectedOverlay = EditorGUIUtility.whiteTexture;
            public static Texture2D pointIconSemiSelectedOverlay = EditorGUIUtility.whiteTexture;
            public static GUIContent wrapModeMenuIcon = EditorGUIUtility.IconContent("AnimationWrapModeMenu");

            public static Vector2 SelectedPointIconSize = new Vector2(8, 8);
            public static Vector2 PointIconSize = new Vector2(6, 6);

            public static GUIStyle none = new GUIStyle();
            public static GUIStyle labelTickMarksY = "CurveEditorLabelTickMarks";
            public static GUIStyle labelTickMarksX = "CurveEditorLabelTickmarksOverflow";
            public static GUIStyle selectionRect = "SelectionRect";

            public static GUIStyle dragLabel = "ProfilerBadge";
            public static GUIStyle axisLabelNumberField = "AxisLabelNumberField";
            public static GUIStyle rightAlignedLabel = "CurveEditorRightAlignedLabel";
        }
    }
}