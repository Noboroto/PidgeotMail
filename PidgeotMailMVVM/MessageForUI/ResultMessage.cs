namespace PidgeotMailMVVM.MessageForUI
{
	public class ResultMessage
	{
		public string Message { get; set; }
		public bool IsEnable { get; set; }
		public ResultMessage(string mess = "", bool enable = false)
		{
			Message = mess;
			IsEnable = enable;
		}
	}
}
