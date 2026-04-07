namespace TodoPortal.Application.DTOs;

public sealed record AddressDto(
    string Street,
    string Suite,
    string City,
    string Zipcode,
    GeoDto Geo);
