using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
[UnityEditor.CustomEditor(typeof(PatrolRoute))]
public class PatrolRouteEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PatrolRoute route = this.target as PatrolRoute;
        GUILayout.Label(route.patrolPoints.Count + " Patrol Points in Route", new GUILayoutOption[] {});
        route.pingPong = EditorGUILayout.Toggle("Ping Pong", route.pingPong, new GUILayoutOption[] {});
        if (GUI.changed)
        {
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Reverse Direction", new GUILayoutOption[] {}))
        {
            route.patrolPoints.Reverse();
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Add Patrol Point", new GUILayoutOption[] {}))
        {
            Selection.activeGameObject = route.InsertPatrolPointAt(route.patrolPoints.Count);
        }
    }

    public virtual void OnSceneGUI()
    {
        PatrolRoute route = this.target as PatrolRoute;
        PatrolRouteEditor.DrawPatrolRoute(route);
    }

    public static void DrawPatrolRoute(PatrolRoute route)
    {
        if (route.patrolPoints.Count == 0)
        {
            return;
        }
        Vector3 lastPoint = route.patrolPoints[0].transform.position;
        int loopCount = route.patrolPoints.Count;
        if (route.pingPong)
        {
            loopCount--;
        }
        int i = 0;
        while (i < loopCount)
        {
            if (!route.patrolPoints[i])
            {
                break;
            }
            Vector3 newPoint = route.patrolPoints[(i + 1) % route.patrolPoints.Count].transform.position;
            if (newPoint != lastPoint)
            {
                Handles.color = new Color(0.5f, 0.5f, 1f);
                PatrolRouteEditor.DrawPatrolArrow(lastPoint, newPoint);
                if (route.pingPong)
                {
                    Handles.color = new Color(1f, 1f, 1f, 0.2f);
                    PatrolRouteEditor.DrawPatrolArrow(newPoint, lastPoint);
                }
            }
            lastPoint = newPoint;
            i++;
        }
    }

    public static void DrawPatrolArrow(Vector3 a, Vector3 b)
    {
        Quaternion directionRotation = Quaternion.LookRotation(b - a);
        Handles.ConeHandleCap (0, ((a + b) * 0.5f) - ((directionRotation * Vector3.forward) * 0.5f), directionRotation, 0.7f, EventType.Layout);
        //Handles.ConeCap(0, ((a + b) * 0.5f) - ((directionRotation * Vector3.forward) * 0.5f), directionRotation, 0.7f);
    }

}