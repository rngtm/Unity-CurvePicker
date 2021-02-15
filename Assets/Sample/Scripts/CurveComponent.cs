namespace CurvePickerTool.Samples
{
    using UnityEngine;

    public class CurveComponent : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 5f));
        [SerializeField, HideInInspector] private CurvePicker curvePicker = new CurvePicker(4);

        private GameObject[] objects;
        private int objectNum;

        public CurvePicker CurvePicker => curvePicker;
        public AnimationCurve Curve => curve;

        private void Start()
        {
            objectNum = curvePicker.Count;
            objects = new GameObject[objectNum];
            for (int i = 0; i < objectNum; i++)
            {
                objects[i] = Instantiate(prefab);
            }
        }

        private void Update()
        {
            for (int i = 0; i < objectNum; i++)
            {
                float y = curvePicker.GetSampleValue(i);
                objects[i].transform.position = new Vector3(0f, y, 0f);

            }
        }
    }
}