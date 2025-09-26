using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotThoseLines_P_FUN_SamuelSallaku
{
    public class CSVRecord
    {
        public double Channel1;
        public double Channel2;
        public double Channel3;
        public double Channel4;
        public double Channel5;

        public override string ToString()
        {
            return Channel1.ToString() + '\t' +
                   Channel2.ToString() + '\t' +
                   Channel3.ToString() + '\t' +
                   Channel4.ToString() + '\t' +
                   Channel5.ToString();
        }
    }
}
