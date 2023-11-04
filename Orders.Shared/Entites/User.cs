using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Orders.Shared.Enums;

namespace Orders.Shared.Entites
{
    public class User : IdentityUser
    {
        [Display(Name = "Documento")]
        [MaxLength(20, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Document { get; set; } = null!;

        [Display(Name = "Nombres")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Apellidos")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Dirección")]
        [MaxLength(200, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Address { get; set; } = null!;

        [Display(Name = "Foto")]
        public string? Photo { get; set; }

        [Display(Name = "Tipo de usuario")]
        public UserType UserType { get; set; }

        public City City { get; set; } = null!;

        [Display(Name = "Ciudad")]
        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una {0}.")]
        public int CityId { get; set; }

        [Display(Name = "Usuario")]
        public string FullName => $"{FirstName} {LastName}";

        public ICollection<TemporalOrder>? TemporalOrders { get; set; }

        public ICollection<Order>? Orders { get; set; }

        [Display(Name = "Dirección")]
        public string FullAddress
        {
            get
            {
                var fullAddress = Address;
                if (City != null && City!.Name != null) fullAddress += $", {City.Name}";
                if (City != null && City!.State != null && City!.State!.Name != null) fullAddress += $", {City.State.Name}";
                if (City != null && City!.State != null && City!.State!.Country != null && City!.State!.Country!.Name != null) fullAddress += $", {City.State.Country.Name}";
                return fullAddress;
            }
        }
    }
}