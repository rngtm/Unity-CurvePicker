using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

// THis
namespace CurvePickerTool
{
    // AnimationCurveに対してSwatchを重ねて表示
    // GradientEditorcsを参考にして作成
    public class SwatchEditor
    {
        static Styles s_Styles;
        
        private int numSteps;
        private Swatch selectedSwatch;
        private List<Swatch> timeSwatches = new List<Swatch>();
        private AnimationCurve curve;
        private bool isDragging;
        private ToolConfig config;
        private float[] samplePoints;
        private Action<List<Swatch>> onChangeSwatch;
        private CurvePicker curvePicker;

        public Swatch SelectedSwatch => selectedSwatch;

        public List<Swatch> TimeSwatches
        {
            get { return timeSwatches; }
        }

        public bool IsDragging => isDragging;

        public SwatchEditor(ToolConfig config)
        {
            this.config = config;
        }

        void DrawSwatch(Rect totalPos, Swatch s, bool upwards)
        {
            if (s_Styles == null)
                s_Styles = new Styles();

            Color temp = GUI.backgroundColor;
            Rect r = CalcSwatchRect(totalPos, s);
            GUI.backgroundColor = Color.white;
            GUIStyle back = upwards ? s_Styles.upSwatch : s_Styles.downSwatch;
            GUIStyle overlay = upwards ? s_Styles.upSwatchOverlay : s_Styles.downSwatchOverlay;
            back.Draw(r, false, false, selectedSwatch == s, false);
            GUI.backgroundColor = temp;
            overlay.Draw(r, false, false, selectedSwatch == s, false);
        }


        static Rect CalcSwatchRect(Rect totalRect, Swatch s)
        {
            float time = s.time;
            return new Rect(totalRect.x + Mathf.Round(totalRect.width * time) - 5, totalRect.y, 10, totalRect.height);
        }


        float GetTime(float actualTime)
        {
            actualTime = Mathf.Clamp01(actualTime);
            return actualTime;
        }

        public void OnGUI(Rect position, CurvePicker curvePicker)
        {
            //return;
            if (s_Styles == null)
                s_Styles = new Styles();

            this.curvePicker = curvePicker;
            samplePoints = curvePicker.SamplePoints;
            curve = curvePicker.Curve;

            BuildArray();

            float modeHeight = 24f;
            float swatchHeight = 16f;
            float editSectionHeight = 26f;
            position.height = modeHeight;
            position.y += 4f;
            position.height = swatchHeight;

            // draw swatches
            var swatchRect = position;
            swatchRect.x += ToolStyle.SwatchPadding.left;
            swatchRect.width -= ToolStyle.SwatchPadding.left + ToolStyle.SwatchPadding.right;
            ShowSwatchArray(swatchRect, timeSwatches, false);

            if (selectedSwatch != null)
            {
                //position.y += swatchHeight;
                position.height = editSectionHeight;
                position.y += 20;

                float locationWidth = 65;
                float locationTextWidth = 48;
                float space = 20;
                float valueTextWidth = 50;
                float totalLocationWidth = locationTextWidth + space + locationTextWidth + locationWidth;

                Rect rect = position;
                rect.height = 18;
                rect.x += 17;
                rect.width -= totalLocationWidth;
                EditorGUIUtility.labelWidth = valueTextWidth;

                // Sample Value
                float sampleValue = curvePicker.GetSampleValue(selectedSwatch.index);
                EditorGUI.LabelField(rect, s_Styles.curveValueText.text, sampleValue.ToString());

                // Location of key
                rect.x += rect.width + space;
                rect.width = locationWidth + locationTextWidth;



                //int controlId = GUIUtility.GetControlID(FocusType.Keyboard);
                //// 監視
                //if (GUIUtility.hotControl != controlId)
                //{
                //    GUIUtility.hotControl = controlId
                //}

                //var evt = Event.current;
                //int controlId = GUIUtility.GetControlID(FocusType.Passive);
                //if (evt.type == EventType.MouseDown && rect.Contains(evt.mousePosition))
                //{
                //    Debug.Log("Click");
                //    if (GUIUtility.hotControl == controlId)
                //    {
                //        evt.Use();
                //    }
                //    //GUIUtility.hotControl = controlId;
                //    //GUIUtility.keyboardControl = controlId;
                //    ////GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Keyboard);
                //    //evt.Use();
                //}

                EditorGUI.BeginChangeCheck();

                float newLocation =
                    EditorGUI.FloatField(rect, s_Styles.locationText, selectedSwatch.time * 100.0f) /
                    100.0f;

                if (EditorGUI.EndChangeCheck())
                {
                    selectedSwatch.time = Mathf.Clamp(newLocation, 0f, 1f);
                    AssignBack();
                }

                //rect.x += rect.width;
                //rect.width = 20;
                //GUI.Label(rect, s_Styles.percentText);
            }
        }

        void ShowSwatchArray(Rect position, List<Swatch> swatches, bool isAlpha)
        {
            //int id = GUIUtility.GetControlID(652347689, FocusType.Passive);
            int id = GUIUtility.GetControlID(652347689, FocusType.Passive);
            Event evt = Event.current;


            float mouseSwatchTime = GetTime((Event.current.mousePosition.x - position.x) / position.width);
            Vector2 fixedStepMousePosition =
                new Vector3(position.x + mouseSwatchTime * position.width, Event.current.mousePosition.y);

            switch (evt.GetTypeForControl(id))
            {
                case EventType.Repaint:
                {
                    bool hasSelection = false;
                    foreach (Swatch s in swatches)
                    {
                        if (selectedSwatch == s)
                        {
                            hasSelection = true;
                            continue;
                        }

                        DrawSwatch(position, s, !isAlpha);
                    }

                    // selected swatch drawn last
                    if (hasSelection && selectedSwatch != null)
                        DrawSwatch(position, selectedSwatch, !isAlpha);
                    break;
                }
                case EventType.MouseDown:
                {
                    Rect clickRect = position;

                    // Swatches have some thickness thus we enlarge the clickable area
                    clickRect.xMin -= 10;
                    clickRect.xMax += 10;
                    if (clickRect.Contains(evt.mousePosition))
                    {
                        GUIUtility.hotControl = id;
                        evt.Use();

                        // Make sure selected is topmost for the click
                        // if (swatches.Contains(m_SelectedSwatch) && !m_SelectedSwatch.m_IsAlpha &&
                        if (swatches.Contains(selectedSwatch) &&
                            CalcSwatchRect(position, selectedSwatch).Contains(evt.mousePosition))
                        {
                            if (evt.clickCount == 2)
                            {
                                GUIUtility.keyboardControl = id;
                                // ColorPicker.Show(GUIView.current, m_SelectedSwatch.m_Value, false, m_HDR);
                                GUIUtility.ExitGUI();
                            }

                            break;
                        }

                        bool found = false;
                        foreach (Swatch s in swatches)
                        {
                            if (CalcSwatchRect(position, s).Contains(fixedStepMousePosition))
                            {
                                found = true;
                                selectedSwatch = s;
                                // EditorGUI.EndEditingActiveTextField();
                                isDragging = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            if (config.InsertSwatchWithMouse)
                            {
                                selectedSwatch = new Swatch(mouseSwatchTime, isAlpha);
                                swatches.Add(selectedSwatch);
                                AssignBack();
                            }
                            else
                            {
                                isDragging = false;
                                selectedSwatch = null;
                            }
                        }
                    }

                    break;
                }
                case EventType.MouseDrag:

                    if (GUIUtility.hotControl == id && selectedSwatch != null)
                    {
                        evt.Use();

                        isDragging = true;

                        if (config.InsertSwatchWithMouse)
                        {
                            // If user drags swatch outside in vertical direction, we'll remove the swatch
                            if ((evt.mousePosition.y + 5 < position.y || evt.mousePosition.y - 5 > position.yMax))
                            {
                                if (config.InsertSwatchWithMouse)
                                {
                                    if (swatches.Count > 1)
                                    {
                                        swatches.Remove(selectedSwatch);
                                        AssignBack();
                                        break;
                                    }
                                }
                            }
                            else if (!swatches.Contains(selectedSwatch))
                            {
                                if (config.InsertSwatchWithMouse)
                                {
                                    swatches.Add(selectedSwatch);
                                }
                            }

                        }

                        selectedSwatch.time = mouseSwatchTime;
                        AssignBack();
                    }

                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id)
                    {
                        GUIUtility.hotControl = 0;
                        evt.Use();

                        // If the dragged swatch is NOT in the timeline, it means it was dragged outside.
                        // We just forget about it and let GC get it later.
                        if (!swatches.Contains(selectedSwatch))
                            selectedSwatch = null;

                        // Remove duplicate keys on mouse up so that we do not kill any keys during the drag
                        // RemoveDuplicateOverlappingSwatches();
                    }

                    isDragging = false;


                    break;

                case EventType.KeyDown:
                    if (evt.keyCode == KeyCode.Delete)
                    {
                        if (config.RemoveSwatchWithKey)
                        {
                            if (selectedSwatch != null)
                            {
                                List<Swatch> listToDeleteFrom;
                                listToDeleteFrom = timeSwatches;
                            
                                if (listToDeleteFrom.Count > 1)
                                {
                                    listToDeleteFrom.Remove(selectedSwatch);
                                    AssignBack();
                                    HandleUtility.Repaint();
                                }
                            }
                        }
                            
                        evt.Use();
                    }

                    break;

                // case EventType.ValidateCommand:
                //     if (evt.commandName == EventCommandNames.Delete)
                //         Event.current.Use();
                //     break;

                // case EventType.ExecuteCommand:
                //     if (evt.commandName == EventCommandNames.ColorPickerChanged)
                //     {
                //         throw new System.NotImplementedException();
                //         // GUI.changed = true;
                //         // m_SelectedSwatch.m_Value = ColorPicker.color;
                //         // AssignBack();
                //         // HandleUtility.Repaint();
                //     }
                //     else if (evt.commandName == EventCommandNames.Delete)
                //     {
                //         // if (swatches.Count > 1)
                //         // {
                //         //     swatches.Remove(m_SelectedSwatch);
                //         //     AssignBack();
                //         //     HandleUtility.Repaint();
                //         // }
                //     }

                // break;
            }

        }

        // Fixed steps are only used if numSteps > 1
        public void BuildSwatch(CurvePicker curvePicker)
        {
            samplePoints = curvePicker.SamplePoints;
            BuildArray();

            if (timeSwatches.Count > 0)
                selectedSwatch = timeSwatches[0];
        }

        void BuildArray()
        {
            if (numSteps != samplePoints.Length)
            {
                numSteps = samplePoints.Length;
                timeSwatches.Clear();
                for (int i = 0; i < numSteps; i++)
                {
                    float time = samplePoints[i];
                    var s = new Swatch(time, false);
                    s.index = i;
                    timeSwatches.Add(s);
                }
            }
            else
            {
                for (int i = 0; i < numSteps; i++)
                {
                    var s = timeSwatches[i];
                    s.index = i;
                    s.time = samplePoints[i];
                }
            }
        }

        // Assign back all swatches, to target gradient.
        void AssignBack()
        {
            timeSwatches.Sort((a, b) => SwatchSort(a, b));
            for (int i = 0; i < timeSwatches.Count; i++)
            {
                timeSwatches[i].index = i;
                // m_SamplePoints[i] = m_TimeSwatches[i].m_ActualTime;
                samplePoints[i] = curvePicker.GetSamplePoint(i);
            }
            
            onChangeSwatch?.Invoke(timeSwatches);
            GUI.changed = true;
        }


        // Kill any swatches that are at the same time (For example as the result of dragging a swatch on top of another)
        void RemoveDuplicateOverlappingSwatches()
        {
            bool didRemoveAny = false;
            for (int i = 1; i < timeSwatches.Count; i++)
            {
                if (Mathf.Approximately(timeSwatches[i - 1].time, timeSwatches[i].time))
                {
                    timeSwatches.RemoveAt(i);
                    i--;
                    didRemoveAny = true;
                }
            }

            if (didRemoveAny)
                AssignBack();
        }

        int SwatchSort(Swatch lhs, Swatch rhs)
        {
            if (lhs.time == rhs.time && lhs == selectedSwatch)
                return -1;
            if (lhs.time == rhs.time && rhs == selectedSwatch)
                return 1;

            return lhs.time.CompareTo(rhs.time);
        }

        public class Swatch
        {
            public int index;
            public float time;

            public Swatch(float time, bool isAlpha)
            {
                this.time = time;
            }
        }

        class Styles
        {
            public GUIStyle upSwatch = "Grad Up Swatch";
            public GUIStyle upSwatchOverlay = "Grad Up Swatch Overlay";
            public GUIStyle downSwatch = "Grad Down Swatch";
            public GUIStyle downSwatchOverlay = "Grad Down Swatch Overlay";

            public GUIContent curveValueText = EditorGUIUtility.TrTextContent("Value");
            public GUIContent locationText = EditorGUIUtility.TrTextContent("Location");
            public GUIContent percentText = new GUIContent("%");
        }

        public void SetListenerChangeSwatch(Action<List<Swatch>> callback)
        {
            onChangeSwatch = callback;
        }
    }
} // namespace