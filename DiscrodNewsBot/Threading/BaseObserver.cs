using System.Threading;
using DiscordNewsBot.Config;

namespace DiscordNewsBot.Threading
{
    internal abstract class BaseObserver
    {
        protected AutoResetEvent WaitHandler { get; private set; }

        /// <summary>
        /// Service observing thread itself
        /// </summary>
        private readonly Thread _observerThread;

        /// <summary>
        /// Thread timer.
        /// It'll set autresetevent to signaled every n interval.
        /// </summary>
        private readonly Timer _threadTimer;

        protected BaseObserver()
        {
            _observerThread = new Thread(new ThreadStart(ObservingThread));
            WaitHandler = new AutoResetEvent(true);
            _threadTimer = new Timer(Awake, null, 0, SettingsManager.Config.ObserveCheckInterval);
        }

        /// <summary>
        /// Starting observing thread
        /// </summary>
        public void Start()
        {
            _observerThread.Start();
        }

        /// <summary>
        /// Stop observing thread
        /// </summary>
        public void Stop()
        {
            _observerThread.Abort();
            _threadTimer.Dispose();
        }

        /// <summary>
        /// Waking up observing thread to check for new notification
        /// </summary>
        /// <param name="obj"></param>
        public void Awake(object obj = null)
        {
            WaitHandler.Set();
        }

        /// <summary>
        /// Observing thread itself
        /// </summary>
        protected abstract void ObservingThread();
    }
}
