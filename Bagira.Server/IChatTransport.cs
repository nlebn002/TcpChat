using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagira.Server
{
    public interface IChatTransport
    {
        void Start(CancellationToken cancellationToken);
        void Stop();
    }
}
