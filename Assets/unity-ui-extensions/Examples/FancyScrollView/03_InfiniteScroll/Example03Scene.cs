using System.Linq;
using UnityEngine;

namespace Examples.FancyScrollView._03_InfiniteScroll
{
    public class Example03Scene : MonoBehaviour
    {
        [SerializeField] private Example03ScrollView scrollView;

        private void Start()
        {
            var cellData = Enumerable.Range(0, 20)
                .Select(i => new Example03CellDto { Message = "Cell " + i })
                .ToList();

            scrollView.UpdateData(cellData);
        }
    }
}
