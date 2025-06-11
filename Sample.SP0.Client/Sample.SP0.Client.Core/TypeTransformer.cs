using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.SP0.Client.Core
{
    public class TypeTransformer
    {
        public string ConvertToString(RepositoryName repositoryName)
        {
            return repositoryName.ToString();
        }

        public RepositoryName ConvertToRepositoryName(string repositoryName)
        {
            if (Enum.TryParse(repositoryName, out RepositoryName result))
                return result;
            return RepositoryName.Default;
        }

        public int ConvertToNumber(string str)
        {
            return int.Parse(str, NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
        }

        public DateTime ConvertToDateTime(string dateString)
        {
            return DateTime.ParseExact(dateString, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
        }

        public string ConvertToString(DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }
    }
}
