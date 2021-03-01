﻿using System.Collections.Generic;
using Scripts.Layout;
using UnityEngine;

namespace Examples.FancyScrollView._03_InfiniteScroll
{
    public class Example03ScrollView : FancyScrollView<Example03CellDto, Example03ScrollViewContext>
    {
        [SerializeField] private ScrollPositionController scrollPositionController;

        private new void Awake()
        {
            scrollPositionController.OnUpdatePosition.AddListener(UpdatePosition);

            // Add OnItemSelected event listener
            scrollPositionController.OnItemSelected.AddListener(CellSelected);

            SetContext(new Example03ScrollViewContext { OnPressedCell = OnPressedCell });
            base.Awake();
        }

        public void UpdateData(List<Example03CellDto> data)
        {
            cellData = data;
            scrollPositionController.SetDataCount(cellData.Count);
            UpdateContents();
        }

        private void OnPressedCell(Example03ScrollViewCell cell)
        {
            scrollPositionController.ScrollTo(cell.DataIndex, 0.4f);
            context.SelectedIndex = cell.DataIndex;
            UpdateContents();
        }

        // An event triggered when a cell is selected.
        private void CellSelected(int cellIndex)
        {
            // Update context.SelectedIndex and call UpdateContents for updating cell's content.
            context.SelectedIndex = cellIndex;
            UpdateContents();
        }
    }
}
