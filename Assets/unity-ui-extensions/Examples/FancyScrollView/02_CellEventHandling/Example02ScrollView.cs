using System.Collections.Generic;
using Scripts.Layout;
using UnityEngine;

namespace Examples.FancyScrollView._02_CellEventHandling
{
	public class Example02ScrollView : FancyScrollView<Example02CellDto, Example02ScrollViewContext>
	{
		[SerializeField] private ScrollPositionController scrollPositionController;

		private new void Awake()
		{
			scrollPositionController.OnUpdatePosition.AddListener(UpdatePosition);
			// Add OnItemSelected event listener
			scrollPositionController.OnItemSelected.AddListener(CellSelected);

			SetContext(new Example02ScrollViewContext { OnPressedCell = OnPressedCell });
			base.Awake();
		}

		public void UpdateData(List<Example02CellDto> data)
		{
			cellData = data;
			scrollPositionController.SetDataCount(cellData.Count);
			UpdateContents();
		}

		private void OnPressedCell(Example02ScrollViewCell cell)
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
