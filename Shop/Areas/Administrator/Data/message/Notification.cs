using System;
using System.Web;

namespace Shop.Areas.Administrator.Data.message
{
    public class Notification
    {
        // Kiểm tra nếu Session["Notification"] tồn tại và không null
        public static bool has_flash()
        {
            return HttpContext.Current.Session["Notification"] != null;
        }

        // Thiết lập một thông báo mới vào Session
        public static void set_flash(string mgs, string mgs_type)
        {
            // Tạo một đối tượng ModelNotification
            ModelNotification tb = new ModelNotification
            {
                mgs = mgs,
                mgs_type = mgs_type
            };

            // Lưu đối tượng vào session
            HttpContext.Current.Session["Notification"] = tb;
        }

        // Lấy thông báo từ Session và xóa sau khi sử dụng
        public static ModelNotification get_flash()
        {
            // Kiểm tra nếu Session không chứa đối tượng ModelNotification
            if (HttpContext.Current.Session["Notification"] == null || !(HttpContext.Current.Session["Notification"] is ModelNotification))
            {
                return null; // Trả về null nếu không hợp lệ
            }

            // Lấy đối tượng từ session
            ModelNotification Notifi = (ModelNotification)HttpContext.Current.Session["Notification"];

            // Xóa thông báo sau khi sử dụng
            HttpContext.Current.Session["Notification"] = null;

            return Notifi;
        }
    }

    // Lớp ModelNotification để định nghĩa thông báo
   
}
