using System;
using UnityEngine;

namespace Fishing
{
    public class LineController : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private Transform[] _points = Array.Empty<Transform>();

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public void SetUpLine(Transform[] points)
        {
            _lineRenderer.positionCount = points.Length;
            _points = points;
        }

        private void Update()
        {
            for (var i = 0; i < _points.Length; i++)
            {
                _lineRenderer.SetPosition(i, _points[i].position);
            }
        }
    }
}