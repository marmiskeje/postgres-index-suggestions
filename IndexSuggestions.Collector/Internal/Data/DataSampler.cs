﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal abstract class DataSampler<TData>
    {
        private readonly Dictionary<string, TData> samples = new Dictionary<string, TData>();
        protected abstract string CreateKey(TData data);

        protected abstract void ApplySampling(TData cumulativeData, TData newSample);

        public void AddSample(TData data)
        {
            var key = CreateKey(data);
            bool doUpdate = true;
            lock (samples)
            {
                if (!samples.ContainsKey(key))
                {
                    samples.Add(key, data);
                    doUpdate = false;
                }
            }
            if (doUpdate)
            {
                lock (String.Intern(key))
                {
                    ApplySampling(samples[key], data);
                }
            }
        }

        public IReadOnlyCollection<TData> ProvideSamples()
        {
            return samples.Values;
        }
    }
}
