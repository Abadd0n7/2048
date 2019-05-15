using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveDirection
{
    None,
    Up,
    Right,
    Down,
    Left
}

public class InputManager : MonoBehaviour
{
    private GameManager gm;
    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;
    Vector2 firstClickPos;
    Vector2 secondClickPos;
    public static MoveDirection SwipeDirection;

    void Awake()
    {
        gm = GameObject.FindObjectOfType<GameManager>();
    }
	// Use this for initialization
	void Start () {
		
	}
    // Update is called once per frame
    void Update()
    {

        DetectSwipe();
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            gm.Move(MoveDirection.Up);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            gm.Move(MoveDirection.Right);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            gm.Move(MoveDirection.Down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            gm.Move(MoveDirection.Left);
        }
    }

    void DetectSwipe(){
        
            if (Input.touches.Length > 0)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began)
                {
                    firstPressPos = new Vector2(t.position.x, t.position.y);
                }
                if (t.phase == TouchPhase.Ended)
                {
                    secondPressPos = new Vector2(t.position.x, t.position.y);
                    currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
                    currentSwipe.Normalize();
                    // Swipe up
                    if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                    {
                        gm.Move(MoveDirection.Up);// Swipe down
                    }
                    else if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                    {
                        gm.Move(MoveDirection.Down);// Swipe left
                    }
                    else if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                    {
                        gm.Move(MoveDirection.Left);// Swipe right
                    }
                    else if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                    {
                        gm.Move(MoveDirection.Right);
                    }
                }
            }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                firstClickPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
            if (Input.GetMouseButtonUp(0))
            {
                secondClickPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                currentSwipe = new Vector3(secondClickPos.x - firstClickPos.x, secondClickPos.y - firstClickPos.y);
                currentSwipe.Normalize();

                //Swipe directional check
                // Swipe up
                if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    gm.Move(MoveDirection.Up);// Swipe down
                }
                else if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    gm.Move(MoveDirection.Down);// Swipe left
                }
                else if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    gm.Move(MoveDirection.Left);
                    Debug.Log("Left");// Swipe right
                }
                else if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    gm.Move(MoveDirection.Up);
                    Debug.Log("right");
                }
            }
        }
    }
}
