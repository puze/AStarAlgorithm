using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAlgorithm
{
    public class AstarNode
    {
        public int nodeID;
        public int parentNodeID;
        public float fScore;    // g+h
        public float gScore;    // current cost
        public float hScore;    // expectation cost

        public int GetNodeXValue()
        {
            return nodeID % NODE_MAP_MAX_X;
        }
        public int GetNodeYValue()
        {
            return nodeID / NODE_MAP_MAX_X;
        }
    }

    public class PriorityQueue : List<AstarNode>
    {
        private bool isCheckEnd = false;

        public new void Add(AstarNode item)
        {
            bool isInsertItem = false;
            for (int i = 0; i < Count; i++)
            {
                // F score가 같다면 최신 아이템이 우선순위
                if (this[i].fScore >= item.fScore)
                {
                    Insert(i, item);
                    isInsertItem = true;
                    return;
                }
            }
            if (!isInsertItem)
                base.Add(item);
            return;
        }

        public bool ContainNodeID(int id)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].nodeID == id)
                    return true;
            }
            return false;
        }

        public AstarNode FindNodeWithID(int id)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].nodeID == id)
                    return this[i];
            }
            return null;
        }

        public void SetCheckEnd(bool set)
        {
            isCheckEnd = set;
        }

        public bool GetCheckEnd()
        {
            return isCheckEnd;
        }
    }
    private const int NODE_MAP_MAX_X = 5;
    private const int NODE_MAP_MAX_Y = 7;
    private PriorityQueue openList = new PriorityQueue();
    private List<AstarNode> closeList = new List<AstarNode>();
    private bool[,] obstacleInfo = new bool[NODE_MAP_MAX_X, NODE_MAP_MAX_Y];

    public List<AstarNode> FindAStar(int x, int y)
    {
        AstarNode newNode = new AstarNode();
        newNode.nodeID = GetNodeId(x, y);
        newNode.parentNodeID = -1;
        newNode.gScore = 0;
        newNode.hScore = y;
        newNode.fScore = newNode.hScore;
        AddOpenList(newNode);
        while (!openList.GetCheckEnd() && openList.Count != 0)
        {
            AstarNode currentNode = new AstarNode();
            currentNode = openList[0];
            openList.Remove(currentNode);
            AddCloseList(currentNode);
        }

        if (openList.GetCheckEnd())
        {
            List<AstarNode> aStarPath = new List<AstarNode>();
            AstarNode currentNode = closeList[closeList.Count - 1];
            aStarPath.Add(currentNode);
            while (currentNode.parentNodeID != -1)
            {
                for (int i = 0; i < closeList.Count; i++)
                {
                    if (closeList[i].nodeID == currentNode.parentNodeID)
                    {
                        currentNode = closeList[i];
                        break;
                    }
                }
                aStarPath.Insert(0, currentNode);
            }
            return aStarPath;
        }
        else
        {
            Debug.Log("Fail path find");
            return null;    // path find fail
        }
    }

    private void AddCloseList(AstarNode node)
    {
        closeList.Add(node);
        // Left
        if (node.GetNodeXValue() > 0)
        {
            AstarNode newNode = new AstarNode();
            newNode.nodeID = node.nodeID - 1;
            newNode.parentNodeID = node.nodeID;
            newNode.gScore = node.gScore + 1;
            newNode.hScore = node.hScore;
            newNode.fScore = newNode.gScore + newNode.hScore;
            AddOpenList(newNode);
        }
        if (node.GetNodeYValue() > 0)
        {
            AstarNode newNode = new AstarNode();
            newNode.nodeID = node.nodeID - 5;
            newNode.parentNodeID = node.nodeID;
            newNode.gScore = node.gScore + 1;
            newNode.hScore = node.hScore - 1;
            newNode.fScore = newNode.gScore + newNode.hScore;
            AddOpenList(newNode);
        }
        if (node.GetNodeXValue() < NODE_MAP_MAX_X - 1)
        {
            AstarNode newNode = new AstarNode();
            newNode.nodeID = node.nodeID + 1;
            newNode.parentNodeID = node.nodeID;
            newNode.gScore = node.gScore + 1;
            newNode.hScore = node.hScore;
            newNode.fScore = newNode.gScore + newNode.hScore;
            AddOpenList(newNode);
        }
        if (node.GetNodeYValue() < NODE_MAP_MAX_Y - 1)
        {
            AstarNode newNode = new AstarNode();
            newNode.nodeID = node.nodeID + 5;
            newNode.parentNodeID = node.nodeID;
            newNode.gScore = node.gScore + 1;
            newNode.hScore = node.hScore + 1;
            newNode.fScore = newNode.gScore + newNode.hScore;
            AddOpenList(newNode);
        }
    }

    private void AddOpenList(AstarNode node)
    {
        // 목적지 도착
        if (node.GetNodeYValue() == 0)
        {
            openList.SetCheckEnd(true);
            closeList.Add(node);
        }
        // close list에 이미 있으면 return
        for (int i = 0; i < closeList.Count; i++)
        {
            if (closeList[i].nodeID == node.nodeID)
                return;
        }
        // 해당 위치에 폴리곤이 있으면 return;
        // Y offset 1
        if (node.GetNodeYValue() > 0 && node.GetNodeYValue() < 6 && obstacleInfo[node.GetNodeXValue(), node.GetNodeYValue()])
            return;

        if (openList.ContainNodeID(node.nodeID)) {            
            if (openList.FindNodeWithID(node.nodeID).gScore > node.gScore)
            {
                // Open list에 이미 존재한다면 parent node와 Gscore를 갱신
                openList.FindNodeWithID(node.nodeID).parentNodeID = node.nodeID;
                openList.FindNodeWithID(node.nodeID).gScore = node.gScore;
            }
        } else
        {
            // Open list에 존재하지않는다면 새로운 노드를 open list에 추가
            openList.Add(node);
        }

    }

    public void SetFieldInfo(bool[,] filedInfo)
    {
        obstacleInfo = filedInfo;
    }
    private int GetNodeId(int x, int y)
    {
        return 5 * y + x;
    }
}
