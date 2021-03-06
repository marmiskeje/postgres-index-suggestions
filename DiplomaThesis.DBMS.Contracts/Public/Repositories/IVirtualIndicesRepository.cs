﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IVirtualIndicesRepository
    {
        void InitializeEnvironment();
        IVirtualIndex Create(VirtualIndexDefinition definition);
        void DestroyAll();
        long GetVirtualIndexSize(uint indexID);
    }

}
