using UnityEngine;
using System.Collections;

public class SpawnBlocks : MonoBehaviour
{
    
    [SerializeField]
    GameObject[] Blocks;
    [SerializeField]
    Grid grid;
    [SerializeField]
    AudioController audioController;
    [SerializeField]
    Controller controller;

   
    public void spawnBlock()
    {        
         int i = Random.Range(0, Blocks.Length);
         GameObject block = Instantiate(Blocks[i], transform.position, Quaternion.identity) as GameObject;
         block.GetComponent<Block>().initialize(grid, this, audioController, controller);
    }
}