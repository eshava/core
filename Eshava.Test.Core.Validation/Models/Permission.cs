using System;

namespace Eshava.Test.Core.Validation.Models
{
    [Flags]
    public enum Permission
    {
        None = 0,
        Read = 1,
        Write = 2,
        Delete = 4
    }
}
