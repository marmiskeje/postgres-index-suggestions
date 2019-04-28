﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface ISettingPropertiesRepository : IBaseRepository<long, SettingProperty>
    {
        SettingProperty Get(string key);
        TObject GetObject<TObject>(string key);
    }
}
