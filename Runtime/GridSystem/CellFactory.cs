using UnityEngine;
using Lucky4u.GridSystem;

public class CellFactory
{
    public Cell SpawnCell(GameObject cellPrefab, Transform parent, Vector2 pos, GridCoordinateSpace gridCoordinateSpace)
    {
        Cell cell = GameObject.Instantiate(cellPrefab, parent).GetComponent<Cell>();
        if(gridCoordinateSpace == GridCoordinateSpace.TWO_DIMENSIONAL)
        {
            cell.transform.localPosition = pos;
        }
        else
        {
            Vector3 newPos = new Vector3();
            newPos.x = pos.x;
            newPos.y = 0;
            newPos.z = pos.y;
            cell.transform.localPosition = newPos;
        }
        cell.name = "cell[" + pos.x + "," + pos.y + "]";
        return cell;
    }
}
