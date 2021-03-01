using System.Linq;
using UnityEngine;

namespace Examples.FancyScrollView._01_Basic
{
    public class Example01Scene : MonoBehaviour
    {
        [SerializeField] private Example01ScrollView scrollView;

        private void Start()
        {
            var cellData = Enumerable.Range(0, 20)
                .Select(i => new Example01CellDto { Message = "Cell " + i })
                .ToList();

            scrollView.UpdateData(cellData);
        }
    }
}
