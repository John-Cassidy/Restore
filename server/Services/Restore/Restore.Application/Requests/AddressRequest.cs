namespace Restore.Application.Requests;

public class AddressRequest
{
    public string FullName { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string Country { get; set; }

    public AddressRequest(string fullName, string address1, string address2, string city, string state, string zip, string country)
    {
        FullName = fullName;
        Address1 = address1;
        Address2 = address2;
        City = city;
        State = state;
        Zip = zip;
        Country = country;
    }
}
