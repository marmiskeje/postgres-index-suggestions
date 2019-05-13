using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public interface ISettingPropertiesRepository : IBaseRepository<long, SettingProperty>
    {
        SettingProperty Get(string key, bool useCache = false);
        TObject GetObject<TObject>(string key, bool useCache = false) where TObject : class;
        void SetObject<TObject>(string key, TObject data) where TObject : class;
    }
}
