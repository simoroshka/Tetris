using UnityEngine;
using System.Collections;

// Block is a figure, consisting of square bricks.
// This script is active only while the block is moving. 

public class Block : MonoBehaviour
{
    //time of the last fall
    float lastFall = 0.0f;
    //fast falling was (not) initialized for this block
    bool fallingDown = false;
    //speed of the falling (per second), lately can depend on the level
    float speed = 1.0f;

    Grid grid;
    SpawnBlocks spawner;   
    AudioController audioController;

    void Start()
    {       

    }

    public void initialize(Grid grid, SpawnBlocks spawner, AudioController audioController, Controller controller)
    {

        this.grid = grid;

        //not a valid position on start means the game is over
        if (!isValidPosition(0, 0))
        {
            controller.stopTheGame();
            Destroy(gameObject);
        }

        this.spawner = spawner;
        this.audioController = audioController;
    }
 
    void Update()
    {
        //move block. If it doesn't, check and destroy full rows and disable the block
        if (moveBlock())
        {
            updateGrid();
        }   
        else
        {
            // remove filled rows
            grid.deleteFullRows();

            // spawn new block            
            spawner.spawnBlock();

            // play sound
            audioController.playFallSound();

            // stop animation
            foreach (Transform brick in transform)
            {
                brick.GetComponent<BrickAnimation>().stopAnimation();
            }
            
            // disable this script
            enabled = false;            
        }    
    }

    // processes user input and also adds "gravity" free fall   
    // returns true if block can be moved
    // returns false if block cannot be moved
    private bool moveBlock()
    {
        //rotate
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.Rotate(0, 0, 90);
            //revert changes if not valid
            if (!isValidPosition(0, 0)) transform.Rotate(0, 0, -90);
            else {                
                return true;
            }
        }

        //move left
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (changePosition(-1, 0)) return true;
        }

        //move right
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (changePosition(1, 0)) return true;
        }

        // move down

        // if one of the buttons (down or spacebar) is pressed, register it for continious fall.
        // This addtional check is performed to avoid the next block to start falling immediately 
        // while the button is still being pressed.
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Space))
            fallingDown = true;

        // if none is pressed, register to stop falling
        else if (!Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.Space))
            fallingDown = false;            

        // fast fall down because of newly or still pressed button
        if (fallingDown)
        {
            if (changePosition(0, -1)) return true; 
            else return false;    
        }
        else
        {
            //only add gravity if not fast falling, otherwise it disrupts the movement
            //if there is nowhere to fall, returns false and the block deactivates
            return addGravity();
        }

    }
    private bool addGravity()
    {
        if (Time.time - lastFall >= 1 / speed)
        {
            lastFall = Time.time;
            return changePosition(0, -1);
        }
        else return true;
    }
    private bool changePosition(int xChange, int yChange)
    {
        
        if (isValidPosition(xChange, yChange))
        {
            transform.position += new Vector3 (xChange, yChange, 0);
            return true;
        }
        else return false;
    }

    // check if the block's current position is valid
    private bool isValidPosition(int xChange, int yChange)
    {
        Vector3 posChange = new Vector3(xChange, yChange, 0);

        foreach (Transform brick in transform)
        {
            Vector3 pos = brick.position + posChange;
           
            if (!grid.isInsideGrid(pos)) return false;

            // there is another block in that position       
            if (grid.getBrick(pos) != null && 
                grid.getBrick(pos).parent != transform)
                    return false;            
        }
        return true;
    }

    //remove old position markers from the grid, add the new ones.
    // NOTE. It is not the most efficient solution, since the grid checks all its elements twice to perform this operation,
    // but the grid is never going to be too large for this to affect the performance. 
    // In turn it makes handling movements (especially rotations) much easier. 
    private void updateGrid()
    {
        // Remove references to bricks of this block from the grid (old positions)
        grid.removeBlock(transform);

        // Add references again, with new positions
        foreach (Transform brick in transform)
        {
            grid.addBrick(brick);
        }     
    }
}