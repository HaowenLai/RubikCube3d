using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public float speed;

    static bool turnFinishFlag = true;
    static int currentTurnName;
    static float turnDegree = .0f;
    static float sumAngle = .0f;

    // Facing the aim face, counter-clockwisely list other face blocks
    static int[] upFaceAdjMvIdx = { 8, 7, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    static int[] upFaceAdjRstIdx = { 6, 6, 6, 0, 1, 2, 0, 1, 2, 0, 1, 2 };
    static int[] leftFaceAdjMvIdx = { 6, 3, 0, 6, 3, 0, 6, 3, 0, 6, 3, 0 };
    static int[] leftFaceAdjRstIdx = { 4, 2, 0, 4, 2, 0, 4, 2, 0, 4, 2, 0 };
    static int[] frontFaceAdjMvIdx = { 8, 7, 6, 2, 4, 6, 0, 0, 0, 6, 3, 0 };
    static int[] frontFaceAdjRstIdx = { 6, 6, 6, 2, 5, 8, 0, 1, 2, 4, 2, 0 };
    static int[] rightFaceAdjMvIdx = { 2, 4, 6, 2, 4, 6, 2, 4, 6, 2, 4, 6 };
    static int[] rightFaceAdjRstIdx = { 2, 5, 8, 2, 5, 8, 2, 5, 8, 2, 5, 8 };
    static int[] backFaceAdjMvIdx = { 0, 0, 0, 2, 4, 6, 8, 7, 6, 6, 3, 0 };
    static int[] backFaceAdjRstIdx = { 0, 1, 2, 2, 5, 8, 6, 6, 6, 4, 2, 0 };
    static int[] downFaceAdjMvIdx = { 8, 7, 6, 8, 7, 6, 0, 0, 0, 8, 7, 6 };
    static int[] downFaceAdjRstIdx = { 6, 6, 6, 6, 6, 6, 0, 1, 2, 6, 6, 6 };

    static GameObject rubikCube;
    static GameObject currentTurn;
    static Transform upFace, leftFace, frontFace, rightFace, backFace, downFace;
    //static Transform[] solidCubes=new Transform[27];

    void Start()
    {
        rubikCube = GameObject.Find("RubikCube");
        upFace = rubikCube.transform.Find("Up Faces");
        leftFace = rubikCube.transform.Find("Left Faces");
        frontFace = rubikCube.transform.Find("Front Faces");
        rightFace = rubikCube.transform.Find("Right Faces");
        backFace = rubikCube.transform.Find("Back Faces");
        downFace = rubikCube.transform.Find("Down Faces");

        //Transform smallCubes = rubikCube.transform.Find("SmallCubes");
        //for (int i = 0; i < 27;i++)
        //    solidCubes[i] = smallCubes.GetChild(i);
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

    static void setSolidCubes()
    {
        
    }

    // Facing the aim face, counter-clockwisely list other face blocks
    static void rstAdjBlocks(Transform f1, Transform f2, Transform f3, Transform f4, int[] mvIdx)
    {
        Transform smallBlock;

        for (int i = 0; i < 12; i++)
        {
            smallBlock = currentTurn.transform.GetChild(0);

            if (turnDegree > 0) //counter-clockwise
            {
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
            else //clockwise
            {
                if (i < 3)
                    smallBlock.SetParent(f4);
                else if (i < 6)
                    smallBlock.SetParent(f1);
                else if (i < 9)
                    smallBlock.SetParent(f2);
                else
                    smallBlock.SetParent(f3);
                smallBlock.SetSiblingIndex(mvIdx[(i + 9) % 12]);
            }
        }//end for i=1:12
    }

    static void rstCurrentFace(Transform crtFace)
    {
        int[] ccwFaceRstIdx = { 2, 5, 8, 4, 6, 8 };
        int[] cwFaceRstIdx = { 6, 4, 2, 7, 6, 5, 8, 8 };

        if(turnDegree>0)    //counter-clockwise
            for (int i = 0; i < 6; i++)
            {
                Transform smallBlock = crtFace.GetChild(ccwFaceRstIdx[i]);
                smallBlock.SetSiblingIndex(i);
            }
        else    //clockwise
            for (int i = 0; i < 8; i++)
            {
                Transform smallBlock = crtFace.GetChild(cwFaceRstIdx[i]);
                smallBlock.SetSiblingIndex(i);
            }

        crtFace.SetParent(rubikCube.transform);

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

    public static void turnLeftFace(float degree)
    {
        if (!isTurnReady(degree))
            return;

        currentTurnName = 1;
        leftFace.SetParent(currentTurn.transform);
        setAdjBlocks(upFace, backFace, downFace, frontFace, leftFaceAdjMvIdx);
    }

    public static void turnFrontFace(float degree)
    {
        if (!isTurnReady(degree))
            return;

        currentTurnName = 2;
        frontFace.SetParent(currentTurn.transform);
        setAdjBlocks(upFace, leftFace, downFace, rightFace, frontFaceAdjMvIdx);
    }

    public static void turnRightFace(float degree)
    {
        if (!isTurnReady(degree))
            return;

        currentTurnName = 3;
        rightFace.SetParent(currentTurn.transform);
        setAdjBlocks(upFace, frontFace, downFace, backFace, rightFaceAdjMvIdx);
    }

    public static void turnBackFace(float degree)
    {
        if (!isTurnReady(degree))
            return;

        currentTurnName = 4;
        backFace.SetParent(currentTurn.transform);
        setAdjBlocks(upFace, rightFace, downFace, leftFace, backFaceAdjMvIdx);
    }

    public static void turnDownFace(float degree)
    {
        if (!isTurnReady(degree))
            return;

        currentTurnName = 5;
        downFace.SetParent(currentTurn.transform);
        setAdjBlocks(frontFace, leftFace, backFace, rightFace, downFaceAdjMvIdx);
    }

    void Update()
    {
        if (turnFinishFlag)
            return;

        if (sumAngle < Mathf.Abs(turnDegree)) //turn
        {
            //calculate turn degrees
            float delta = speed * Time.deltaTime;
            if (sumAngle + delta < Mathf.Abs(turnDegree))
                sumAngle += delta;
            else
            {
                delta = Mathf.Abs(turnDegree) - sumAngle;
                sumAngle = Mathf.Abs(turnDegree);
            }

            //judge turn method
            switch (currentTurnName)
            {
                case 0:
                    execTurn(new Vector3(0, 1, 0), -delta, delta);
                    break;
                case 1:
                    execTurn(new Vector3(1, 0, 0), delta, -delta);
                    break;
                case 2:
                    execTurn(new Vector3(0, 0, 1), delta, -delta);
                    break;
                case 3:
                    execTurn(new Vector3(1, 0, 0), -delta, delta);
                    break;
                case 4:
                    execTurn(new Vector3(0, 0, 1), -delta, delta);
                    break;
                case 5:
                    execTurn(new Vector3(0, 1, 0), delta, -delta);
                    break;
            }//end switch currentTurnName
        }//end if angle<abs(turnDegree)

        else  //do some reset jobs
        {
            switch (currentTurnName)
            {
                case 0:
                    rstCurrentFace(upFace);
                    rstAdjBlocks(backFace, leftFace, frontFace, rightFace, upFaceAdjRstIdx);
                    break;
                case 1:
                    rstCurrentFace(leftFace);
                    rstAdjBlocks(upFace, backFace, downFace, frontFace, leftFaceAdjRstIdx);
                    break;
                case 2:
                    rstCurrentFace(frontFace);
                    rstAdjBlocks(upFace, leftFace, downFace, rightFace, frontFaceAdjRstIdx);
                    break;
                case 3:
                    rstCurrentFace(rightFace);
                    rstAdjBlocks(upFace, frontFace, downFace, backFace, rightFaceAdjRstIdx);
                    break;
                case 4:
                    rstCurrentFace(backFace);
                    rstAdjBlocks(upFace, rightFace, downFace, leftFace, backFaceAdjRstIdx);
                    break;
                case 5:
                    rstCurrentFace(downFace);
                    rstAdjBlocks(frontFace, leftFace, backFace, rightFace, downFaceAdjRstIdx);
                    break;
            }//end switch currentTurnName

            sumAngle = .0f;
            turnFinishFlag = true;
            GameObject.Destroy(currentTurn);
        }//end if angle<abs(turnDegree)
    }
}
