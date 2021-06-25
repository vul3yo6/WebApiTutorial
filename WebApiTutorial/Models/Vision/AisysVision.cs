using AxAltairUDrv;
using AxOvkBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AisysWinApp.Models
{
    /// <summary>
    /// 視覺辨識的授權 (光道視覺)
    /// </summary>
    public class AisysVision : IDisposable
    {
        #region Members

        private readonly object _lockObj = new object();
        private AxAltairU axAltairU;
        private AxMMX axMMX;

        #endregion

        #region Events

        #endregion

        #region Public Methods

        public AisysVision()
        {
            if (axAltairU == null)
            {
                axMMX = new AxMMX();

                axAltairU = new AxAltairU();
                axAltairU.OnDevicePlugged += AxAltairU_OnDevicePlugged;
                axAltairU.OnDeviceUnplugged += AxAltairU_OnDeviceUnplugged;
                axAltairU.OnChannelCreated += AxAltairU_OnChannelCreated;
                axAltairU.OnChannelDestroyed += AxAltairU_OnChannelDestroyed;
                axAltairU.WatchDogTimerState = TxAxauWatchDogTimerState.AXAU_WATCH_DOG_TIMER_STATE_ENABLED;
            }

            lock (_lockObj)
            {
                axAltairU.ResetChannel();         // 強制重設 keypro 狀態
                axAltairU.SoftResetChannel();
                axAltairU.QuickCreateChannel();   // 連接License 
            }

            // 幫助用戶判斷是否有 Altair U/Cyclops U 相機經由 AxAltairU 元件連結
            if (axAltairU.IsPortCreated == false)
            {
                throw new Exception("AisysVision connect fail.");
            }
        }

        private void Disconnect()
        {
            if (axAltairU == null)
            {
                return;
            }

            lock (_lockObj)
            {
                axAltairU.WatchDogTimerState = TxAxauWatchDogTimerState.AXAU_WATCH_DOG_TIMER_STATE_DISABLED;
                axAltairU.Freeze();
                axAltairU.DestroyChannel();
            }
        }

        #endregion

        #region Events Receive

        private void AxAltairU_OnDevicePlugged(int NumOfDevices)
        {
            //AddLog(LogTypes.Info, $"AisysVision OnDevicePlugged: QuickCreateChannel, {axAltairU.WatchDogTimerState}", null);

            lock (_lockObj)
            {
                //axAltairU.ResetChannel();         // 強制重設 keypro 狀態
                axAltairU.SoftResetChannel();
                axAltairU.QuickCreateChannel();     // 連接License 
            }
        }

        private void AxAltairU_OnDeviceUnplugged(int NumOfDevices)
        {
            //AddLog(LogTypes.Error, $"AisysVision OnDeviceUnplugged", null);
        }

        private void AxAltairU_OnChannelCreated(int ImageWidth, int ImageHeight)
        {
            //AddLog(LogTypes.Info, $"AisysVision OnChannelCreated: Connect", null);

            lock (_lockObj)
            {
                // 要在 CreateChannel 之後
                axMMX.SupportMP = false;                            // 要改為 false, 設定 NumOfVisionProcessors 才有效果
                int halfQtyOfCPU = axMMX.NumOfProcessors / 2;       // 只用一半的 CPU 效能
                int useQtyOfCPU = halfQtyOfCPU <= 1 ? 1 : halfQtyOfCPU;
                axMMX.NumOfVisionProcessors = useQtyOfCPU;
                //AddLog(LogTypes.Info, $"AisysVision Init: NumOfProcessors: {axMMX.NumOfProcessors}, NumOfVisionProcessors: {useQtyOfCPU}", null);
            }
        }

        private void AxAltairU_OnChannelDestroyed(int ImageWidth, int ImageHeight)
        {
            //AddLog(LogTypes.Error, $"AisysVision OnChannelDestroyed: Trigger Event", null);
        }

        #endregion

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
                return;

            if (disposing)
            {
                // Free any other managed objects here. 
                // 
                Disconnect();
            }

            // Free any unmanaged objects here. 
            // 

            disposed = true;
        }

        ~AisysVision()
        {
            Dispose(false);
        }

        #endregion
    }
}
