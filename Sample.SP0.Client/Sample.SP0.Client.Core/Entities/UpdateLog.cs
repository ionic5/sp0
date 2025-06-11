using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.SP0.Client.Core.Entities
{
    public class UpdateLog
    {
        public RepositoryName RepositoryName;
        public DateTime LastUpdated;

        public UpdateLog()
        {
            RepositoryName = RepositoryName.Default;
        }

        public UpdateLog(RepositoryName repositoryName, DateTime lastUpdated)
        {
            this.RepositoryName = repositoryName;
            this.LastUpdated = lastUpdated;
        }
    }
}
