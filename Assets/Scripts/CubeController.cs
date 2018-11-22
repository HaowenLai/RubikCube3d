using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public float speed;

    static bool turnFinishFlag = true;
    static int currentTurnName;
    static float turnDegree = .0f;
    static float angle = .0f;

    // Facing the aim face, counter-clockwisely list other face blocks
    static int[] upFaceAdjMvIdx = { 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    static int[] upFaceAdjRstIdx = { 6, 7, 8, 0, 1, 2, 0, 1, 2, 0, 1, 2 };
    static int[] frontFaceAdjMvIdx = { 6, 6, 6, 2, 4, 6, 0, 0, 0, 0, 2, 4 };
    static int[] frontFaceAdjRstIdx = { 6, 7, 8, 2, 5, 8, 0, 1, 2, 0, 3, 6 };

    static GameObject rubikCube;
    static GameObject currentTurn;
    static Transform upFace, leftFace, frontFace, rightFace, backFace, downFace;

    void Start()
    {
        rubikCube = GameObject.Find("RubikCube");
        upFace = rubikCube.transform.Find("Up Faces");
        leftFace = rubikCube.transform.Find("Left Faces");
        frontFace = rubikCube.transform.Find("Front Faces");
        rightFace = rubikCube.transform.Find("Right Faces");
        backFace = rubikCube.transform.Find("Back Faces");
        downFace = rubikCube.transform.Find("Down Faces");
    }

    static bool isTurnReady(float degree)
    {
        if (!turnFinishFlag)
            return false;
        else
        {
            turnFinishFlag = false;
            turnDegree = degree;

            currentTurn = new GameObject("currentTurnFace");
            currentTurn.transform.SetPositionAndRotation(rubikCube.transform.position, rubikCube.transform.rotation);
            return true;
        }
    }

    // Facing the aim face, counter-clockwisely list other face blocks
    static void setAdjBlocks(Transform f1, Transform f2, Transform f3, Transform f4, int[] mvIdx)
    {
        for (int i = 0; i < 3; i++)
            f1.GetChild(mvIdx[i]).SetParent(currentTurn.transform);
        for (int i = 3; i < 6; i++)
            f2.GetChild(mvIdx[i]).SetParent(currentTurn.transform);
        for (int i = 6; i < 9; i++)
            f3.GetChild(mvIdx[i]).SetParent(currentTurn.transform);
        for (int i = 9; i < 12; i++)
            f4.GetChild(mvIdx[i]).SetParent(currentTurn.transform);
    }

    // Facing the aim face, counter-clockwisely list other face blocks
    static void rstAdjBlocks(Transform f1, Transform f2, Transform f3, Transform f4, int[] mvIdx)
    {
        Transform smallBlock;
        for (int i = 0; i < 12; i++)
        {
            smallBlock = currentTurn.transform.GetChild(0);

            if (i < 3)
                smallBlock.SetParent(f2);
            else if (i < 6)
                smallBlock.SetParent(f3);
            else if (i < 9)
                smallBlock.SetParent(f4);
            else
                smallBlock.SetParent(f1);

            smallBlock.SetSiblingIndex(mvIdx[(i + 3) % 12]);
        }
    }

    static void execTurn(Vector3 orient, float counterClk, float clk)
    {
        if (turnDegree > 0)
            currentTurn.transform.Rotate(orient, counterClk);
        else
            currentTurn.transform.Rotate(orient, clk);
    }

    //degree can be 90(conter-clockwise), -90(clockwise) and 180.
    public static void turnUpFace(float degree)
    {
        if (!isTurnReady(degree))
            return;

        currentTurnName = 0;
        upFace.SetParent(currentTurn.transform);
        setAdjBlocks(backFace, leftFace, frontFace, rightFace, upFaceAdjMvIdx);
    }

    public static void turnFrontFace(float degree)
    {
        if (!isTurnReady(degree))
            return;

        currentTurnName = 2;
        frontFace.SetParent(currentTurn.transform);
        setAdjBlocks(upFace, leftFace, downFace, rightFace, frontFaceAdjMvIdx);
    }

    void Update()
    {
        if (turnFinishFlag)
            return;

        if (angle < Mathf.Abs(turnDegree)) //turn
        {
            //calculate turn degrees
            float delta = speed * Time.deltaTime;
            if (angle + delta < Mathf.Abs(turnDegree))
                angle += delta;
            else
            {
                delta = Mathf.Abs(turnDegree) - angle;
                angle = Mathf.Abs(turnDegree);
            }

            //judge turn method
            switch (currentTurnName)
            {
                case 0:
                    execTurn(new Vector3(0, 1, 0), -delta, delta);
                    break;
                case 2:
                    execTurn(new Vector3(0, 0, 1), delta, -delta);
                    break;
            }//end switch currentTurnName
        }//end if angle<abs(turnDegree)

        else  //do some reset jobs
        {
            switch (currentTurnName)
            {
                case 0:
                    upFace.SetParent(rubikCube.transform);
                    rstAdjBlocks(backFace, leftFace, frontFace, rightFace, upFaceAdjRstIdx);
                    break;
                case 2:
                    frontFace.SetParent(rubikCube.transform);
                    rstAdjBlocks(upFace, leftFace, downFace, rightFace, frontFaceAdjRstIdx);
                    break;
            }//end switch currentTurnName

            angle = .0f;
            turnFinishFlag = true;
            GameObject.Destroy(currentTurn);
        }//end if angle<abs(turnDegree)
    }
}
