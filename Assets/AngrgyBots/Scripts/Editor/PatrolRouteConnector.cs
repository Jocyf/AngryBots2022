using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
public partial class PatrolRouteConnector : MonoBehaviour
{
    //import UnityEditor;
    // MenuItem adds a menu item in the GameObject menu
    // and executes the following function when clicked
    [UnityEditor.MenuItem("Tools/Assign Closest Patrol Routes")]
    public static void AssignPatrolRoutes()
    {
        PatrolPoint[] points = (PatrolPoint[]) UnityEngine.Object.FindObjectsOfType(typeof(PatrolPoint));
        PatrolMoveController[] patrollers = (PatrolMoveController[]) UnityEngine.Object.FindObjectsOfType(typeof(PatrolMoveController));
        int connected = 0;
        foreach (PatrolMoveController patroller in patrollers)
        {
            float closestDist = Mathf.Infinity;
            PatrolPoint closestPoint = null;
            foreach (PatrolPoint point in points)
            {
                float dist = (patroller.transform.position - point.transform.position).magnitude;
                if (dist < closestDist)
                {
                    closestPoint = point;
                    closestDist = dist;
                }
            }
            if (!(closestDist == null))
            {
                patroller.patrolRoute = closestPoint.transform.parent.GetComponent<PatrolRoute>();
                connected++;
            }
        }
        Debug.Log(((("Successfully connected routes to " + connected) + " out of ") + patrollers.Length) + " patrollers.");
    }

}