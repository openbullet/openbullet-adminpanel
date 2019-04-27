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
        public string[] IPs { get; set; }

        public string GroupsString { get { return Groups == null ? "" : string.Join(",", Groups); } }
        public string IPsString { get { return IPs == null ? "" : string.Join(",", IPs); } }
    }
}
