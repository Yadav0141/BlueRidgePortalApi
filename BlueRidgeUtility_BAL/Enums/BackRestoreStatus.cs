using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_BAL.Enums
{
    public enum BackRestoreStatus
    {
        InProgress=1,
        Completed=2,
        Errored=3,
        Copying=4,
        Aborted=5,
        Backup=6,
        Restore=7
    }
}
