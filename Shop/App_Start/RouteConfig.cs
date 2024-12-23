using System.Web.Mvc;
using System.Web.Routing;

namespace Shop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Bỏ qua yêu cầu BotDetect Captcha
            routes.IgnoreRoute("{*botdetect}",
              new { botdetect = @"(.*)BotDetectCaptcha\.ashx" });

            // Tuyến trang chủ
            routes.MapRoute(
                name: "Home Page",
                url: "trang-chu",
                defaults: new { controller = "Home", action = "Index" }
            );

            // Tuyến giới thiệu
            routes.MapRoute(
                name: "About Page",
                url: "gioi-thieu",
                defaults: new { controller = "Home", action = "About" }
            );

            // Tuyến liên hệ
            routes.MapRoute(
                name: "Contact Page",
                url: "lien-he",
                defaults: new { controller = "Home", action = "Contact" }
            );

            // Tuyến quảng cáo
            routes.MapRoute(
                name: "Advertisement Page",
                url: "quang-cao",
                defaults: new { controller = "Home", action = "QuangCao" }
            );

            // Chi tiết bài viết
            routes.MapRoute(
                name: "Post Details",
                url: "post/{id}/{postName}",
                defaults: new { controller = "Home", action = "PostDetails", id = UrlParameter.Optional, postName = UrlParameter.Optional }
            );

            // Chi tiết laptop
            routes.MapRoute(
                name: "Laptop Details",
                url: "laptop/{id}/{postName}",
                defaults: new { controller = "Home", action = "Details", id = UrlParameter.Optional, postName = UrlParameter.Optional }
            );

            // Tuyến mặc định
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
