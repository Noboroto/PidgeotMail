
using System;

namespace PidgeotMail.Lib
{
	public static class HandleException
	{
		public static string CorrectErrorMessage(Exception e, int loop = 0)
		{
			string result = "Có lỗi không xác định, vui lòng đăng nhập lại hoặc báo lỗi!\n\n" + e.Message;
			if (e is AggregateException)
			{
				string pre = "";
				for (; loop > 0; loop--) pre += "-";
				result = "Có nhiều lỗi: ";
				var ae = (AggregateException)e;
				foreach (var x in ae.InnerExceptions)
				{
					result += "\n" + CorrectErrorMessage(x, loop + 1);
				}
			}
			if (e is MailKit.CommandException)
				if (e.Message.Contains("5.5.2 Syntax error")) result = "Sai định dạng email người nhận hoặc Cc, Bcc";
				else result = "Lỗi câu lệnh, vui lòng thử lại\n\n" + e.Message;
			if (e is MailKit.ProtocolException)
				result = "Lỗi protocol, vul lòng thử lại\n\n" + e.Message;
			if (e is System.IO.IOException)
				result = "Lỗi nhập/xuất, vui lòng thử lại";
			if (e is ObjectDisposedException)
				result = "Enail đã bị huỷ do lỗi, vui lòng thử lại";
			if (e is TimeoutException)
				result = "Kết nối quá lâu, vui lòng kiểu tra mạng!";
			if (e is InvalidOperationException)
				result = "Kết nối không hợp lệ, vui lòng đăng nhập lại";
			if (e is MailKit.ServiceNotConnectedException)
				result = "Lỗi kết nối máy chủ, vui lòng thử lại";
			if (e is OperationCanceledException)
				result = "Bị dừng";
			if (e is MailKit.Security.AuthenticationException)
				result = "Có lỗi xác thực, bạn vui lòng đợi 5 phút rồi hãy thử lại";
			if (e is System.Security.Authentication.AuthenticationException)
				result = "Có lỗi xác thực, bạn vui lòng đợi 5 phút rồi hãy thử lại";
			if (e is MailKit.ServiceNotAuthenticatedException)
				result = "Có lỗi xác thực, bạn vui lòng đợi 5 phút rồi hãy thử lại";
			return result;
		}
	}
}
