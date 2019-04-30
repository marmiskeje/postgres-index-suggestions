using DiplomaThesis.Common.Logging;
using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace DiplomaThesis.Collector
{
    internal class LastProcessedLogEntryEvidence : ILastProcessedLogEntryEvidence
    {
        private readonly object lockObject = new object();
        private readonly ILog log;
        private readonly ISettingPropertiesRepository settingsRepository;
        private DateTime previous = DateTime.MinValue;
        private DateTime last = DateTime.MinValue;
        private SettingProperty lastSetting;

        public LastProcessedLogEntryEvidence(ILog log, ISettingPropertiesRepository settingsRepository)
        {
            this.log = log;
            this.settingsRepository = settingsRepository;
            lastSetting = this.settingsRepository.Get(SettingPropertyKeys.LAST_PROCESSED_LOG_ENTRY_TIMESTAMP);
            last = lastSetting.DateTimeValue.HasValue ? lastSetting.DateTimeValue.Value : DateTime.MinValue;
        }

        public void PersistCurrentState()
        {
            DateTime dateToSave = DateTime.MinValue;
            lock (lockObject)
            {
                dateToSave = last;
            }
            lastSetting.DateTimeValue = dateToSave;
            settingsRepository.Update(lastSetting);
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
    }
}
