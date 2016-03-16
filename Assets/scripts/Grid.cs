using UnityEngine;


// grid is a class for simpler representation of the game. 
// The array grid contains references to brick game objects.


public class Grid : MonoBehaviour {
    
    //Grid parameters
    int width = 10;
    int height = 20;
    int left = -5;            //right now the position of the field is centered but it is possible to change in the future
    int bottom = -10;
    int brickSize = 1;
    Transform[,] grid;

    AudioController audioController;    
    Controller controller;

    void Start()
    {
        controller = FindObjectOfType<Controller>();
        audioController = FindObjectOfType<AudioController>();
    }
    // initialization
    public void initializeGrid()
    {  
        grid = new Transform[width, height];
    }

    // delete all full rows
    public void deleteFullRows()
    {
        
        int count = 0;
        int firstRow = -1;
        for (int row = 0; row < height; row++)
        {
            if (isFull(row))
            {
                count++;
                deleteRow(row);
                if (firstRow == -1) firstRow = row; //mark down first deleted row to move bricks later
            }
            if (count == 4) break; // maximum possible amount of rows to delete, no need to check further
        }
        
       
        if (count > 0)
        {            
            moveBricksDown(firstRow, count);            
           
            controller.updateScore(count);

            audioController.playDestroySound();
        }
       
    }
    
    // check if the row is full
    private bool isFull(int row)
    {
        for (int col = 0; col < width; col++)
        {
            if (grid[col, row] == null) return false;
        }

        return true;
    }

    // delete a row 
    private void deleteRow(int row)
    {
        for (int col = 0; col < width; col++)
        {
            Destroy(grid[col, row].gameObject);
            grid[col, row] = null;
        }
    }

    // move all bricks above the deleted rows, N rows down 
    private void moveBricksDown(int row, int n)
    {
        
        for (int i = 0; i < width; i++)
        {
            for (int j = row; j < height - n; j++)
            {
                grid[i, j] = grid[i, j + n];
                if (grid[i, j + n] != null)
                {
                    grid[i, j + n].position -= new Vector3(0, n, 0);
                    grid[i, j + n] = null;
                }
            }
        }
    }

    // check if the point is inside the grid
    public bool isInsideGrid(Vector2 pos)
    {    
        return (pos.x > left &&
                pos.x < left + width &&
                pos.y > bottom);
    }
    
    // return brick that is in the position (or null)
    public Transform getBrick(Vector2 pos)
    {        
        int x = (int)(pos.x + brickSize / 2 - left);
        int y = (int)(pos.y + brickSize / 2 - bottom);

        return grid[x, y];
    }

    // remove all links to the block from the grid. 
    public void removeBlock(Transform block)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (grid[i, j] != null)
                    if (grid[i, j].parent == block)
                        grid[i, j] = null;
            }
        }
    }

    // add brick to the grid
    public void addBrick(Transform brick)
    {
        
        int x = (int)(brick.position.x + brickSize / 2 - left);
        int y = (int)(brick.position.y + brickSize / 2 - bottom);    

        grid[x, y] = brick;
       
    }

    //getters and setters
    public int getWidth()
    {
        return width;
    }
    public int getHeight()
    {
        return height;
    }
    public int getLeft()
    {
        return left;
    }
    public int getBottom()
    {
        return bottom;
    }
    public int getBrickSize()
    {
        return brickSize;
    }
    public bool setParameters(int w, int h, int l, int b)
    {
        if (w >= 4 && h >= 6)
        {
            width = w;
            height = h;
            left = l;
            bottom = b;
            return true;
        }
        else return false;
    }


}
