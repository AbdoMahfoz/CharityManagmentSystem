using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charity_Managment_System
{
    public enum DataAccessMode { Connected, Disconnected };
    public class CMS
    {
        readonly IDBLayer dbLayer;
        public CMS(DataAccessMode mode)
        {
            if(mode == DataAccessMode.Connected)
            {
                dbLayer = new DBConnectedMode();
            }
            else
            {
                dbLayer = new DBDisconnectedMode();
            }
        }
    }
}
