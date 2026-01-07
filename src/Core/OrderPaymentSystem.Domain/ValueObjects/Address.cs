using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Domain.ValueObjects;

public sealed class Address
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string ZipCode { get; private set; }
    public string Country { get; private set; }

    private Address() { }

    private Address(string street, string city, string state, string zipCode, string country)
    {
        Street = street;
        City = city;
        ZipCode = zipCode;
        Country = country;
    }

    public static DataResult<Address> TryCreate(string street, string city, string state, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street)) return DataResult<Address>.Failure(1001, "Street cannot be empty.");
        if (string.IsNullOrWhiteSpace(city)) return DataResult<Address>.Failure(1002, "City cannot be empty.");
        if (string.IsNullOrWhiteSpace(postalCode)) return DataResult<Address>.Failure(1003, "PostalCode cannot be empty.");
        if (string.IsNullOrWhiteSpace(country)) return DataResult<Address>.Failure(1004, "Country cannot be empty.");

        return DataResult<Address>.Success(new Address(street, city, state, postalCode, country));
    }

    protected bool Equals(Address other)
    {
        return Street == other.Street && City == other.City && ZipCode == other.ZipCode && Country == other.Country;
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Address)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, City, ZipCode, Country);
    }
}