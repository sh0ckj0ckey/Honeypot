using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Honeypot.Models
{
    public enum MigrateStepEnum
    {
        Welcome,
        BeforeMigrate,
        ReadingFile,
        Migrating,
        Successful,
        Failed,
    }
}
