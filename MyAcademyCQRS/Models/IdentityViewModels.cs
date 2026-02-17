using System.ComponentModel.DataAnnotations;

namespace MyAcademyCQRS.Models;

public class AdminLoginViewModel
{
    [Required(ErrorMessage = "Kullanıcı adı gerekli")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Şifre gerekli")]
    public string Password { get; set; }
}

public class AdminRegisterViewModel
{
    [Required(ErrorMessage = "Ad gerekli")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Soyad gerekli")]
    public string Surname { get; set; }

    [Required(ErrorMessage = "Kullanıcı adı gerekli")]
    public string Username { get; set; }

    [Required(ErrorMessage = "E-posta gerekli")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Şifre gerekli")]
    public string Password { get; set; }
}
