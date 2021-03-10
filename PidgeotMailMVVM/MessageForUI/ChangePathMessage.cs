namespace PidgeotMailMVVM.MessageForUI
{
	public class ChangePathMessage
	{
		public string Path { get; set; }
		public ChangePathMessage (string path)
		{
			Path = path;
		}
	}
}
