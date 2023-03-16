using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyChunk : ChunkObject
{
    private bool _isChunkDisable = false;

    private void OnEnable()
    {
        _isChunkDisable = false;
    }

    public override void Update()
    {
        base.Update();
        if (transform.position.x <= -_distanceToDisableObject && !_isChunkDisable)
        {
            _isChunkDisable = true;
            this.gameObject.SetActive(false);
        }
    }
}
