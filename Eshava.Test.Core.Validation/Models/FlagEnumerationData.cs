using Eshava.Core.Validation.Attributes;

namespace Eshava.Test.Core.Validation.Models
{
    public class FlagEnumerationData
    {
        [Enumeration]
        public Permission Permission { get; set; }
    }
}
