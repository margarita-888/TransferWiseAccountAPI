using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferWiseClient
{
    public class ProfileDTO
    {
        [JsonProperty("id")]
        public int Id { get; set; }   // Profile Id

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("details")]
        public ProfileDetails ProfileDetails { get; set; }
    }

    public class ProfileDetails
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("registrationNumber")]
        public string RegistrationNumber { get; set; }

        [JsonProperty("acn")]
        public string ACN { get; set; }

        [JsonProperty("abn")]
        public string ABN { get; set; }

        [JsonProperty("arbn")]
        public string ARBN { get; set; }

        [JsonProperty("companyType")]
        public string CompanyType { get; set; }

        [JsonProperty("companyRole")]
        public string CompanyRole { get; set; }

        [JsonProperty("descriptionOfBusiness")]
        public string DescriptionOfBusiness { get; set; }

        [JsonProperty("primaryAddress")]
        public int PrimaryAddress { get; set; }

        [JsonProperty("webpage")]
        public string Webpage { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("dateOfBirth")]
        public string DateOfBirth { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("occupation")]
        public string Occupation { get; set; }
    }
}