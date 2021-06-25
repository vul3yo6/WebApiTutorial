using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AisysWinApp.Models.Structures
{
    internal class VisionResult
    {
        public string Name { get; private set; }
        public double Score { get; private set; }

        public VisionResult(string name, double score)
        {
            Name = name;
            Score = score;
        }

        public override string ToString()
        {
            //return $"{{{Name}:{Score:0.0000}}}";
            return $"{Score:0.0000}";
        }
    }

    internal class VisionResultList : IEnumerable<VisionResult>
    {
        private List<VisionResult> Results { get; set; } = new List<VisionResult>();

        public void Add(VisionResult visionResult)
        {
            Results.Add(visionResult);
        }

        public override string ToString()
        {
            return $"[{string.Join(",", Results)}]";
        }

        #region IEnumerable

        public IEnumerator<VisionResult> GetEnumerator()
        {
            return Results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        } 

        #endregion
    }
}
