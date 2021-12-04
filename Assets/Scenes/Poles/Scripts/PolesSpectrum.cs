using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolesSpectrum : SimpleSpectrum
{
    public Vector3 trackStartPoint;
    public Vector3 trackEndpoint;

    private List<Transform> _bars;

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_bars == null || _bars.Count != barAmount)
            _bars = new List<Transform>(bars);

        for (int i = 0; i < _bars.Count; i++)
        {
            if (_bars[i].position.x < trackEndpoint.x)
            {
                var trm = _bars[i];
                var lastIndex = _bars.Count - 1;
                trm.position = _bars[lastIndex].position + new Vector3(barXSpacing + trm.localScale.x, 0, 0);
                _bars.RemoveAt(i);
                _bars.Insert(lastIndex, trm);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_bars == null) return;
        Gizmos.color = Color.red;
        for (int i = 0; i < _bars.Count; i++)
        {
            Gizmos.DrawSphere(_bars[i].position, 0.5f);
        }
    }
}
