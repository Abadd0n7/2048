using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Playing,
    GameOver,
    WaitingForMoveToEnd
}
public class GameManager : MonoBehaviour
{
    public GameState State;
    [Range(0, 2f)]
    public float delay;

    private bool moveMade;
    private bool[] lineMoveComplete = new bool[4]{true,true,true,true};

    public GameObject YouWonText;
    public GameObject GameOverText;

    public Text GameOverScore;
    public GameObject GameOverPanel;

    private Tile[,] AllTiles = new Tile[4,4];

    private List<Tile[]> Columns = new List<Tile[]>();
    private List<Tile[]> Rows = new List<Tile[]>();

    private List<Tile> EmptyTiles = new List<Tile>();

	// Use this for initialization
	void Start ()
	{
	    Tile[] AllTilesOneDim = GameObject.FindObjectsOfType<Tile>();

	    foreach (Tile t in AllTilesOneDim)
	    {
	        t.Number = 0;
	        AllTiles[t.indRow, t.indCol] = t;
            EmptyTiles.Add(t);
	    }


	    
	    Columns.Add(new Tile[] { AllTiles[0, 0], AllTiles[1, 0], AllTiles[2, 0], AllTiles[3, 0] });
	    Columns.Add(new Tile[] { AllTiles[0, 1], AllTiles[1, 1], AllTiles[2, 1], AllTiles[3, 1] });
	    Columns.Add(new Tile[] { AllTiles[0, 2], AllTiles[1, 2], AllTiles[2, 2], AllTiles[3, 2] });
	    Columns.Add(new Tile[] { AllTiles[0, 3], AllTiles[1, 3], AllTiles[2, 3], AllTiles[3, 3] });


	    Rows.Add(new Tile[] { AllTiles[0, 0], AllTiles[0, 1], AllTiles[0, 2], AllTiles[0, 3] });
	    Rows.Add(new Tile[] { AllTiles[1, 0], AllTiles[1, 1], AllTiles[1, 2], AllTiles[1, 3] });
	    Rows.Add(new Tile[] { AllTiles[2, 0], AllTiles[2, 1], AllTiles[2, 2], AllTiles[2, 3] });
	    Rows.Add(new Tile[] { AllTiles[3, 0], AllTiles[3, 1], AllTiles[3, 2], AllTiles[3, 3]});

        Generate();
        Generate();
    }

    // Update is called once per frame
    /*void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.G))
	        Generate();
	}*/
    public void NewGameButtnonHandler()
    {
        SceneManager.LoadScene("2048", LoadSceneMode.Single);
    }

    IEnumerator MoveOneLineUpIndexCoroutine(Tile[] line, int index)
    {
        lineMoveComplete[index] = false;
        while (MakeOneMoveUpIndex(line))
        {
            moveMade = true;
            yield return new WaitForSeconds(delay);
        }

        lineMoveComplete[index] = true;
    }

    IEnumerator MoveOneLineDownIndexCoroutine(Tile[] line, int index)
    {
        lineMoveComplete[index] = false;
        while (MakeOneMoveDownIndex(line))
        {
            moveMade = true;
            yield return new WaitForSeconds(delay);
        }

        lineMoveComplete[index] = true;
    }

    bool MakeOneMoveDownIndex(Tile[] LineOfTiles)
    {
        for (int i = 0; i < LineOfTiles.Length - 1; i++)
        {
            //Move 
            if (LineOfTiles[i].Number == 0 && LineOfTiles[i + 1].Number != 0)
            {
                LineOfTiles[i].Number = LineOfTiles[i + 1].Number;
                LineOfTiles[i + 1].Number = 0;
                return true;
            }

            if (LineOfTiles[i].Number != 0 && LineOfTiles[i].Number == LineOfTiles[i + 1].Number && 
                LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i+1].mergedThisTurn == false)
            {
                LineOfTiles[i].Number *= 2;
                LineOfTiles[i + 1].Number = 0;
                LineOfTiles[i].mergedThisTurn = true;

                LineOfTiles[i].PlayMergeAnimation();

                ScoreTracker.Instance.Score += LineOfTiles[i].Number;
                if (LineOfTiles[i].Number == 2048)
                {
                    YouWon();
                }
                return true;
            }
        }
        return false;
    }

    bool MakeOneMoveUpIndex(Tile[] LineOfTiles)
    {
        for (int i = LineOfTiles.Length - 1; i > 0; i--)
        {
            //Move 
            if (LineOfTiles[i].Number == 0 && LineOfTiles[i - 1].Number != 0)
            {
                LineOfTiles[i].Number = LineOfTiles[i - 1].Number;
                LineOfTiles[i - 1].Number = 0;
                return true;
            }

            if (LineOfTiles[i].Number != 0 && LineOfTiles[i].Number == LineOfTiles[i - 1].Number &&
                LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i - 1].mergedThisTurn == false)
            {
                LineOfTiles[i].Number *= 2;
                LineOfTiles[i - 1].Number = 0;
                LineOfTiles[i].mergedThisTurn = true;

                LineOfTiles[i].PlayMergeAnimation();

                ScoreTracker.Instance.Score += LineOfTiles[i].Number;
                if (LineOfTiles[i].Number == 2048)
                {
                    YouWon();
                }
                return true;
            }
        }
        return false;
    }

    private void ResetMergedFlag()
    {
        foreach (Tile t in AllTiles)
        {
            t.mergedThisTurn = false;
        }
    }
    void Generate()
    {
        if (EmptyTiles.Count > 0)
        {
            int indexForNewNumber = Random.Range(0, EmptyTiles.Count);
            int randomNum = Random.Range(0, 10);
            if (randomNum == 0)
                EmptyTiles[indexForNewNumber].Number = 4;
            else 
                EmptyTiles[indexForNewNumber].Number = 2;

            EmptyTiles[indexForNewNumber].PlayAppearAnimation();
            EmptyTiles.RemoveAt(indexForNewNumber);
        }
    }

    private void UpdateEmptyTiles()
    {
        EmptyTiles.Clear();
        foreach (Tile t in AllTiles)
        {
            if(t.Number == 0)
                EmptyTiles.Add(t);
        }
    }

    IEnumerator MoveCoroutine(MoveDirection md)
    {
        State = GameState.WaitingForMoveToEnd;

        switch (md)
        {
            case MoveDirection.Down:
                for (int i = 0; i < Columns.Count; i++)
                    StartCoroutine(MoveOneLineUpIndexCoroutine(Columns[i], i));
                break;
            case MoveDirection.Left:
                for (int i = 0; i < Rows.Count; i++)
                    StartCoroutine(MoveOneLineDownIndexCoroutine(Rows[i], i));
                break;
            case MoveDirection.Right:
                for (int i = 0; i < Rows.Count; i++)
                    StartCoroutine(MoveOneLineUpIndexCoroutine(Rows[i], i));
                break;
            case MoveDirection.Up:
                for (int i = 0; i < Columns.Count; i++)
                    StartCoroutine(MoveOneLineDownIndexCoroutine(Columns[i], i));
                break;
        }

        while (!(lineMoveComplete[0] && lineMoveComplete[1] && lineMoveComplete[2] && lineMoveComplete[3]))
        {
            yield return null;
        }

        if (moveMade)
        {
            UpdateEmptyTiles();
            Generate();
            if (!CanMove())
            {
                GameOver();
            }
        }
        State = GameState.Playing;
        StopAllCoroutines();
    }

    public void Move(MoveDirection md)
    {
        //Debug.Log(md.ToString() + " move.");
        moveMade = false;
        ResetMergedFlag();
        if (delay > 0)
        {
            StartCoroutine(MoveCoroutine(md));
        }
        else
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                switch (md)
                {
                    case MoveDirection.Down:
                        while (MakeOneMoveUpIndex(Columns[i]))
                        {
                            moveMade = true;
                        }
                        break;
                    case MoveDirection.Left:
                        while (MakeOneMoveDownIndex(Rows[i]))
                        {
                            moveMade = true;
                        }
                        break;
                    case MoveDirection.Right:
                        while (MakeOneMoveUpIndex(Rows[i]))
                        {
                            moveMade = true;
                        }
                        break;
                    case MoveDirection.Up:
                        while (MakeOneMoveDownIndex(Columns[i]))
                        {
                            moveMade = true;
                        }
                        break;
                }
            }

            if (moveMade)
            {
                UpdateEmptyTiles();
                Generate();

                if (!CanMove())
                {
                    GameOver();
                }
            }
        }
    }

    private void GameOver()
    {
        GameOverScore.text = ScoreTracker.Instance.Score.ToString();
        GameOverPanel.SetActive(true);
    }

    private void YouWon()
    {
        GameOverText.SetActive(false);
        YouWonText.SetActive(true);
        GameOverScore.text = ScoreTracker.Instance.Score.ToString();
        GameOverPanel.SetActive(true);
    }

    bool CanMove()
    {
        if (EmptyTiles.Count > 0)
        {
            return true;
        }
        else
        {
            //Check cols
            for (int i = 0; i < Columns.Count; i++)
            {
                for (int j = 0; j < Rows.Count - 1; j++)
                {
                    if (AllTiles[j, i].Number == AllTiles[j + 1, i].Number)
                    {
                        return true;
                    }
                }
            }
            //Check rows
            for (int i = 0; i < Rows.Count; i++)
            {
                for (int j = 0; j < Columns.Count - 1; j++)
                {
                    if (AllTiles[i, j].Number == AllTiles[i, j + 1].Number)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
