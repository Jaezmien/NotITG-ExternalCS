using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

using NotITG.External;


namespace NotITG_External_Debug
{
    public partial class Form1 : Form
    {

        public static NotITGHandler nITG = null;
        public bool HasNotITGInitialized = false;
        public BackgroundWorker TryConnectWorker;
        public bool HasNotITG = false;
        public Form1()
        {
            nITG = new NotITGHandler(false);

            // Background worker for reconnecting
            TryConnectWorker = new BackgroundWorker();
            TryConnectWorker.DoWork += TryConnectWorker_DoWork;

            // We dont use any heartbeat timers here because we know if NotITG exited using an event
            TryConnectWorker.RunWorkerAsync();

            // Update
            var update_timer = new Timer();
            update_timer.Enabled = true;
            update_timer.Interval = 20; // 20ms
            update_timer.Tick += Update_timer_Elapsed;
            update_timer.Start();
        }

        private async void TryConnectWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while(!nITG.HasNotITG)
            {
                await Task.Delay(5000);
                ScanNotITG();
            }
        }

        private void Update_timer_Elapsed(object sender, EventArgs e)
        {
            if (!nITG.HasNotITG)
            {
                if(HasNotITG)
                {
                    Debug.WriteLine("NotITG Closed");
                    TryConnectWorker.RunWorkerAsync();
                    HasNotITGInitialized = false;
                    HasNotITG = false;
                    return;
                }
            }
            if (nITG.HasNotITG && !HasNotITG) HasNotITG = true;
            if (!HasNotITGInitialized)
            {
                if(nITG.GetExternal(60) == 0) return;
                
                HasNotITGInitialized = true;
            }

            // TODO: Insert Code here
        }

        private void ScanNotITG()
        {
            
            bool result = nITG.TryScan();
            if (!result)
            {
                Debug.WriteLine("Can't find NotITG...");
            } else
            {
                Debug.WriteLine("NotITG Connected!");
            }
        }
    }
}
