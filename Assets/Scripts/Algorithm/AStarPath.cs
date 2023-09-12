using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPath
{
    [Header("AStarPath Variables")]
    private List<AStarNode> openList = new List<AStarNode>();
    private List<AStarNode> closedList = new List<AStarNode>();
    private readonly List<Cell> path = new List<Cell>();

    private Cell target;

    private MainManager manager;

    public List<Cell> CalculatePath(Cell start, Cell target)
    {
        this.target = target;

        openList.Clear();
        closedList.Clear();
        path.Clear();


        openList.Add(new AStarNode(start));


        while (openList.Count > 0)
        {
            int nodeIndex = 0;
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].f < openList[nodeIndex].f)
                    nodeIndex = i;
            }

            AStarNode currentNode = openList[nodeIndex];

            openList.Remove(currentNode);
            closedList.Add(currentNode);


            if(currentNode.cell == target)
            {
                while (currentNode != null)
                {
                    path.Add(currentNode.cell);
                    currentNode = currentNode.parentNode;
                }
                path.Reverse();
                return path;
            }

            InvestigateNeighbors(currentNode);          
        }

        return null;
    }

    private void InvestigateNeighbors(AStarNode node)
    {
        List<Cell> neighborList = node.cell.GetNeighbors();

        for (int i = 0; i < neighborList.Count; i++)
        {
            if (neighborList[i] == target)
            {
                openList.Add(new AStarNode(neighborList[i], node));
                return;
            }


            if (neighborList[i].blocked)
                continue;

            Vector2Int currentPos = node.cell.GetPosition();
            Vector2Int neighborPosition = neighborList[i].GetPosition();

            int g = CalculateG(neighborPosition, currentPos, node.g);
            int h = CalculateH(neighborPosition, target.GetPosition());
            int f = g + h;

            bool handled = false;
            for (int j = 0; j < openList.Count; j++)
            {
                if (openList[j].cell == neighborList[i])
                {
                    if (f < openList[j].f)
                    {
                        openList[j].f = f;
                        openList[j].g = g;
                        openList[j].h = h;
                        openList[j].parentNode = node;
                    }
                    handled = true;
                    break;
                }
            }

            if (!handled)
            {
                for (int j = 0; j < closedList.Count; j++)
                    if (closedList[j].cell.GetPosition() == neighborList[i].GetPosition())
                    {
                        if (f >= closedList[j].f)
                            handled = true;
                        break;
                    }
            }

            
            if (!handled)
                openList.Add(new AStarNode(neighborList[i], node, h, g, f));      
        }
    }


    private int CalculateG(Vector2Int current, Vector2Int goal, int g)
    {
        if (current == goal)
            return g;

        int dx = Mathf.Clamp(goal.x - current.x, -1, 1);
        int dy = Mathf.Clamp(goal.y - current.y, -1, 1);

        current.x += dx;
        current.y += dy;

        return CalculateG(current, goal, g + dx != 0 && dy != 0 ? 14 : 10);
    }

    private int CalculateH(Vector2Int current, Vector2Int target)
    {
        return (Mathf.Abs(target.x - current.x) + Mathf.Abs(target.y - current.y)) * 10;
    }
}
