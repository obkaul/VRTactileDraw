/// Credit setchi (https://github.com/setchi)
/// Sourced from - https://github.com/setchi/FancyScrollView

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts.Layout
{

	public class ScrollPositionController : UIBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		#region Sub-Classes
		[System.Serializable]
		public class UpdatePositionEvent : UnityEvent<float> { }

		[System.Serializable]
		public class ItemSelectedEvent : UnityEvent<int> { }
		#endregion

		[Serializable]
		private struct Snap
		{
			public bool Enable;
			public float VelocityThreshold;
			public float Duration;
		}

		private enum ScrollDirection
		{
			Vertical,
			Horizontal,
		}

		private enum MovementType
		{
			Unrestricted = ScrollRect.MovementType.Unrestricted,
			Elastic = ScrollRect.MovementType.Elastic,
			Clamped = ScrollRect.MovementType.Clamped
		}

		[SerializeField] private RectTransform viewport;
		[SerializeField] private ScrollDirection directionOfRecognize = ScrollDirection.Vertical;
		[SerializeField] private MovementType movementType = MovementType.Elastic;
		[SerializeField] private float elasticity = 0.1f;
		[SerializeField] private float scrollSensitivity = 1f;
		[SerializeField] private bool inertia = true;
		[SerializeField, Tooltip("Only used when inertia is enabled")]
		private float decelerationRate = 0.03f;
		[SerializeField, Tooltip("Only used when inertia is enabled")]
		private Snap snap = new Snap { Enable = true, VelocityThreshold = 0.5f, Duration = 0.3f };
		[SerializeField] private int dataCount;

		#region Events
		[Tooltip("Event that fires when the position of an item changes")]
		public UpdatePositionEvent OnUpdatePosition;

		[Tooltip("Event that fires when an item is selected/focused")]
		public ItemSelectedEvent OnItemSelected;
		#endregion

		private Vector2 pointerStartLocalPosition;
		private float dragStartScrollPosition;
		private float currentScrollPosition;
		private bool dragging;

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			pointerStartLocalPosition = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				viewport,
				eventData.position,
				eventData.pressEventCamera,
				out pointerStartLocalPosition);

			dragStartScrollPosition = currentScrollPosition;
			dragging = true;
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			if (!dragging)
			{
				return;
			}

			Vector2 localCursor;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
					viewport,
					eventData.position,
					eventData.pressEventCamera,
					out localCursor))
			{
				return;
			}

			var pointerDelta = localCursor - pointerStartLocalPosition;
			var position = (directionOfRecognize == ScrollDirection.Horizontal ? -pointerDelta.x : pointerDelta.y)
						   / GetViewportSize()
						   * scrollSensitivity
						   + dragStartScrollPosition;

			var offset = CalculateOffset(position);
			position += offset;

			if (movementType == MovementType.Elastic)
			{
				if (offset != 0)
				{
					position -= RubberDelta(offset, scrollSensitivity);
				}
			}
			UpdatePosition(position);
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			dragging = false;
		}

		private float GetViewportSize()
		{
			return directionOfRecognize == ScrollDirection.Horizontal
				? viewport.rect.size.x
				: viewport.rect.size.y;
		}

		private float CalculateOffset(float position)
		{
			if (movementType == MovementType.Unrestricted)
			{
				return 0;
			}
			if (position < 0)
			{
				return -position;
			}
			if (position > dataCount - 1)
			{
				return (dataCount - 1) - position;
			}
			return 0f;
		}

		private void UpdatePosition(float position)
		{
			currentScrollPosition = position;

			if (OnUpdatePosition != null)
			{
				OnUpdatePosition.Invoke(currentScrollPosition);
			}
		}

		private float RubberDelta(float overStretching, float viewSize)
		{
			return (1 - (1 / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1))) * viewSize * Mathf.Sign(overStretching);
		}

		//public void OnUpdatePosition(Action<float> onUpdatePosition)
		//{
		//    this.onUpdatePosition = onUpdatePosition;
		//}

		public void SetDataCount(int dataCont)
		{
			this.dataCount = dataCont;
		}

		private float velocity;
		private float prevScrollPosition;

		private bool autoScrolling;
		private float autoScrollDuration;
		private float autoScrollStartTime;
		private float autoScrollPosition;

		private void Update()
		{
			var deltaTime = Time.unscaledDeltaTime;
			var offset = CalculateOffset(currentScrollPosition);

			if (autoScrolling)
			{
				var alpha = Mathf.Clamp01((Time.unscaledTime - autoScrollStartTime) / Mathf.Max(autoScrollDuration, float.Epsilon));
				var position = Mathf.Lerp(dragStartScrollPosition, autoScrollPosition, EaseInOutCubic(0, 1, alpha));
				UpdatePosition(position);

				if (Mathf.Approximately(alpha, 1f))
				{
					autoScrolling = false;
					// Auto scrolling is completed, get the item's index and firing OnItemSelected event.
					if(OnItemSelected != null)
					{
						OnItemSelected.Invoke(Mathf.RoundToInt(GetLoopPosition(autoScrollPosition, dataCount)));
					}
				}
			}
			else if (!dragging && (offset != 0 || velocity != 0))
			{
				var position = currentScrollPosition;
				// Apply spring physics if movement is elastic and content has an offset from the view.
				if (movementType == MovementType.Elastic && offset != 0)
				{
					var speed = velocity;
					position = Mathf.SmoothDamp(currentScrollPosition, currentScrollPosition + offset, ref speed, elasticity, Mathf.Infinity, deltaTime);
					velocity = speed;
				}
				// Else move content according to velocity with deceleration applied.
				else if (inertia)
				{
					velocity *= Mathf.Pow(decelerationRate, deltaTime);
					if (Mathf.Abs(velocity) < 0.001f)
						velocity = 0;
					position += velocity * deltaTime;

					if (snap.Enable && Mathf.Abs(velocity) < snap.VelocityThreshold)
					{
						ScrollTo(Mathf.RoundToInt(currentScrollPosition), snap.Duration);
					}
				}
				// If we have neither elaticity or friction, there shouldn't be any velocity.
				else
				{
					velocity = 0;
				}

				if (velocity != 0)
				{
					if (movementType == MovementType.Clamped)
					{
						offset = CalculateOffset(position);
						position += offset;
					}
					UpdatePosition(position);
				}
			}

			if (!autoScrolling && dragging && inertia)
			{
				var newVelocity = (currentScrollPosition - prevScrollPosition) / deltaTime;
				velocity = Mathf.Lerp(velocity, newVelocity, deltaTime * 10f);
			}

			if (currentScrollPosition != prevScrollPosition)
			{
				prevScrollPosition = currentScrollPosition;
			}
		}

		public void ScrollTo(int index, float duration)
		{
			velocity = 0;
			autoScrolling = true;
			autoScrollDuration = duration;
			autoScrollStartTime = Time.unscaledTime;
			dragStartScrollPosition = currentScrollPosition;

			autoScrollPosition = movementType == MovementType.Unrestricted
				? CalculateClosestPosition(index)
				: index;
		}

		private float CalculateClosestPosition(int index)
		{
			var diff = GetLoopPosition(index, dataCount)
					   - GetLoopPosition(currentScrollPosition, dataCount);

			if (Mathf.Abs(diff) > dataCount * 0.5f)
			{
				diff = Mathf.Sign(-diff) * (dataCount - Mathf.Abs(diff));
			}
			return diff + currentScrollPosition;
		}

		private float GetLoopPosition(float position, int length)
		{
			if (position < 0)
			{
				position = (length - 1) + (position + 1) % length;
			}
			else if (position > length - 1)
			{
				position = position % length;
			}
			return position;
		}

		private float EaseInOutCubic(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * 0.5f * value * value * value + start;
			}
			value -= 2f;
			return end * 0.5f * (value * value * value + 2f) + start;
		}
	}
}