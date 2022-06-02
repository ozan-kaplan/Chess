using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Web.ViewModels
{
    public class LineChartViewModel<Tx, Ty>
    {

        public Tx x { get; set; }

        public Ty y { get; set; }

        public string label { get; set; }
    }

    public class BarChartViewModel<Ty>
    {

        public string id { get; set; }
        public Ty y { get; set; }

        public string label { get; set; }

        public string extensiondata { get; set; }
        public string cursor { get; set; } = "pointer";
    }
}
