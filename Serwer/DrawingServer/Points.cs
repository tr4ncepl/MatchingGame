using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serwer
{
    class Points
    {
        public int number;
        public string player;


        public override string ToString()
        {
            return string.Format("{0}", player);
        }
    }
}
