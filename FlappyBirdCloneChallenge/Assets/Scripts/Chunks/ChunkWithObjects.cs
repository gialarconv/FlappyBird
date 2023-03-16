using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkWithObjects : ChunkObject
{
    [SerializeField] private PipesController _pipesController;

    private bool _completeDistanceForDisable;

    private void OnEnable()
    {
        _completeDistanceForDisable = false;
        _pipesController.InitPipePositions();
    }
    public override void Update()
    {
        base.Update();
        DisableThisChunk();
    }

    private void DisableThisChunk()
    {
        if (transform.position.x <= -_distanceToDisableObject && !_completeDistanceForDisable)
        {
            _completeDistanceForDisable = true;
            this.gameObject.SetActive(false);

            ChunksController.OnReuseChunk?.Invoke();
        }
    }
}