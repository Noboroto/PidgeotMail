namespace PidgeotMail.Lib
{
	public class SenderInfo
	{
		public int ID { get; set; }
		public string To { get; set; }
		public string Status { get; set; }
		public SenderInfo(int i = 0, string t = "", string s = "OK")
		{
			ID = i;
			To = t;
			Status = s;
		}
	}
}
