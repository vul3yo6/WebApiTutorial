using AxOvkBase;
using AxOvkPat;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AisysWinApp.Models
{
    internal class AisysMatch
    {
        private readonly bool _isSaveFile = false;  // 是否輸出 _axImage 和 _learnAxROI, _matchAxROI 載入的圖片

        private AxImageBW8 _learnAxImage = new AxImageBW8();
        private AxImageBW8 _matchAxImage = new AxImageBW8();
        private AxROIBW8 _learnAxROI = new AxROIBW8();
        private AxROIBW8 _matchAxROI = new AxROIBW8();
        private AxMatch _axMatch = new AxMatch();

        public void Learn(string filePath)
        {
            var bitmap = LoadImage(filePath);
            LoadLearnBitmap(bitmap);

            SetLearnRoiControls(0, 0, bitmap.Width, bitmap.Height, bitmap.RawFormat);
            Learn();

            // 輸出學習後的模型
            //_axMatch.LoadFile(@"C:\Users\kentseng\Desktop\test_images\s.pat");
            //_axMatch.SaveFile(@"C:\Users\kentseng\Desktop\test_images\s.pat");
        }

        public MatchResult Match2(string filePath)
        {
            bool isMatch = false;
            using (var bitmap = LoadImage(filePath))
            {
                LoadMatchBitmap(bitmap);

                SetMatchRoiControls(0, 0, bitmap.Width, bitmap.Height, bitmap.RawFormat);
                isMatch = IsImageMatch(0);
            }

            if (isMatch == false)
            {
                return new MatchResult();
            }

            return new MatchResult(isMatch, _axMatch.MatchedScore, _axMatch.MatchedAngle);
        }

        public double Match(string filePath)
        {
            var bitmap = LoadImage(filePath);
            LoadMatchBitmap(bitmap);

            SetMatchRoiControls(0, 0, bitmap.Width, bitmap.Height, bitmap.RawFormat);
            bool isMatch = IsImageMatch(0);

            return isMatch ? _axMatch.MatchedScore : double.NaN;
        }

        public double Match(string sourceFilePath, string targetFilePath)
        {
            Learn(sourceFilePath);
            return Match(targetFilePath);
        }

        public double MatchScore(string sourceFilePath, string targetFilePath)
        {
            var sourceBitmap = LoadImage(sourceFilePath);
            LoadLearnBitmap(sourceBitmap);
            SetLearnRoiControls(0, 0, sourceBitmap.Width, sourceBitmap.Height, sourceBitmap.RawFormat);

            var targetBitmap = LoadImage(targetFilePath);
            LoadMatchBitmap(targetBitmap);
            SetMatchRoiControls(0, 0, targetBitmap.Width, targetBitmap.Height, targetBitmap.RawFormat);

            return _axMatch.GetMatchingScore(_learnAxROI.VegaHandle, _matchAxROI.VegaHandle);
            //return _axMatch.GetMatchingScore(_learnAxImage.VegaHandle, _matchAxImage.VegaHandle);
        }

        public bool SaveModel(string filePath)
        {
            return _axMatch.SaveFile(filePath);
        }

        public bool LoadModel(string filePath)
        {
            return _axMatch.LoadFile(filePath);
        }

        #region Helper Methods

        /// <summary>
        /// 將 Bitmap 繪製到畫布上
        /// </summary>
        /// <param name="bitmap"></param>
        private void LoadLearnBitmap(Bitmap bitmap)
        {
            // 鎖定 Image bitmap 影像空間
            System.Drawing.Imaging.BitmapData bitmapData;
            bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Size.Width, bitmap.Size.Height),
                                                        ImageLockMode.ReadOnly,
                                                        bitmap.PixelFormat);

            //取得 Image bitmap 影像起點指標
            IntPtr imgPtr = bitmapData.Scan0;

            // 將 Image bitmap 影像起點指標傳入 C24 中
            // 注意: SetSurfacePtr 指令需要 OVKBase License, 請使用 AxAltairU 元件連接 License
            _learnAxImage.SetSurfacePtr(bitmap.Size.Width, bitmap.Size.Height, (int)imgPtr);

            // 影像複製完成後，解除鎖定 Image bitmap 影像空間
            bitmap.UnlockBits(bitmapData);

            // 綁定來源
            _learnAxROI.ParentHandle = _learnAxImage.VegaHandle;

            // 儲存檔案
            if (_isSaveFile)
            {
                bool result1 = bitmap.RawFormat.Equals(ImageFormat.Png) ?
                    _learnAxImage.SaveFile(@"C:\Users\kentseng\Desktop\test_images\s2.png", TxAxImageType.AX_IMAGE_FILE_TYPE_GREYLEVEL_PNG) :
                    _learnAxImage.SaveFile(@"C:\Users\kentseng\Desktop\test_images\s2.bmp", TxAxImageType.AX_IMAGE_FILE_TYPE_GREYLEVEL_BMP);
            }
        }
        private void LoadMatchBitmap(Bitmap bitmap)
        {
            // 鎖定 Image bitmap 影像空間
            System.Drawing.Imaging.BitmapData bitmapData;
            bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Size.Width, bitmap.Size.Height),
                                                        ImageLockMode.ReadOnly,
                                                        bitmap.PixelFormat);

            //取得 Image bitmap 影像起點指標
            IntPtr imgPtr = bitmapData.Scan0;

            // 將 Image bitmap 影像起點指標傳入 C24 中
            // 注意: SetSurfacePtr 指令需要 OVKBase License, 請使用 AxAltairU 元件連接 License
            _matchAxImage.SetSurfacePtr(bitmap.Size.Width, bitmap.Size.Height, (int)imgPtr);

            // 影像複製完成後，解除鎖定 Image bitmap 影像空間
            bitmap.UnlockBits(bitmapData);

            // 綁定來源
            _matchAxROI.ParentHandle = _matchAxImage.VegaHandle;

            // 儲存檔案
            if (_isSaveFile)
            {
                bool result2 = bitmap.RawFormat.Equals(ImageFormat.Png) ?
                    _matchAxImage.SaveFile(@"C:\Users\kentseng\Desktop\test_images\t2.png", TxAxImageType.AX_IMAGE_FILE_TYPE_GREYLEVEL_PNG) :
                    _matchAxImage.SaveFile(@"C:\Users\kentseng\Desktop\test_images\t2.bmp", TxAxImageType.AX_IMAGE_FILE_TYPE_GREYLEVEL_BMP);
            }
        }

        private void SetLearnRoiControls(int x, int y, int width, int height, ImageFormat imageFormat)
        {
            _learnAxROI.SetPlacement(x, y, width, height);
            _learnAxROI.SkewAngle = 0;

            // 儲存檔案
            if (_isSaveFile)
            {
                bool result1 = imageFormat.Equals(ImageFormat.Png) ?
                    _learnAxROI.SaveFile(@"C:\Users\kentseng\Desktop\test_images\s1.png", TxAxImageType.AX_IMAGE_FILE_TYPE_GREYLEVEL_PNG) :
                    _learnAxROI.SaveFile(@"C:\Users\kentseng\Desktop\test_images\s1.bmp", TxAxImageType.AX_IMAGE_FILE_TYPE_GREYLEVEL_BMP);
            }
        }

        private void Learn()
        {
            _axMatch.SrcImageHandle = _learnAxROI.VegaHandle;
            _axMatch.LearnPattern();
        }

        private void SetMatchRoiControls(int x, int y, int width, int height, ImageFormat imageFormat)
        {
            _matchAxROI.SetPlacement(x, y, width, height);
            _matchAxROI.SkewAngle = 0;

            // 儲存檔案
            if (_isSaveFile)
            {
                bool result1 = imageFormat.Equals(ImageFormat.Png) ?
                    _matchAxROI.SaveFile(@"C:\Users\kentseng\Desktop\test_images\t1.png", TxAxImageType.AX_IMAGE_FILE_TYPE_GREYLEVEL_PNG) :
                    _matchAxROI.SaveFile(@"C:\Users\kentseng\Desktop\test_images\t1.bmp", TxAxImageType.AX_IMAGE_FILE_TYPE_GREYLEVEL_BMP);
            }

            _axMatch.AbsoluteCoord = true;      // 使否使用絕對座標系
            _axMatch.ToleranceAngle = 180f;     // 搜尋偏差角度
            _axMatch.MinScore = 0.0f;           // 降低門檻, 才能顯示目標分數, 由外部門檻來控管
            _axMatch.PositionType = TxAxMatchPositionType.AX_MATCH_POSITION_TYPE_CENTER;    // 搜尋結果座標位置選擇
            _axMatch.OperationMode = TxAxMatchOperationMode.AX_MATCH_OPERATION_MODE_FAST;   // 樣板比對運作模式
        }

        /// <summary>
        /// 判斷 Canvas 圖像是否有符合的特徵
        /// </summary>
        /// <returns></returns>
        private bool IsImageMatch(float minScore = 0)
        {
            // 沒有 Learn 建模
            if (_axMatch.IsLearnPattern == false)
            {
                return false;
            }

            _axMatch.PatternIndex = -1; // 先復原上一次的比對

            _axMatch.DstImageHandle = _matchAxROI.VegaHandle;
            _axMatch.Match();

            if (_axMatch.MatchedScore < minScore)
            {
                return false;
            }

            return true;
        }

        #endregion

        /// <summary>
        /// 彩色圖片轉換為 Format8bppIndexed 灰階圖片
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private Bitmap LoadImage(string filePath)
        {
            var bitmap = new Bitmap(filePath, false);
            if (bitmap.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                return 灰度处理(bitmap);
            }

            return bitmap;
        }

        /// <summary>
        /// 灰度处理(BitmapData类)
        /// </summary>
        /// <returns>输出8位灰度图片</returns>
        /// <remarks>
        /// http://stroke7387.blogspot.com/2014/07/248.html
        /// </remarks>
        private static Bitmap 灰度处理(Bitmap 图像)
        {
            Bitmap bmp = new Bitmap(图像.Width, 图像.Height, PixelFormat.Format8bppIndexed);

            //设定实例BitmapData相关信息
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData data = 图像.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            //锁定bmp到系统内存中
            BitmapData data2 = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            //获取位图中第一个像素数据的地址
            IntPtr ptr = data.Scan0;
            IntPtr ptr2 = data2.Scan0;

            int numBytes = data.Stride * data.Height;
            int numBytes2 = data2.Stride * data2.Height;

            int n2 = data2.Stride - bmp.Width; //// 显示宽度与扫描线宽度的间隙

            byte[] rgbValues = new byte[numBytes];
            byte[] rgbValues2 = new byte[numBytes2];
            //将bmp数据Copy到申明的数组中
            Marshal.Copy(ptr, rgbValues, 0, numBytes);
            Marshal.Copy(ptr2, rgbValues2, 0, numBytes2);

            int n = 0;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width * 3; x += 3)
                {
                    int i = data.Stride * y + x;

                    double value = rgbValues[i + 2] * 0.299 + rgbValues[i + 1] * 0.587 + rgbValues[i] * 0.114; //计算灰度

                    rgbValues2[n] = (byte)value;

                    n++;
                }
                n += n2; //跳过差值
            }

            //将数据Copy到内存指针
            Marshal.Copy(rgbValues, 0, ptr, numBytes);
            Marshal.Copy(rgbValues2, 0, ptr2, numBytes2);

            //// 下面的代码是为了修改生成位图的索引表，从伪彩修改为灰度
            ColorPalette tempPalette;
            using (Bitmap tempBmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
            {
                tempPalette = tempBmp.Palette;
            }
            for (int i = 0; i < 256; i++)
            {
                tempPalette.Entries[i] = Color.FromArgb(i, i, i);
            }

            bmp.Palette = tempPalette;


            //从系统内存解锁bmp
            图像.UnlockBits(data);
            bmp.UnlockBits(data2);

            return bmp;
        }
    }

    internal class MatchResult
    {
        public bool IsMatch { get; set; }
        public double Score { get; set; }
        public double Angle { get; set; }

        public MatchResult() : this(false, double.NaN, double.NaN)
        {
        }

        public MatchResult(bool isMatch, double score, double angle)
        {
            IsMatch = isMatch;
            Score = score;
            Angle = angle;
        }
    }
}
