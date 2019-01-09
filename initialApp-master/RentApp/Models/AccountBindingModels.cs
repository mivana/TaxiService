using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using RentApp.Models.Entities;

namespace RentApp.Models
{
    // Models used as parameters to AccountController actions.

    public class AddExternalLoginBindingModel
    {
        [Required]
        [Display(Name = "External access token")]
        public string ExternalAccessToken { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterBindingModel
    {
        [Required]
        [Display(Name = "FullName")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "JMBG")]
        public string JMBG { get; set; }

        [Display(Name = "ContactNumber")]
        public string ContactNumber { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

    }

    public class RegisterExternalBindingModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = "Login provider")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = "Provider key")]
        public string ProviderKey { get; set; }
    }

    public class SetPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterDriverBindingModel
    {
        [Required]
        [Display(Name = "FullName")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "JMBG")]
        public string JMBG { get; set; }

        [Display(Name = "ContactNumber")]
        public string ContactNumber { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [Required]
        [Display(Name = "TaxiNumber")]
        public string TaxiNumber { get; set; }

        [Display(Name = "YearMade")]
        public DateTime YearMade { get; set; }

        [Required]
        [Display(Name = "RegistrationPlate")]
        public string RegistrationPlate { get; set; }

        [Required]
        [Display(Name = "CarType")]
        public string CarType { get; set; }
    }

    public class RideBindingModel
    {
        [Required]
        [Display(Name ="StreetName")]
        
        public string StreetName { get; set; }

        [Required]
        [Display(Name = "Number")]
        public int Number { get; set; }

        [Required]
        [Display(Name = "Town")]
        public string Town { get; set; }

        [Required]
        [Display(Name = "AreaCode")]
        [DataType(DataType.PostalCode)]
        public int AreaCode { get; set; }

        [Required]
        [Display(Name = "CarType")]
        public string CarType { get; set; }

        [Display(Name = "Driver")]
        public AppUser Driver { get; set; }
    }

    public class CommentRideBindingModel
    {
        [Required]
        [Display(Name = "Ride")]
        public Ride Ride { get; set; }

        [Required]
        [Display]
        public Comment UserComment { get; set; }

    }

    public class FinishRideBindingModel
    {
        [Required]
        [Display(Name = "IsGood")]
        public bool IsGood { get; set; }

        [Required]
        [Display(Name = "FinishRide")]
        public Ride FinishRide { get; set; }
        
        [Display(Name = "Content")]
        public string Content { get; set; }

        [Display(Name = "DStreetName")]
        public string DStreetName { get; set; }

        [Display(Name = "DNumber")]
        public int DNumber { get; set; }

        [Display(Name = "DTown")]
        public string DTown { get; set; }

        [Display(Name = "DAreaCode")]
        [DataType(DataType.PostalCode)]
        public int DAreaCode { get; set; }

        [Display(Name = "Price")]
        public double Price { get; set; }

    }



}
