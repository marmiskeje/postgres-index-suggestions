using IndexSuggestions.Common.Logging;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace IndexSuggestions.Collector
{
    internal class LastProcessedLogEntryEvidence : ILastProcessedLogEntryEvidence
    {
        private readonly object lockObject = new object();
        private readonly Timer timer;
        private readonly ILog log;
        private readonly ISettingPropertiesRepository settingsRepository;
        private DateTime previous = DateTime.MinValue;
        private DateTime last = DateTime.MinValue;
        private SettingProperty lastSetting;
        private bool isDisposed = false;

        public LastProcessedLogEntryEvidence(ILog log, ISettingPropertiesRepository settingsRepository)
        {
            this.log = log;
            this.settingsRepository = settingsRepository;
            lastSetting = this.settingsRepository.Get(SettingPropertyKeys.LAST_PROCESSED_LOG_ENTRY_TIMESTAMP);
            last = lastSetting.DateTimeValue.HasValue ? lastSetting.DateTimeValue.Value : DateTime.MinValue;
            timer = new Timer(60000);
            timer.Elapsed += (x,y) => TimerJob();
            timer.Start();
        }

        private void TimerJob()
        {
            try
            {
                lock (lockObject)
                {
                    if (last != previous)
                    {
                        lastSetting.DateTimeValue = last;
                        settingsRepository.Update(lastSetting); 
                    }
                }
            }
            catch (Exception ex)
            {
                log.Write(ex);
            }
        }

        public DateTime Provide()
        {
            lock (lockObject)
            {
                return last;
            }
        }

        public void Publish(DateTime date)
        {
            lock (lockObject)
            {
                previous = last;
                last = date;
            }
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    timer.Stop();
                    TimerJob();
                }
                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~LastProcessedLogEntryEvidence()
        {
            Dispose(false);
        }
    }
}
