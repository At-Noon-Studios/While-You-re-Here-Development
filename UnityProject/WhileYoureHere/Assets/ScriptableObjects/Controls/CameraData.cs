using UnityEngine;

[CreateAssetMenu(fileName = "CameraData", menuName = "ScriptableObjects/CameraData")]
public class CameraData : ScriptableObject
{
        [SerializeField] private float sensitivity;
        [SerializeField] private float maxYAngle;
        [SerializeField] private float minYAngle;
        
        public float Sensitivity => sensitivity;
        public float MaxYAngle => maxYAngle;
        public float MinYAngle => minYAngle;
}