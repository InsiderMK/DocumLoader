using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumLoader
{
    internal class ParticipantInfo
    {
        private List<AddressInfo> addresses = new List<AddressInfo>();

        public int CategoryOid { get; set; }
        public int StatusOid { get; set; }
        public int TypeOid { get; set; }

        public string Name { get; set; }
        public string Inn { get; set; }
        public string Ogrn { get; set; }

        public List<AddressInfo> Addresses
        {
            get { return this.addresses; }
        }
    }
}
