using UnityEngine;
using System.Collections;

[System.Serializable]
// TODO: Wait for this - will work in 3.3
//import System.Collections.Generic;
[UnityEngine.RequireComponent(typeof(Collider))]
public partial class PatrolRoute : MonoBehaviour
{
    public bool pingPong;
    // TODO: In Unity 3.3 remove the System.Collections.Generic and impoprt the namespace instead
    public System.Collections.Generic.List<PatrolPoint> patrolPoints;
    private System.Collections.Generic.List<GameObject> activePatrollers;
    public virtual void Register(GameObject go)
    {
        this.activePatrollers.Add(go);
    }

    public virtual void UnRegister(GameObject go)
    {
        this.activePatrollers.Remove(go);
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (this.activePatrollers.Contains(other.gameObject))
        {
            AI ai = other.gameObject.GetComponentInChildren<AI>();
            if (ai)
            {
                ai.OnEnterInterestArea();
            }
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (this.activePatrollers.Contains(other.gameObject))
        {
            AI ai = other.gameObject.GetComponentInChildren<AI>();
            if (ai)
            {
                ai.OnExitInterestArea();
            }
        }
    }

    public virtual int GetClosestPatrolPoint(Vector3 pos)
    {
        if (this.patrolPoints.Count == 0)
        {
            return 0;
        }
        float shortestDist = Mathf.Infinity;
        int shortestIndex = 0;
        int i = 0;
        while (i < this.patrolPoints.Count)
        {
            this.patrolPoints[i].position = this.patrolPoints[i].transform.position;
            float dist = (this.patrolPoints[i].position - pos).sqrMagnitude;
            if (dist < shortestDist)
            {
                shortestDist = dist;
                shortestIndex = i;
            }
            i++;
        }
        // If going towards the closest point makes us go in the wrong direction,
        // choose the next point instead.
        if (!this.pingPong || (shortestIndex < (this.patrolPoints.Count - 1)))
        {
            int nextIndex = (shortestIndex + 1) % this.patrolPoints.Count;
            float angle = Vector3.Angle(this.patrolPoints[nextIndex].position - this.patrolPoints[shortestIndex].position, this.patrolPoints[shortestIndex].position - pos);
            if (angle > 120)
            {
                shortestIndex = nextIndex;
            }
        }
        return shortestIndex;
    }

    public virtual void OnDrawGizmos()
    {
        if (this.patrolPoints.Count == 0)
        {
            return;
        }
        Gizmos.color = new Color(0.5f, 0.5f, 1f);
        Vector3 lastPoint = this.patrolPoints[0].transform.position;
        int loopCount = this.patrolPoints.Count;
        if (this.pingPong)
        {
            loopCount--;
        }
        int i = 0;
        while (i < loopCount)
        {
            if (!this.patrolPoints[i])
            {
                break;
            }
            Vector3 newPoint = this.patrolPoints[(i + 1) % this.patrolPoints.Count].transform.position;
            Gizmos.DrawLine(lastPoint, newPoint);
            lastPoint = newPoint;
            i++;
        }
    }

    public virtual int GetIndexOfPatrolPoint(PatrolPoint point)
    {
        int i = 0;
        while (i < this.patrolPoints.Count)
        {
            if (this.patrolPoints[i] == point)
            {
                return i;
            }
            i++;
        }
        return -1;
    }

    public virtual GameObject InsertPatrolPointAt(int index)
    {
        GameObject go = new GameObject("PatrolPoint", new System.Type[] {typeof(PatrolPoint)});
        go.transform.parent = this.transform;
        int count = this.patrolPoints.Count;
        if (count == 0)
        {
            go.transform.localPosition = Vector3.zero;
            this.patrolPoints.Add(go.GetComponent<PatrolPoint>());
        }
        else
        {
            if ((!this.pingPong || ((index > 0) && (index < count))) || (count < 2))
            {
                index = index % count;
                int prevIndex = index - 1;
                if (prevIndex < 0)
                {
                    prevIndex = prevIndex + count;
                }
                go.transform.position = (this.patrolPoints[prevIndex].transform.position + this.patrolPoints[index].transform.position) * 0.5f;
            }
            else
            {
                if (index == 0)
                {
                    go.transform.position = (this.patrolPoints[0].transform.position * 2) - this.patrolPoints[1].transform.position;
                }
                else
                {
                    go.transform.position = (this.patrolPoints[count - 1].transform.position * 2) - this.patrolPoints[count - 2].transform.position;
                }
            }
            this.patrolPoints.Insert(index, go.GetComponent<PatrolPoint>());
        }
        return go;
    }

    public virtual void RemovePatrolPointAt(int index)
    {
        GameObject go = this.patrolPoints[index].gameObject;
        this.patrolPoints.RemoveAt(index);
        UnityEngine.Object.DestroyImmediate(go);
    }

    public PatrolRoute()
    {
        this.patrolPoints = new System.Collections.Generic.List<PatrolPoint>();
        this.activePatrollers = new System.Collections.Generic.List<GameObject>();
    }

}