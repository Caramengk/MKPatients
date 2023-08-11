using Microsoft.AspNetCore.Mvc;
using MKClassLibrary;
using System.ComponentModel.DataAnnotations;

namespace MKPatients.Models
{
    [ModelMetadataType(typeof(PatientMetadata))]
    public partial class Patient : IValidatableObject
    {
       
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            PatientsContext _context = new PatientsContext();

            FirstName = FirstName?.Trim();
            LastName = LastName?.Trim();
            Address = Address?.Trim();
            City = City?.Trim();
            Gender = Gender?.Trim();

   
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                 yield return new ValidationResult("FirstName is required", new[] { "FirstName" });
            }
               
            if (string.IsNullOrWhiteSpace(LastName))
            {
                yield return new ValidationResult("LastName is required", new[] { "LastName" });
            }
               
            if (string.IsNullOrWhiteSpace(Gender))
            {
                yield return new ValidationResult("Gender is required", new[] { "Gender" });
            }



            FirstName = MKValidations.MKCapitalize(FirstName);
            LastName = MKValidations.MKCapitalize(LastName);
            Address = MKValidations.MKCapitalize(Address);
            City = MKValidations.MKCapitalize(City);
            Gender = MKValidations.MKCapitalize(Gender);

            if (!string.IsNullOrEmpty(Ohip))
            {
                Ohip = Ohip.ToUpper();
               
            }

          
            if (!string.IsNullOrEmpty(HomePhone))
            {
                
                var digitOnlyPhone = MKValidations.MKExtractDigits(HomePhone);
                if (digitOnlyPhone.Length == 10)
                {
                    HomePhone = $"{digitOnlyPhone.Substring(0, 3)}-{digitOnlyPhone.Substring(3, 3)}-{digitOnlyPhone.Substring(6)}";
                }
                else
                {
                    yield return new ValidationResult("HomePhone must contain exactly 10 digits", new[] { "HomePhone" });
                }
            }



            if (MKValidations.MKPostalCodeValidation(PostalCode))
            {  
                PostalCode = MKValidations.MKPostalCodeFormat(PostalCode);
            }

            if (Deceased)
            {
                if (DateOfDeath == null)
                    yield return new ValidationResult("Date Of Death is required when Deceased is true", new[] { "DateOfDeath" });
            }
            else
            {
                if (DateOfDeath != null)
                    yield return new ValidationResult("Date Of Death should be null when Deceased is false", new[] { "DateOfDeath" });
            }

            //Province province = null;

            if (!string.IsNullOrEmpty(ProvinceCode))
            {
                ProvinceCode = ProvinceCode.ToUpper();
             
                    var province = _context.Provinces
                  .FirstOrDefault(p => p.ProvinceCode == ProvinceCode.ToUpper());

                    if (province == null)
                        yield return new ValidationResult("ProvinceCode not found", new[] { "ProvinceCode" });

            }





        }
        public class PatientMetadata
        {
            public int PatientId { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; } = null!;

            [Required]
            [Display(Name = "Last Name")]

            public string LastName { get; set; } = null!;

            [Display(Name = "Street Address")]
            public string? Address { get; set; }

            public string? City { get; set; }

            [Display(Name = "Postal Code")]
            public string? PostalCode { get; set; }

            [Display(Name = "OHIP")]
            [Remote("ValidateOHIPPattern", "MKPatient")]
            public string? Ohip { get; set; }

            [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
            [Display(Name = "Date of Birth")]
            [Remote("BirthDateNotFuture", "MKPatient")]
            public DateTime? DateOfBirth { get; set; }

            public bool Deceased { get; set; }

            [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
            [Display(Name = "Date of Death")]
            [Remote("DeathDateNotFuture", "MKPatient")]
            public DateTime? DateOfDeath { get; set; }

            [Display(Name = "Home Phone")]
            public string? HomePhone { get; set; }

            [Required]
            [Remote("GenderValid", "MKPatient")]
            public string? Gender { get; set; }

            [Display(Name = "Province Code")]
            //[Remote("ProvinceCodeValid", "MKPatient")]
            public string? ProvinceCode { get; set; }
            public string? CountryCode { get; set; }
        }
    }
}

    