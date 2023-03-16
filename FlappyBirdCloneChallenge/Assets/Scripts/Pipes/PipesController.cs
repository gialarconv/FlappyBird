using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipesController : MonoBehaviour
{
    [SerializeField] private float _distanceBetweenPipes = 2f;
    [SerializeField] private float _randomMaxYPosition = 4f;
    [SerializeField] private PipesObject[] _pipesObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            InitPipePositions();
        }
    }
    public void InitPipePositions()
    {
        int numOfPipes = _pipesObject.Length;

        for (int i = 0; i < numOfPipes; i++)
        {
            int index = i;
            // Calculate the position of the object based on its index in the loop
            float xPos = (index - (numOfPipes / _distanceBetweenPipes)) * _distanceBetweenPipes;
            float randomYPos = Random.Range(0f, _randomMaxYPosition);
            Vector3 newPipePos = new Vector3(xPos, randomYPos, 0f);

            _pipesObject[index].name = $"Pipe {i}";
            _pipesObject[i].InitPipeObject(newPipePos);
        }
    }
}