using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharityManagmentSystem
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
