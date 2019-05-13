using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Common.Cryptography
{
    public interface IEncryptionSupport
    {
        string Encrypt(string password);
        string Decrypt(string cipher);
    }
}
