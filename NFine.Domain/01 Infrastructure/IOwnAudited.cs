using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain
{
    public interface IOwnAudited
    {
        string F_OwnerId { get; set; }
    }
}
