using GalaSoft.MvvmLight;

namespace PidgeotMail.Lib
{
	public class SenderInfo : ObservableObject
	{
		private string _Status;
		public int ID { get; set; }
		public string To { get; set; }
		public string Status { get => _Status; set => Set(ref _Status, value); }
		public SenderInfo(int i = 0, string t = "", string s = "OK")
		{
			ID = i;
			To = t;
			Status = s;
		}
	}
}
