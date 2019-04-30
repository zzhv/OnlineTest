using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace HOPU.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "代码")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "记住此浏览器?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "学号不得为空！")]
        [Display(Name = "学号")]
        [StringLength(10, ErrorMessage = "学号不能超过10个字符！")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "密码不得为空！")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }

    public class OnlyNumberAttribute : RegularExpressionAttribute
    {
        public OnlyNumberAttribute() : base("[mf]")
        {

        }
        public override string FormatErrorMessage(string name)
        {
            return "性别只能输入m(男)或者f(女)";
        }
    }
    //public class RegisterViewModel
    //{
    //    [Required]
    //    [EmailAddress]
    //    [Display(Name = "电子邮件")]
    //    public string Email { get; set; }

    //    [Required]
    //    [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "密码")]
    //    public string Password { get; set; }

    //    [DataType(DataType.Password)]
    //    [Display(Name = "确认密码")]
    //    [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
    //    public string ConfirmPassword { get; set; }
    //}



    public class RegisterViewModel
    {
        [Required(ErrorMessage = "学号不得为空！")]
        [Display(Name = "学号")]
        [StringLength(10, ErrorMessage = "学号不能超过10个字符！")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "姓名不得为空！")]
        [Display(Name = "姓名")]
        [StringLength(10, ErrorMessage = "你确定你名字有这么长？")]
        public string RealUserName { get; set; }


        [Required(ErrorMessage = "密码不得为空！")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required(ErrorMessage = "确认密码不得为空！")]
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不一致。")]
        public string ConfirmPassword { get; set; }
    }


    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }
    }
}
