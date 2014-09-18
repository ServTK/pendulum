using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pendulum.Wrappers
{
    public class TkProcess
    {
        private Process _process;

        public TkProcess(int processId)
        {
            _process = Process.GetProcessById(processId);
        }
    }
}
