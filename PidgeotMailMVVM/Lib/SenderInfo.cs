namespace PidgeotMailMVVM.Lib
{
	public class SenderInfo
	{
		public int ID { get; set; }
		public string To { get; set; }
		public SenderInfo(int i = 0, string t = "")
		{
			ID = i;
			To = t;
		}
	}
}
