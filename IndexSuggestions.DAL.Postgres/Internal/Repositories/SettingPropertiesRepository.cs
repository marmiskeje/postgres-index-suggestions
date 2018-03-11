using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace IndexSuggestions.DAL.Postgres
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
    }
}
