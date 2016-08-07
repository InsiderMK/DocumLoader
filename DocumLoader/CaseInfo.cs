using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumLoader
{
    /// <summary>
    /// POCO for court case
    /// </summary>
    internal class CaseInfo
    {
        public int Oid { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public int CategoryOid { get; set; }
        public int SubcategoryOid { get; set; }
        public int TypeOid { get; set; }
        public int NatureOid { get; set; }

        public bool IsArchived { get; set; }
        public bool IsDestroyed { get; set; }

        public decimal ClaimSum { get; set; }
    }
}
