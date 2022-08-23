using UnityEngine;
using System.Collections.Generic;
using System;

namespace Lucky4u.GridSystem
{
    public class Cell : MonoBehaviour
    {
        public static Action<Cell> OnCellClick;

        [SerializeField]
        private bool isEmpty = true;

        [SerializeField]
        private bool isOpen;

        [SerializeField]
        private bool isCenterCell;

        private GridUnit occupiedGridUnit = null;

        private List<Cell> neighbourCells;

        private int xIndex, yIndex;

        public List<Cell> NeighbourCells
        {
            get
            {
                return neighbourCells;
            }
        }
        public int XIndex { get => xIndex; private set => xIndex = value; }
        public int YIndex { get => yIndex; private set => yIndex = value; }
        public GridUnit OccupiedGridUnit { get => occupiedGridUnit; private set => occupiedGridUnit = value; }
        public bool IsEmpty { get => isEmpty; private set => isEmpty = value; }
        public bool IsOpen { get => isOpen; set => isOpen = value; }
        public bool IsCenter { get => isCenterCell; set => isCenterCell = value; }

        public void SetIndices(int xIn, int yIn)
        {
            this.XIndex = xIn;
            this.YIndex = yIn;
        }

        public void AddToNeigbhourCells(Cell cellIn)
        {
            if (cellIn == null)
                return;
            if (neighbourCells == null)
                neighbourCells = new List<Cell>();
            if (!neighbourCells.Contains(cellIn))
            {
                neighbourCells.Add(cellIn);
            }
        }

        public void Occupy(GridUnit gridUnit)
        {
            IsEmpty = false;
            this.OccupiedGridUnit = gridUnit;
            OccupiedGridUnit.transform.SetParent(transform);
            gridUnit.transform.localPosition = Vector3.zero;
        }

        public void Vacate(bool doNotDestroyUnit = false)
        {
            IsEmpty = true;
            if (!doNotDestroyUnit)
                Destroy(this.OccupiedGridUnit.gameObject);
            this.OccupiedGridUnit = null;
        }

        public void AddCellVariantGraphics(GameObject prefab)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
        }

        void OnMouseDown()
        {
            OnCellClick?.Invoke(this);
        }
    }
}