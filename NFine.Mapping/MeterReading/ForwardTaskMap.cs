using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFine.Domain.Entity.MeterReading;

namespace NFine.Mapping.MeterReading
{
    public class ForwardTaskMap : EntityTypeConfiguration<ForwardTaskEntity>
    {
        public ForwardTaskMap()
        {
            this.ToTable("Mer_ForwardTask");
            this.HasKey(t => t.F_Id);
        }
    }
}