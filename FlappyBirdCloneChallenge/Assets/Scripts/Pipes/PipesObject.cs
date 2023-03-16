using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PipesObject : MonoBehaviour
{
    [SerializeField] private Transform[] _pipesTransforms;
    [SerializeField] private float _pipeHeight = 4f;
    [SerializeField] private LayerMask _layerToDetect;

    private RaycastHit _rayCastHit;
    private bool _playerPassThrow;

    void Update()
    {
        if (Physics.Linecast(_pipesTransforms.First().position, _pipesTransforms.Last().position, out _rayCastHit, _layerToDetect) && !_playerPassThrow)
        {
            _playerPassThrow = true;
            //add score to the current session
            GameScoreController.OnAddScore?.Invoke();
        }
    }

    public void InitPipeObject(Vector3 newPos)
    {
        _pipesTransforms.First().localPosition = new Vector3(0f, _pipeHeight, 0f);
        _playerPassThrow = false;
        transform.localPosition = newPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_pipesTransforms.First().position, _pipesTransforms.Last().position);
    }
}