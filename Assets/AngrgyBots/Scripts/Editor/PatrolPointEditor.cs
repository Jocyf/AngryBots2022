using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
[UnityEditor.CustomEditor(typeof(PatrolPoint))]
public class PatrolPointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PatrolPoint point = this.target as PatrolPoint;
        PatrolRoute route = point.transform.parent.GetComponent<PatrolRoute>();
        int thisIndex = route.GetIndexOfPatrolPoint(point);
        if (GUILayout.Button("Remove This Patrol Point", new GUILayoutOption[] {}))
        {
            route.RemovePatrolPointAt(thisIndex);
            int newSelectionIndex = Mathf.Clamp(thisIndex, 0, route.patrolPoints.Count - 1);
            Selection.activeGameObject = route.patrolPoints[newSelectionIndex].gameObject;
        }
        if (GUILayout.Button("Insert Patrol Point Before", new GUILayoutOption[] {}))
        {
            Selection.activeGameObject = route.InsertPatrolPointAt(thisIndex);
        }
        if (GUILayout.Button("Insert Patrol Point After", new GUILayoutOption[] {}))
        {
            Selection.activeGameObject = route.InsertPatrolPointAt(thisIndex + 1);
        }
    }

    public virtual void OnSceneGUI()
    {
        PatrolPoint point = this.target as PatrolPoint;
        PatrolRoute route = point.transform.parent.GetComponent<PatrolRoute>();
        PatrolRouteEditor.DrawPatrolRoute(route);
    }

}