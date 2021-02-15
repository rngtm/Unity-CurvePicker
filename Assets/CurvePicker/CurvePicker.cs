using System;
using UnityEngine;

namespace CurvePickerTool
{
    [Serializable]
    public class CurvePicker
    {
        [SerializeField] private float[] samplePoints = new float[0];
        [SerializeField] private float[] sampleTimes = new float[0];
        [SerializeField] private float[] sampleValues = new float[0];
        private AnimationCurve curve;
        public AnimationCurve Curve => curve;
        public int Count => samplePoints.Length;
        public float[] SamplePoints => samplePoints;

        public CurvePicker(int size)
        {
            samplePoints = new float[size];
            sampleValues = new float[size];

            for (int i = 0; i < size; i++)
            {
                samplePoints[i] = (float) i / (size - 1);
            }
        }

        public void SetSamplePoint(int index, float value)
        {
            if (samplePoints.Length == 0) return;
            samplePoints[index] = value;
        }

        public float GetSamplePoint(int index)
        {
            if (samplePoints.Length == 0) return 0f;
            return samplePoints[index];
        }

        public float GetSampleTime(int index)
        {
            if (sampleTimes.Length == 0) return 0f;
            return sampleTimes[index];
        }

        public float GetSampleValue(int index)
        {
            return sampleValues[index];
        }

        public void DoSample(AnimationCurve curve)
        {
            this.curve = curve;
            int steps = samplePoints.Length;
            sampleValues = new float[steps];
            sampleTimes = new float[steps];

            for (int i = 0; i < steps; i++)
            {
                float sampleTime = samplePoints[i];
                float minTime = curve[0].time;
                float maxTime = curve[curve.length - 1].time;
                sampleTimes[i] = Mathf.Lerp(minTime, maxTime, sampleTime);
                sampleValues[i] = curve.Evaluate(sampleTimes[i]);
            }
        }

        public float GetSampleTime(float pickerPosition)
        {
            // remap
            float minTime = curve[0].time;
            float maxTime = curve[curve.length - 1].time;
            return Mathf.Lerp(minTime, maxTime, pickerPosition);
        }

        public float SampleValue(float swatchTime)
        {
            float time = GetSampleTime(swatchTime);
            return curve.Evaluate(time);
        }
    }

}