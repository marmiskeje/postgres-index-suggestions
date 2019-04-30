using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public interface ISettingPropertiesRepository : IBaseRepository<long, SettingProperty>
    {
        SettingProperty Get(string key);
        TObject GetObject<TObject>(string key);
    }
}
