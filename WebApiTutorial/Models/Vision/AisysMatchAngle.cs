using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AisysWinApp.Models
{
    internal class AisysMatchAngle : IDisposable
    {
        private IReadOnlyCollection<ArrowType> _arrowTypeList = (Enum.GetValues(typeof(ArrowType))).Cast<ArrowType>().ToList().AsReadOnly();
        //private AisysVision _aisys = new AisysVision();
        private AisysMatch _match = new AisysMatch();
        private double _threshold = 0.8d;

        public bool Learn(ArrowType arrowType, string imagePath)
        {
            string filePath = GetArrowModelFilePath(arrowType);

            _match.Learn(imagePath);
            return _match.SaveModel(filePath);
        }

        public bool Learn()
        {
            bool isSuccess = true;
            foreach (ArrowType item in _arrowTypeList)
            {
                string filePath = GetArrowModelFilePath(item);
                if (HasModelFile(filePath))
                {
                    isSuccess &= _match.LoadModel(filePath);
                    continue;
                }

                string imagePath = GetArrowModelImagePath(item);
                isSuccess &= Learn(item, imagePath);
            }
            
            return isSuccess;
        }

        public ArrowResult Match(string imagePath)
        {
            var dict = new Dictionary<ArrowType, MatchResult>();
            foreach (ArrowType item in _arrowTypeList)
            {
                dict[item] = new MatchResult();

                string filePath = GetArrowModelFilePath(item);
                if (File.Exists(filePath) == false)
                {
                    continue;
                }

                if (_match.LoadModel(filePath))
                {
                    dict[item] = _match.Match2(imagePath);
                }
            }

            ArrowResult result = null;
            foreach (var kvp in dict)
            {
                var matchResult = kvp.Value;

                if (matchResult.IsMatch == false)
                {
                    continue;
                }

                if (double.IsNaN(matchResult.Score))
                {
                    continue;
                }

                //if (result == null)
                //{
                //    result = new ArrowResult(kvp.Key, kvp.Value);
                //    continue;
                //}

                if (matchResult.Score >= _threshold && IsValidAndBetter(result, matchResult))
                {
                    result = new ArrowResult(kvp.Key, kvp.Value);
                }
            }

            return result;
        }

        private static bool IsValidAndBetter(ArrowResult result, MatchResult matchResult)
        {
            if (result == null)
            {
                return true;
            }

            return matchResult.Score > result.Score;
        }

        private bool HasModelFile(string filePath)
        {
            return File.Exists(filePath);
        }

        private static string GetArrowModelFilePath(ArrowType arrowType)
        {
            string filePath = string.Empty;
            switch (arrowType)
            {
                case ArrowType.A:
                    filePath = "a.model";
                    break;
                default:
                case ArrowType.B:
                    filePath = "b.model";
                    break;
                //case ArrowType.C:
                //    filePath = "c.model";
                //    break;
            }

            return Path.Combine(GetRootPath(), filePath);
        }

        private static string GetArrowModelImagePath(ArrowType arrowType)
        {
            string filePath = string.Empty;
            switch (arrowType)
            {
                case ArrowType.A:
                    filePath = "a.bmp";
                    break;
                default:
                case ArrowType.B:
                    filePath = "b.bmp";
                    break;
                //case ArrowType.C:
                //    filePath = "c.bmp";
                //    break;
            }

            return Path.Combine(GetRootPath(), filePath);
        }

        private static string GetRootPath()
        {
            string codeBase = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            return Path.GetDirectoryName(codeBase).Replace("file:\\", string.Empty);
        }

        #region IDisposable
        // Has Dispose already been called?
        bool disposed = false;
        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // Free any other managed objects here.
                //
                //_aisys.Dispose();
            }
            // Free any unmanaged objects here.
            //
            disposed = true;
        }
        ~AisysMatchAngle()
        {
            Dispose(false);
        }
        #endregion
    }

    internal enum ArrowType
    {
        A,
        B,
        //C,
    }

    internal class ArrowResult
    {
        public ArrowType Type { get; set; }
        public bool IsMatch { get; set; }
        public double Score { get; set; }
        public double Angle { get; set; }
        public double AngleIn180
        {
            get
            {
                if (Angle > 180)
                {
                    return Angle - 360;
                }

                return Angle;
            }
        }
        public double TrunAngleIn180
        {
            get
            {
                if (double.IsNaN(AngleIn180))
                {
                    return 0;
                }

                return 0 - AngleIn180;
            }
        }

        public ArrowResult(ArrowType type, MatchResult result) : this(type, result.IsMatch, result.Score, result.Angle)
        {
        }

        public ArrowResult(ArrowType type, bool isMatch, double score, double angle)
        {
            Type = type;
            IsMatch = isMatch;
            Score = score;
            Angle = angle;
        }
    }
}
