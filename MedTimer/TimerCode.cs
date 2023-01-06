using Android.Media;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Android.Widget;

namespace MedTimer
{
    public delegate void OutPutFunction(string value);
    
    public static class TimerCode
    {
        private static bool _timerRunned =false;

        private static MediaPlayer _mediaPlayer;

        /// <summary>
        /// Запуск таймера
        /// </summary>
        private static void runPlayer()
        {
            if (_mediaPlayer == null)
            {
                _mediaPlayer = new MediaPlayer();
                _mediaPlayer.SetDataSource("https://www.soundjay.com/buttons/beep-01a.mp3");
                _mediaPlayer.Prepare();
                _mediaPlayer.SetVolume(100, 100);
            }
            _mediaPlayer.Start();
        }

        public static bool RunTimer(TimerConfig timerConfig, OutPutFunction outputFunc)
        {
            if (timerConfig == null) return false;
            _timerRunned = true;
            outputFunc(String.Format($"{timerConfig.LoopCount}:{timerConfig.Interval}"));
            return true;
        }

        /// <summary>
        /// Работа с запущенным таймером
        /// </summary>
        /// <param name="timerConfig"></param>
        /// <param name="outputFunc"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool TimerTick(TimerConfig timerConfig, OutPutFunction outputFunc, string text)
        {
            if (!_timerRunned) return true;
            string[] values = text.Split(':');
            if (values.Length == 2 && int.TryParse(values[1], out int counterInterval) && int.TryParse(values[0], out int counterLoop))
            {
                Task.Delay(1000).Wait();
                int newCounterInterval = counterInterval - 1;
                if (newCounterInterval > 0)
                {
                    outputFunc(String.Format($"{counterLoop}:{newCounterInterval}"));
                }
                else
                {
                    int newCounterLoop = counterLoop - 1;
                    runPlayer();
                    if (newCounterLoop > 0)
                    {
                        outputFunc(String.Format($"{newCounterLoop}:{timerConfig.Interval}"));
                    }
                    else
                    {
                        _timerRunned = false;
                       outputFunc(String.Format($"{0}:{0}"));
                        return true;
                    }
                }
            }

            return false;
        }

    }

    /// <summary>
    /// Объект для хранения настроек таймера
    /// </summary>
    public class TimerConfig
    {
        private int _interval = 60;

        public int Interval
        {
            get { return _interval; }
            set
            {
                if (value < 1) value = 1;
                _interval = value;
            }
        }

        private int _loopCount =1;

        public int LoopCount
        {
            get { return _loopCount; }
            set
            {
                if (value < 1) value = 1;
                _loopCount = value;
            }
        }

        public void SetInterval(string interval)
        {
            if (int.TryParse(interval, out int intervalInt))
            {
                Interval = intervalInt;
            }
            else
            {
                Interval = 60;
            }
        }

        public void SetLoopCount(string loopCount)
        {
            if (int.TryParse(loopCount, out int loopCountInt))
            {
                LoopCount = loopCountInt;
            }
            else
            {
                LoopCount = 1;
            }
        }

    }
}