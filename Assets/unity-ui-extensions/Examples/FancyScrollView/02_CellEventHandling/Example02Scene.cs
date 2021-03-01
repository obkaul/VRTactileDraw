﻿using System.Linq;
using UnityEngine;

namespace Examples.FancyScrollView._02_CellEventHandling
{
    public class Example02Scene : MonoBehaviour
    {
        [SerializeField] private Example02ScrollView scrollView;

        private void Start()
        {
            var cellData = Enumerable.Range(0, 20)
                .Select(i => new Example02CellDto { Message = "Cell " + i })
                .ToList();

            scrollView.UpdateData(cellData);
        }
    }
}
