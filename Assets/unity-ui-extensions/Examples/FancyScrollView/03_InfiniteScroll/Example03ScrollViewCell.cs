﻿using Scripts.Layout;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.FancyScrollView._03_InfiniteScroll
{
    public class Example03ScrollViewCell
        : FancyScrollViewCell<Example03CellDto, Example03ScrollViewContext>
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Text message;
        [SerializeField] private Image image;
        [SerializeField] private Button button;

        private readonly int scrollTriggerHash = Animator.StringToHash("scroll");
        private Example03ScrollViewContext context;

        private void Start()
        {
            var rectTransform = transform as RectTransform;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchoredPosition3D = Vector3.zero;
            UpdatePosition(0);

            button.onClick.AddListener(OnPressedCell);
        }

        /// <summary>
        /// コンテキストを設定します
        /// </summary>
        /// <param name="context"></param>
        public override void SetContext(Example03ScrollViewContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// セルの内容を更新します
        /// </summary>
        /// <param name="itemData"></param>
        public override void UpdateContent(Example03CellDto itemData)
        {
            message.text = itemData.Message;

            if (context != null)
            {
                var isSelected = context.SelectedIndex == DataIndex;
                image.color = isSelected
                    ? new Color32(0, 255, 255, 100)
                    : new Color32(255, 255, 255, 77);
            }
        }

        /// <summary>
        /// セルの位置を更新します
        /// </summary>
        /// <param name="position"></param>
        public override void UpdatePosition(float position)
        {
            animator.Play(scrollTriggerHash, -1, position);
            animator.speed = 0;
        }

        public void OnPressedCell()
        {
            if (context != null)
            {
                context.OnPressedCell(this);
            }
        }
    }
}
