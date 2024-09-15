using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolesSpectrum : SimpleSpectrum
{
    public bool boundsEnabled = true;
    public bool automaticBounds = true;
    public Bounds trackBounds;
    // public BoxCollider boxCollider;
    private List<Transform> _bars;

    void Awake()
    {
        if (boundsEnabled)
        {
            if (automaticBounds)
            {
                var boxCollider = gameObject.AddComponent<BoxCollider>();
                var extentsX = barAmount * (barXSpacing + barXScale);
                boxCollider.size = new Vector3(extentsX, trackBounds.size.y, trackBounds.size.z);
                trackBounds = boxCollider.bounds;
                Destroy(boxCollider);
            }
        }
    }

    private void MoveBarsWithinBounds()
    {
        if (_bars == null || _bars.Count != barAmount)
            _bars = new List<Transform>(bars);

        for (int i = 0; i < _bars.Count; i++)
        {
            Vector3 position = _bars[i].position;
            bool positionChanged = false;

            // Check and adjust X axis
            if (position.x < trackBounds.min.x)
            {
                position.x = trackBounds.max.x;
                positionChanged = true;
            }
            else if (position.x > trackBounds.max.x)
            {
                position.x = trackBounds.min.x;
                positionChanged = true;
            }

            // Check and adjust Y axis
            if (position.y < trackBounds.min.y)
            {
                position.y = trackBounds.max.y;
                positionChanged = true;
            }
            else if (position.y > trackBounds.max.y)
            {
                position.y = trackBounds.min.y;
                positionChanged = true;
            }

            // Check and adjust Z axis
            if (position.z < trackBounds.min.z)
            {
                position.z = trackBounds.max.z;
                positionChanged = true;
            }
            else if (position.z > trackBounds.max.z)
            {
                position.z = trackBounds.min.z;
                positionChanged = true;
            }

            if (positionChanged)
            {
                _bars[i].position = position;
            }
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (boundsEnabled)
        {
            MoveBarsWithinBounds();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (boundsEnabled)
        {
            // Draw bounds
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(trackBounds.center, trackBounds.size);
        }

        if (_bars != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < _bars.Count; i++)
            {
                Gizmos.DrawSphere(_bars[i].position, 0.5f);
            }
        }
    }

}
