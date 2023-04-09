﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(FizickaLicaServer)))
            {
                host.Open();
                Console.WriteLine("Servis je uspesno pokrenut ");
                Console.ReadKey();
                host.Close();
            }
        }
    }
}
