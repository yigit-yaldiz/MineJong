using System.Collections.Generic;
using UnityEngine;

namespace GridAsset
{
    public class GridPath
    {
        public static bool IsReachableTraverse(GridCell startCell, GridCell endCell, GridObject gridObject = null)
        {
            return GetPath(true, startCell, endCell, out GridCell[] path, gridObject);
        }
        public static bool IsReachableNonTraverse(GridCell startCell, GridCell endCell, GridObject gridObject = null)
        {
            return GetPath(false, startCell, endCell, out GridCell[] path, gridObject);
        }
        public static bool GetPathTraverse(GridCell startCell, GridCell endCell, out GridCell[] path, GridObject gridObject = null)
        {
            return GetPath(true, startCell, endCell, out path, gridObject);
        }
        public static bool GetPathNonTraverse(GridCell startCell, GridCell endCell, out GridCell[] path, GridObject gridObject = null)
        {
            return GetPath(false, startCell, endCell, out path, gridObject);
        }
        private static bool GetPath(bool traverse, GridCell startCell, GridCell endCell, out GridCell[] path, GridObject gridObject = null)
        {
            List<GridCell> openCells = new List<GridCell>();
            List<GridCell> closedCells = new List<GridCell>();
            openCells.Add(startCell);

            if (gridObject != null)
            {
                for (int i = 0; i < gridObject.AdditionalDimensions.Length; i++)
                {
                    GridCell offsetCell = endCell.CellWithOffset(gridObject.AdditionalDimensions[i]);

                    if (offsetCell == null || (!offsetCell.Available && !gridObject.IsUsingCell(offsetCell)))
                    {
                        path = null;
                        return false;
                    }
                }
            }

            while (openCells.Count > 0)
            {
                GridCell currentCell = openCells[0];

                for (int i = 1; i < openCells.Count; i++)
                {
                    GridCell usingCell = openCells[i];
                    if (usingCell.FCost < currentCell.FCost ||
                        usingCell.FCost == currentCell.FCost &&
                        usingCell.HCost < currentCell.HCost)
                        currentCell = usingCell;
                }

                openCells.Remove(currentCell);
                closedCells.Add(currentCell);

                if (currentCell == endCell)
                {
                    path = RetracePath(startCell, endCell);
                    return true;
                }

                foreach (GridCell neighbour in currentCell.GetNeighbours(traverse))
                {
                    if (gridObject == null)
                    {
                        if (!neighbour.Available || closedCells.Contains(neighbour))
                            continue;
                    }
                    else
                    {
                        if ((!neighbour.Available && !gridObject.IsUsingCell(neighbour)) || (closedCells.Contains(neighbour)))
                            continue;

                        bool c = false;
                        for (int i = 0; i < gridObject.AdditionalDimensions.Length; i++)
                        {
                            GridCell offsetCell = neighbour.CellWithOffset(gridObject.AdditionalDimensions[i]);

                            if (offsetCell == null || (!offsetCell.Available && !gridObject.IsUsingCell(offsetCell)))
                            {
                                c = true;
                                break;
                            }
                        }
                        if (c)
                            continue;
                    }

                    int newMovementCost = currentCell.GCost + GetCellDistance(currentCell, neighbour);
                    if (newMovementCost < neighbour.GCost || !openCells.Contains(neighbour))
                    {
                        neighbour.GCost = newMovementCost;
                        neighbour.HCost = GetCellDistance(neighbour, endCell);
                        neighbour.Parent = currentCell;

                        if (!openCells.Contains(neighbour))
                            openCells.Add(neighbour);
                    }
                }
            }

            path = null;
            return false;
        }

        private static GridCell[] RetracePath(GridCell startCell, GridCell endCell)
        {
            List<GridCell> path = new List<GridCell>();
            path.Add(endCell);

            GridCell currentCell = endCell;

            while (currentCell != startCell)
            {
                path.Add(currentCell);
                currentCell = currentCell.Parent;
            }
            path.Add(startCell);
            path.Reverse();

            return path.ToArray();
        }

        private static int GetCellDistance(GridCell cell1, GridCell cell2)
        {
            int distX = Mathf.Abs(cell1.Coords.x - cell2.Coords.x);
            int distY = Mathf.Abs(cell1.Coords.y - cell2.Coords.y);

            if (distX > distY)
                return 14 * distY + 10 * (distX - distY);

            return 14 * distX + 10 * (distY - distX);
        }
    }
}