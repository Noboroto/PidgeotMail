using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PidgeotMail.MessageForUI
{
	public class ChangeHTMLContent
	{
		public string content { get; set; }
		public ChangeHTMLContent(string text)
		{
			content = text;
		}
	}
}