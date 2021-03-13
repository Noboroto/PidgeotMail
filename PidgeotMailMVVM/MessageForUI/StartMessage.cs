namespace PidgeotMail.MessageForUI
{
	public class StartMessage
	{
		public View CurrentView { get; set; }
		public StartMessage(View v)
		{
			CurrentView = v;
		}

		public enum View
		{
			Login,
			ChooseDraft,
			ChooseSource,
			Attachments,
			Result
		}
	}
}
