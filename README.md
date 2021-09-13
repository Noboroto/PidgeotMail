# PidgeotMail

## Giới thiệu sơ lược phần mềm

Phần mềm hỗ trợ những người dùng Gmail có thể tùy chỉnh thông tin trong mail phù hợp với người nhận. Phần mềm sẽ sử dụng mẫu được người dùng tạo sẵn được lưu trong hộp thư nháp của Gmail (không bao gồm danh sách người nhận, Cc, Bcc; có thể bao gồm cả tệp đính kèm). Phần mềm dùng danh sách được nhập liệu từ Google Spreedsheet hoặc Microsoft Excel, hỗ trợ Cc, Bcc cho từng người nhận khác nhau.

Ngoài ra, phần mềm còn hỗ trợ các thầy cô cắt 1 file bảng điểm duy nhất của 1 lớp gửi theo danh sách với thứ tự đã định, hoặc gửi 1 thư mục file đã được đặt tên theo thứ tự định sẵn trong Sheet danh sách.

Do chưa có phương pháp quản lý người dùng nên hiện tại mình sử dụng tính năng thử nghiệm của Google, và bạn sẽ nhận được cảnh báo **Google chưa xác minh ứng dụng này**, nhưng hãy yên tâm vì trong app không hề có virus và cũng không sử dụng những thông tin nhạy cảm của bạn

## Các quyền cần được cấp để sử dụng app

- Đọc, soạn, gửi và xóa vĩnh viễn tất cả email của bạn khỏi Gmail
- Xem cài đặt thông báo email của bạn
- Xem, chỉnh sửa, tạo và xóa tất cả bảng tính của bạn trên Google Trang tính

**LƯU Ý**: đối với mail cá nhân, mỗi ngày bị giới hạn gửi 500 mail cho 500 người (kể cả bằng phần mềm này)

## Thông tin kĩ thuật

Phiên bản .NET Framework được sử dụng: 4.6

Các thư viện hỗ trợ:
- [AutoUpdate.NET](https://github.com/ravibpatel/AutoUpdater.NET): hỗ trợ cập nhật tự động
- [Material Design In XAML Toolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit): hỗ trợ giao diện phần mềm
- [Gmail API](https://developers.google.com/gmail/api): lấy danh sách mail nháp và gửi mail
- [MimeKit](https://github.com/jstedfast/MimeKit): tùy chỉnh mail
- [Epplus](https://github.com/EPPlusSoftware/EPPlus): nạp file .xlxs vào phần mềm
- [Google Sheet API](https://developers.google.com/sheets/api): nạp dữ liệu từ Google Sheet vào phần mềm

## Liên hệ

Email: thanhtuvo135@gmail.com

## Một số hình ảnh của phần mềm

![LoginScreen](https://user-images.githubusercontent.com/48942146/133089500-94fc24a4-46f1-40b2-9ff5-8e7980e995ed.png)
![Permission](https://user-images.githubusercontent.com/48942146/133089557-f18563da-480a-4ea7-b45c-932c678920e7.png)
![SelectDraft](https://user-images.githubusercontent.com/48942146/133089731-82bd34a9-81db-431a-ba28-bee527c722ba.png)
![SelectSheet](https://user-images.githubusercontent.com/48942146/133089908-e24d3949-3708-438a-9c98-3697fc56b6f8.png)

