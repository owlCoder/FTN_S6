using Common;

namespace ProcessingModule
{
    public class AlarmProcessor
	{
		public AlarmType GetAlarmForAnalogPoint(double eguValue, IConfigItem configItem)
		{
            if (eguValue >= configItem.HighLimit)
            {
                return AlarmType.HIGH_ALARM;
            }
            else if (eguValue <= configItem.LowLimit)
            {
                return AlarmType.LOW_ALARM;
            }
            return AlarmType.NO_ALARM;
        }
		public AlarmType GetAlarmForDigitalPoint(ushort state, IConfigItem configItem)
		{
            // da li je trenutna vrrednost koja je parametar == sa abonrmalnolm
            // ako jeste vratitit alarm
            // u suprotnom no alarm
            if (state == configItem.AbnormalValue)
            {
                return AlarmType.ABNORMAL_VALUE;
            }
            else
            {
                return AlarmType.NO_ALARM;
            }
        }
    }
}
