using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    [SerializeField]
    Transform wall;
    [SerializeField]
    GameObject currentScoreLabel;
    [SerializeField]
    GameObject bestScoreLabel;
    [SerializeField]
    GameObject startPanel;
    [SerializeField]
    GameObject gameOverPanel;
    [SerializeField]
    SpawnBlocks spawner;
    [SerializeField]
    Timer timer;
    [SerializeField]
    AudioController audioController;
    [SerializeField]
    Dropdown widthDropdown;
    [SerializeField]
    Dropdown heightDropdown;
    
    string scoreFilename = "scores.txt";

    int bestScore = 0;
    int currentScore = 0;
    int level = 0;

    Grid grid;

    void Start()
    {
        //load best score
        loadData();
        updateScoreLabels();

        //get grid reference to pass to objects
        grid = GetComponent<Grid>();        
    }
    
    //run the game 
    public void startGame()
    {
        updateDimensions();
       
        adjustCamera();

        createField();
        
        timer.run();

        audioController.playMusic();
    }

    //start everything from the very beginning
    public void displayStartPanel()
    {
        gameOverPanel.SetActive(false);
        startPanel.SetActive(true);
    }

    //finish the game:  display gameover, remove playfield, save score
    public void stopTheGame()
    {
        //disable spawner script
        spawner.enabled = false;

        //remove the playfield
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("playfield");
        for (var i = 0; i < gameObjects.Length; i++)
            Destroy(gameObjects[i]);

        //display Game Over panel
        gameOverPanel.SetActive(true);

        //stop the timer
        timer.stop();

        //save the highscore to file
        saveScore();

        audioController.stopMusic();

    }

    //replay with the same field configuration
    public void replay()
    {
        createField();

        timer.run();

        gameOverPanel.SetActive(false);

        audioController.playMusic();
    }

    // update score based on the level and number of rows deleted
    public void updateScore(int n)
    {
        switch (n)
        {
            case 1: currentScore += 40 * (level + 1); break;
            case 2: currentScore += 100 * (level + 1); break;
            case 3: currentScore += 300 * (level + 1); break;
            case 4: currentScore += 1200 * (level + 1); break;
            default: break;
        }
        updateScoreLabels();
    }

    //create playfield and prepare it for launching
    private void createField()
    {        
        grid.initializeGrid();

        placeBorders();

        //hide UI panel
        startPanel.SetActive(false);

        //place and run spawner
        spawner.enabled = true;
        spawner.transform.position = new Vector3(0, grid.getHeight() + grid.getBottom() - 2, 0);        
        spawner.spawnBlock();       
        
    }

    //update dimensions of the playfield grid 
    private void updateDimensions()
    {     
        int width = int.Parse(widthDropdown.options[widthDropdown.value].text);
        int height = int.Parse(heightDropdown.options[heightDropdown.value].text);
      
        grid.setParameters(width, height, -width / 2, -height / 2);           
    }
    
    //save score to the file if it is the highest so far
    private void saveScore()
    {
        if (currentScore > bestScore)
        {
            bestScore = currentScore;

            string path = Application.dataPath + "/" + scoreFilename;
            StreamWriter writer = new StreamWriter(path);
           
            writer.WriteLine(currentScore);

            writer.Close();
        }
    }

    //update scores displayed
    private void updateScoreLabels()
    {
        //update score labels
        currentScoreLabel.GetComponent<Text>().text = "Score: " + currentScore;
        bestScoreLabel.GetComponent<Text>().text = "Best: " + bestScore;
        
    }
    
    //place border bricks around the field
    private void placeBorders()
    {
        //left and right walls
        for (int y = grid.getBottom(); y < grid.getHeight() + grid.getBottom(); y++)
        {           
             Instantiate(wall, new Vector3((float)grid.getLeft() - grid.getBrickSize(), y, 0), Quaternion.identity);
             Instantiate(wall, new Vector3((float)grid.getLeft() + grid.getWidth(), y, 0), Quaternion.identity);
        }

        //bottom wall
        for (int x = grid.getLeft(); x <= grid.getWidth() + grid.getLeft(); x++)
        {
            Instantiate(wall, new Vector3(x, grid.getBottom(), 0), Quaternion.identity);
        }

    }

    //adjust size of the camera to fit the playfield
    private void adjustCamera()
    {
        Camera.main.orthographicSize = Mathf.Max(grid.getHeight(), grid.getWidth()) / 2 + 1;
    } 

    //load parameters and highscore from the text file
    private void loadData()
    {
        // in the current setup the parameters are not used, because the user chooses dimentions
        // but it is possible to load the dimensions and grid position from a file
        // parseTextFile("parameters.txt");

        parseTextFile(scoreFilename);
    }

    //parse integers from a text file 
    private bool parseTextFile(string fileName)
    {
        try
        {
            string line;
            string path = Application.dataPath + "/" + fileName;
            StreamReader theReader = new StreamReader(path);

            using (theReader)
            {
                List<string> entries = new List<string>();
                do
                {
                    line = theReader.ReadLine();

                    if (line != null)
                        entries.Add(line);
                }
                while (line != null);

                if (entries.Count > 0) processEntries(entries);

                theReader.Close();
                return true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("{0}\n", e.Message);
            return false;
        }
    }
    
    //process loaded data (specific implementation for the current configuration)
    private void processEntries(List<string> entries)
    {
        //parsing highest score file
        if (entries.Count == 1)
        {
            bestScore = Int32.Parse(Regex.Match(entries[0], @"-?\d+").Value);
            return;
        }

        //parsing parameter file (currently not in use)
       /*int[] param = new int[entries.Count];

        for (int i = 0; i < entries.Count; i++)
        {
            param[i] = Int32.Parse(Regex.Match(entries[i], @"-?\d+").Value);
        }

        if (param[0] > 4 && param[1] > 6)
        {
            Grid.setParameters(param[0], param[1], param[2], param[3]);
        } */
    }   
}
