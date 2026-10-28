using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Przelewy
{
    class pacz
    {
        public struct ListaPaczek
        {
            public int ID { set; get; }
            public string Nazwa { set; get; }

            public DateTime Data { set; get; }
            public string Rejestr { set; get; }

            public string Waluta { set; get; }
        }
    }
}
