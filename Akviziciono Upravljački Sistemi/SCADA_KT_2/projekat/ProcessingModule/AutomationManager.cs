using Common;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;

namespace ProcessingModule
{
    public class AutomationManager : IAutomationManager, IDisposable
	{
		private Thread automationWorker;
        private AutoResetEvent automationTrigger;
        private IStorage storage;
		private IProcessingManager processingManager;
		private int delayBetweenCommands;
        private IConfiguration configuration;
        public AutomationManager(IStorage storage, IProcessingManager processingManager, AutoResetEvent automationTrigger, IConfiguration configuration)
		{
			this.storage = storage;
			this.processingManager = processingManager;
            this.configuration = configuration;
            this.automationTrigger = automationTrigger;
        }
		private void InitializeAndStartThreads()
		{
			InitializeAutomationWorkerThread();
			StartAutomationWorkerThread();
		}
		private void InitializeAutomationWorkerThread()
		{
			automationWorker = new Thread(AutomationWorker_DoWork);
			automationWorker.Name = "Aumation Thread";
		}
		private void StartAutomationWorkerThread()
		{
			automationWorker.Start();
		}


		private void AutomationWorker_DoWork()
		{
			// konverter jedinica iz inzinjerskih u raw
			EGUConverter konverter = new EGUConverter();

			// samo analogni i digitalni izlazi bez konstanti i alarma
			PointIdentifier L = new PointIdentifier(PointType.ANALOG_OUTPUT, 1000);
			PointIdentifier stop = new PointIdentifier(PointType.DIGITAL_OUTPUT, 2000);
			PointIdentifier v1 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 2002);
			PointIdentifier p1 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 2005);
			PointIdentifier p2 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 2006);

			// lista svih pin point-a
			List<PointIdentifier> lista = new List<PointIdentifier> { L, stop, v1, p1, p2 };

			// promenljiva u kojoj cuvamo vrednost
			int vrednost = 0;

			// IV Automatizacija worka
			while(!disposedValue)
			{
				List<IPoint> points = storage.GetPoints(lista);

				// konverzija analognog izlaza u injzenjersku jedinicu
				// vrednost = (int)konverter.ConvertToEGU(points[0].ConfigItem.ScaleFactor,
				//									   points[0].ConfigItem.Deviation,
				//									   points[0].RawValue);
			 	vrednost = points[0].RawValue;

				// automatizacija: pumpa 1 i pumpa 2 (tabela strana 2)
				if (points[3].RawValue == 0 && points[4].RawValue == 0) // P1 = P2 = 0
				{
					vrednost += 0;
				}
				else if (points[3].RawValue == 0 && points[4].RawValue == 1) // P1 = 0, P2 = 1
                {
                    vrednost += 80;
                }
				else if (points[3].RawValue == 1 && points[4].RawValue == 0) // P1 = 1, P2 = 0
                {
                    vrednost += 160;
                }
				else if (points[3].RawValue == 1 && points[4].RawValue == 1) // P1 = P2 = 1
                {
                    vrednost += 240;
                }

                // Kada je ventil V1 otvoren, i ako je visina vode veća od 2m,
				// bazen se prazni brzinom od 50 l/s.
				// ako se to konvertuje u trenutnu visinu onda je po litrazi to
				// 2 * 1.5 * 2 = 6m3 tj 6000 litara
				if(vrednost >= 6000 && points[2].RawValue == 1) // v1
				{
					vrednost -= 50; // bazen se onda prazni 50l/s OutFlow
				}

				// Kako bi se obezbedilo da se bazen nikada ne prazni i ne puni istovremeno,
				// prekidačem STOP se može
				// upravljati u kom od dva stanja će se dati sistem nalaziti.
				if (points[1].RawValue == 1) // stop prekidac
				{
                    // ugasiti obe pumpe
                    processingManager.ExecuteWriteCommand(points[3].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, p1.Address, 0);
                    processingManager.ExecuteWriteCommand(points[4].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, p2.Address, 0);
                }

				if (points[1].RawValue == 0) // stop prekidac
				{
					// zatvoriti V1
                    processingManager.ExecuteWriteCommand(points[2].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, v1.Address, 0);
                }

				// Kada zapremina vode pređe HighAlarm vrednost,
				// aktivira se sistem za automatsko pražnjenje vode.
				// Pri tome, prekidač STOP prelazi u stanje 1.
				// Pumpa se automatski isključuje, a ventil otvara.
				// Automatsko pražnjenje vode se isključuje ručno.
				// SISTEM ZA AUTOMATSKO PRAZNJENJE VODE
				if (vrednost >= points[0].ConfigItem.HighLimit)
				{
                    // prekidac stop = 1
                    processingManager.ExecuteWriteCommand(points[1].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, stop.Address, 1);

                    // automatski ugasiti pumpu
                    processingManager.ExecuteWriteCommand(points[3].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, p1.Address, 0);
                    processingManager.ExecuteWriteCommand(points[4].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, p2.Address, 0);

                    // otvoriti ventil v1
                    processingManager.ExecuteWriteCommand(points[2].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, v1.Address, 1);
                }
				else if(vrednost <= points[0].ConfigItem.LowLimit)
				{
                    // ispod dozvoljenog nivoa - ukljuciti obe pumpe
                    // prekidac stop = 0
                    processingManager.ExecuteWriteCommand(points[1].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, stop.Address, 0);

                    // automatski ukljuciti pumpu
                    processingManager.ExecuteWriteCommand(points[3].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, p1.Address, 1);
                    processingManager.ExecuteWriteCommand(points[4].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, p2.Address, 1);

                    // zatvoriti ventil v1
                    processingManager.ExecuteWriteCommand(points[2].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, v1.Address, 0);
                }
				else
				{
                    // ako nisu alarmatne vrednosti
                    processingManager.ExecuteWriteCommand(points[0].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, L.Address, vrednost);
                }


                // thread sleep 1 second
                automationTrigger.WaitOne();
			}
		}

		#region IDisposable Support
		private bool disposedValue = false;
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
                    if (automationWorker != null)
                        automationWorker.Abort();
                }
				disposedValue = true;
			}
		}
		public void Dispose()
		{
			Dispose(true);
		}
        public void Start(int delayBetweenCommands)
		{
			this.delayBetweenCommands = delayBetweenCommands*1000;
            InitializeAndStartThreads();
		}
        public void Stop()
		{
			Dispose();
		}
		#endregion
	}
}
