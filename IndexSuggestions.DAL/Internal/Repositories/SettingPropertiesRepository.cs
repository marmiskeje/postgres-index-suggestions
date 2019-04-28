using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace IndexSuggestions.DAL
{
    internal class SettingPropertiesRepository : BaseRepository<long, SettingProperty>, ISettingPropertiesRepository
    {
        public SettingPropertiesRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {

        }

        public SettingProperty Get(string key)
        {
            using (var context = CreateContextFunc())
            {
                return context.SettingProperties.Where(x => x.Key == key).SingleOrDefault();
            }
        }

        public TObject GetObject<TObject>(string key)
        {
            TObject result = default(TObject);
            using (var context = CreateContextFunc())
            {
                var setting = context.SettingProperties.Where(x => x.Key == key).SingleOrDefault();
                if (setting != null && !String.IsNullOrEmpty(setting.StrValue))
                {
                    result = JsonSerializationUtility.Deserialize<TObject>(setting.StrValue);
                }
            }
            return result;
        }
    }
}
