using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunksController : MonoBehaviour
{
    private const int BOUND_MIDDLE = 2;
    //1 means right
    private const int SPAWN_DIRECTION = 1;

    public static System.Action OnReuseChunk;

    [Tooltip("The total number of chunks to be created in the pool")]
    [SerializeField] private int _amountOfChunks;
    [Tooltip("The total number of chunks that will be used at the start of the game.")]
    [SerializeField] private int _initAmountOfChunks;
    [SerializeField] private ChunkObject _initialChunkPrefab;
    [SerializeField] private Transform _chunkWithPipesParent;
    [SerializeField] private ChunkObject _chunkWithPipesPrefab;

    private ChunkObject _initialChunkEmptyTransform;
    private ChunkObject _currentChunk;
    private void OnEnable()
    {
        GameStateController.OnResetGame += InitChunks;
        OnReuseChunk += ReuseChunk;
    }
    private void OnDisable()
    {
        GameStateController.OnResetGame += InitChunks;
        OnReuseChunk += ReuseChunk;
    }

    public void InitChunks()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("Need GameManager.");
            return;
        }

        InitChunkEmpty();

        InitChunksWithPipes();
    }
    private void InitChunkEmpty()
    {
        if (_initialChunkEmptyTransform == null)
        {
            _initialChunkEmptyTransform = Instantiate(_initialChunkPrefab, transform);
        }
        else
        {
            _initialChunkEmptyTransform.gameObject.SetActive(true);
            _initialChunkEmptyTransform.transform.localPosition = Vector3.zero;
        }
    }
    private void InitChunksWithPipes()
    {
        if (PoolManager.Instance.PoolAmount() <= 0)
        {
            PoolManager.Instance.CreatePool(_chunkWithPipesPrefab.gameObject, _amountOfChunks, _chunkWithPipesParent);
        }
        else
        {
            foreach (Transform item in _chunkWithPipesParent)
            {
                item.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < _initAmountOfChunks; i++)
        {
            int index = i;

            GameObject chunk = PoolManager.Instance.ReuseObject(_chunkWithPipesPrefab.gameObject, PositionNextTo(_initialChunkEmptyTransform, index), Quaternion.identity);
            _currentChunk = chunk.GetComponent<ChunkObject>();
        }
    }

    private void ReuseChunk()
    {
        GameObject chunkReused = PoolManager.Instance.ReuseObject(_chunkWithPipesPrefab.gameObject, PositionNextTo(_currentChunk, 0), Quaternion.identity);
        _currentChunk = chunkReused.GetComponent<ChunkObject>();
    }

    private Vector3 PositionNextTo(ChunkObject orig, int index)
    {
        // get position next to original (in the x direction)
        Vector3 offset = new Vector3(orig.GroundRenderer.bounds.extents.x * BOUND_MIDDLE, 0f, 0f);
        return orig.GroundTransfrom.position + orig.GroundTransfrom.right * offset.x * -(index + SPAWN_DIRECTION);
    }
}