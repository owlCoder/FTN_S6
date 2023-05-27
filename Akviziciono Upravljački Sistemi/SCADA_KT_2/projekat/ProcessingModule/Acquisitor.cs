using Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ProcessingModule
{
    public class Acquisitor : IDisposable
	{
		private AutoResetEvent acquisitionTrigger;
        private IProcessingManager processingManager;
        private Thread acquisitionWorker;
		private IStateUpdater stateUpdater;
		private IConfiguration configuration;
		public Acquisitor(AutoResetEvent acquisitionTrigger, IProcessingManager processingManager, IStateUpdater stateUpdater, IConfiguration configuration)
		{
			this.stateUpdater = stateUpdater;
			this.acquisitionTrigger = acquisitionTrigger;
			this.processingManager = processingManager;
			this.configuration = configuration;
			this.InitializeAcquisitionThread();
			this.StartAcquisitionThread();
		}

		#region Private Methods
		private void InitializeAcquisitionThread()
		{
			this.acquisitionWorker = new Thread(Acquisition_DoWork);
			this.acquisitionWorker.Name = "Acquisition thread";
		}
		private void StartAcquisitionThread()
		{
			acquisitionWorker.Start();
		}
		private void Acquisition_DoWork()
		{
			// I AKVIZICIJA
			// izvlacimo sve redove iz rtucfg
			List<IConfigItem> config = configuration.GetConfigurationItems();

			while(true)
			{
				acquisitionTrigger.WaitOne(); // analogno thread sleep 1000

				// za svaki red u rtucfg
				foreach(IConfigItem item in config)
				{
					item.SecondsPassedSinceLastPoll += 1;
					
					if(item.SecondsPassedSinceLastPoll == item.AcquisitionInterval)
					{
						item.SecondsPassedSinceLastPoll = 0;

						processingManager.ExecuteReadCommand(item, configuration.GetTransactionId(),
															 configuration.UnitAddress, item.StartAddress,
															 item.NumberOfRegisters);
                    }
				}
			}
        }

        #endregion Private Methods
        public void Dispose()
		{
			acquisitionWorker.Abort();
        }
	}
}