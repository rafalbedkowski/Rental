using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rental.WPF.Helpers
{
    class SettingsViewModel
    {
        public string CompanyName { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Conditions { get; set; }
        public string LogoUrl { get; set; }
    }
}
