using UnityEditor;
using UnityEngine;

namespace CurvePickerTool
{
    public class ToolStyle
    {

        public static bool drawLocation = false;

        // axis
        public static int AixsNumberX = 7;
        public static int AixsNumberY = 3;
        public static SkinnedColor axisTickColor = new SkinnedColor(kGridMajorColorDark, kGridMajorColorLight); // color and opacity of ticks
        public static SkinnedColor axisLabelColor = new SkinnedColor(new Color(0.0f, 0.0f, 0.0f, 0.32f), new Color(0.8f, 0.8f, 0.8f, 0.32f)); // color and opacity of tick labels
        public static RectOffset axisLabelPadding = new RectOffset(0, 10, 3, 7);
        private static readonly Color kGridMinorColorDark = new Color(0.0f, 0.0f, 0.0f, 0.18f);
        private static readonly Color kGridMajorColorDark = new Color(0.0f, 0.0f, 0.0f, 0.28f);
        private static readonly Color kGridMinorColorLight = new Color(0.0f, 0.0f, 0.0f, 0.1f);
        private static readonly Color kGridMajorColorLight = new Color(0.0f, 0.0f, 0.0f, 0.15f);
        public static GUIStyle yAxisLabel = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleRight,
        };

        // graph name
        public static SkinnedColor graphHeaderColor = new SkinnedColor(new Color(0.0f, 0.0f, 0.0f, 0.32f), new Color(1f, 1f, 1f, 1f) * 1.1f);
        public static GUIStyle graphHeaderLabel = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.UpperLeft,
            //fontStyle = FontStyle.BoldAndItalic,
            padding = new RectOffset(6, 0, 2, 0),
        };

        // padding
        public static RectOffset GraphPadding = new RectOffset(1, 12, 12, 16);
        public static RectOffset GridPadding = new RectOffset(60, 12, 12, 10);
        public static RectOffset CurvePadding => GridPadding;
        public static RectOffset SwatchPadding => CurvePadding;
        public static RectOffset NameLabelPadding = new RectOffset(4, 10, 0, 0);

        // size
        public static int SwatchHeight = 20;
        public static int GraphSizeY = 280;
    }

    public class SkinnedColor
    {
        private Color normalColor;
        private Color proColor;

        public SkinnedColor(Color color, Color proColor)
        {
            this.normalColor = color;
            this.proColor = proColor;
        }

        public SkinnedColor(Color color)
        {
            this.normalColor = color;
            this.proColor = color;
        }

        public Color color
        {
            get { return EditorGUIUtility.isProSkin ? this.proColor : this.normalColor; }
            set
            {
                if (EditorGUIUtility.isProSkin)
                    this.proColor = value;
                else
                    this.normalColor = value;
            }
        }
    }
}