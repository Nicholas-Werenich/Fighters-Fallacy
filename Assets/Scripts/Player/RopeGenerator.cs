using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public class RopeGenerator : MonoBehaviour
{
    public Transform ropeHoldPosition;
    public Transform point;
    public GameObject ropePiece;
    private List<GameObject> ropePieces = new List<GameObject>();


    private float pixelsPerUnit = 0.0625f;
    private float totalPieces;
    private bool isXLimit;
    private float constraintDistance;
    private Vector2 ropePosition;

    private void Awake()
    {
        //Start from the top part
        GameObject startingPiece = Instantiate(ropePiece, point);
        startingPiece.GetComponent<HingeJoint2D>().connectedAnchor = point.position;
        ropePieces.Add(startingPiece);
        RopeBuilder();
    }

    private void RopeBuilder()
    {

        float distance = Vector2.Distance(ropeHoldPosition.transform.position, point.position);
        float angle = Vector2.Angle(ropePiece.transform.position, point.position);
        Debug.Log("Distance: " + distance);

        //Find the limiting side of the triangle
        if (Mathf.Abs(ropeHoldPosition.position.x) - Mathf.Abs(point.position.x) <= Mathf.Abs(ropeHoldPosition.position.y) - Mathf.Abs(point.position.y))
            constraintDistance = Mathf.Cos(angle) * distance;
        else
            constraintDistance = Mathf.Sin(angle) * distance;

        constraintDistance = Mathf.Abs(constraintDistance);
        Debug.Log("Constraint: " + constraintDistance);

        totalPieces = constraintDistance / pixelsPerUnit;

        Debug.Log("Pieces: " + totalPieces);

        ropePosition = isXLimit ? new Vector2(pixelsPerUnit / constraintDistance, pixelsPerUnit) : new Vector2(pixelsPerUnit, pixelsPerUnit / constraintDistance);
        for (int i = 0; i < totalPieces; i++) 
        {
            GenerateRopePiece(ropePieces.Last());
        }

        LastPiece(ropePieces.Last());
        
    }

    private Vector2 AbsVector(Vector2 vector)
    {
        return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }

    private void GenerateRopePiece(GameObject lastPiece)
    {
        GameObject currentPiece = Instantiate(ropePiece, (Vector2)lastPiece.transform.position - ropePosition, lastPiece.transform.rotation);
        currentPiece.GetComponent<HingeJoint2D>().connectedBody = lastPiece.GetComponent<Rigidbody2D>();
        ropePieces.Add(currentPiece);
    }

    private void LastPiece(GameObject lastPiece)
    {
        FixedJoint2D playerJoint = lastPiece.AddComponent<FixedJoint2D>();
        playerJoint.connectedBody = GameObject.Find("Player").GetComponent<Rigidbody2D>();

        
        DistanceJoint2D ropeDistance = lastPiece.AddComponent<DistanceJoint2D>();
        ropeDistance.connectedBody = ropePieces.First().GetComponent<Rigidbody2D>();
        ropeDistance.maxDistanceOnly = true;
        ropeDistance.autoConfigureDistance = false;
        ropeDistance.distance = Vector2.Distance(ropeHoldPosition.transform.position, point.position);
        
        
        lastPiece.GetComponent<Rigidbody2D>().gravityScale = 1;
    }

}
