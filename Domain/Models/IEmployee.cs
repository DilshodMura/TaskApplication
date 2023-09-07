
namespace Domain.Models
{
    public interface IEmployee
    {
        /// <summary>
        /// Gets or sets employee id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets employee payroll number
        /// </summary>
        public string Payroll_Number { get; set; }

        /// <summary>
        /// Gets or sets employee forename
        /// </summary>
        public string Forenames { get; set; }

        /// <summary>
        /// Gets or sets employee surname
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Gets or sets employee date of birth
        /// </summary>
        public DateTime Date_of_Birth { get; set; }

        /// <summary>
        /// Gets or sets employee telephone
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// Gets or sets employee mobile
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets employee address 
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets employee address 2
        /// </summary>
        public string Address_2 { get; set; }

        /// <summary>
        /// Gets or sets employee postcode
        /// </summary>
        public string Postcode { get; set; }

        /// <summary>
        /// Gets or sets employee home email
        /// </summary>
        public string EMail_Home { get; set; }

        /// <summary>
        /// Gets or sets employee start date
        /// </summary>
        public DateTime Start_Date { get; set; }
    }
}
