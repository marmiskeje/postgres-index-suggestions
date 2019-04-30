using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IVirtualIndex
    {
        uint ID { get; set; }
        string Name { get; set; }
    }
}
