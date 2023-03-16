using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChunkObject : MonoBehaviour
{
    [SerializeField] protected Renderer _groundRenderer;
    [SerializeField] protected float _distanceToDisableObject;

    public Renderer GroundRenderer => _groundRenderer;
    public Transform GroundTransfrom => _groundRenderer.transform;

    public virtual void Update()
    {
        if (GameStateController.OnGetCurrentGameState() == EnumGameState.Progress)
        {
            transform.position += new Vector3(GameManager.Instance.GameSpeed * -Time.deltaTime, 0f, 0f);
        }
    }
}