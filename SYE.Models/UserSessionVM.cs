using System.Collections.Generic;

namespace SYE.Models
{
    public class UserSessionVM
    {
        public string ProviderId { get; set; }
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public List<string> NavOrder { get; set; }
    }
}
