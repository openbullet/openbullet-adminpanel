using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBulletAdminPanel
{
    public class User
    {
        public string Key { get; set; }
        public string[] Groups { get; set; }

        public string Grps { get { return string.Join(",", Groups); } }
    }
}
