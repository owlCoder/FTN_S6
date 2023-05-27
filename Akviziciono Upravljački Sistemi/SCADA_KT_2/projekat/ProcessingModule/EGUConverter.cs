using System;

namespace ProcessingModule
{
	// III EGU Konverter
    public class EGUConverter
	{
		public double ConvertToEGU(double scalingFactor, double deviation, ushort rawValue)
		{
			return (scalingFactor * deviation) / rawValue;
		}
		public ushort ConvertToRaw(double scalingFactor, double deviation, double eguValue)
        {
			return (ushort)((eguValue - deviation) / scalingFactor);
		}
	}
}
