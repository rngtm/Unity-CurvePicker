using System;
using UnityEngine;

namespace CurvePickerTool
{
    [Serializable]
    public class ToolConfig
    {
        public bool m_CanInsertSwatch;
        public bool m_CanRemoveSwatch;
        
        public bool CanRemoveSwatch => m_CanRemoveSwatch;
        public bool CanInsertSwatch => m_CanInsertSwatch;
        
        public bool InsertSwatchWithMouse => m_CanInsertSwatch;
        public bool RemoveSwatchWithMouse => m_CanRemoveSwatch;
        public bool RemoveSwatchWithKey => m_CanRemoveSwatch;
        
        public SkinnedColor tickColor = new SkinnedColor(new Color(0.0f, 0.0f, 0.0f, 0.2f), new Color(.45f, .45f, .45f, 0.2f)); // color and opacity of ticks
        public SkinnedColor labelColor = new SkinnedColor(new Color(0.0f, 0.0f, 0.0f, 0.32f), new Color(0.8f, 0.8f, 0.8f, 0.32f)); // color and opacity of tick labels
    }
}