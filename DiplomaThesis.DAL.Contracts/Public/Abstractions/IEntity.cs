using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public interface IEntity<TKey>
    {
        TKey ID { get; set; }
    }
}
