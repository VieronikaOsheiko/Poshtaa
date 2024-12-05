using Domain.Category;
using Domain.Users;

namespace Domain.Parcels;

public class Parcel
{
    public ParcelId Id { get; private set; }
    public int TrackNumber { get; private set; }
    public DateTime DateOfShipment { get; private set; }
    public string AddresToCome { get; private set; }
    public string Weight { get; private set; }
    public CategoryId CategoryId { get; private set; }
    public Category.Category? Category { get; private set; }
    public UserId UserId { get; private set; }
    public UserId ReceiverId { get; private set; }
    public User? User { get; private set; }
    
    public Parcel(ParcelId id, int trackNumber, string addresToCome, DateTime dateOfShipment, string weight, UserId userId, UserId receiverId,CategoryId categoryId)
    {
        Id = id;
        TrackNumber = trackNumber;
        DateOfShipment = dateOfShipment;
        AddresToCome = addresToCome;
        Weight = weight;
        UserId = userId;
        ReceiverId = receiverId; 
        CategoryId = categoryId;
    }

    public static Parcel New(ParcelId id, int trackNumber, DateTime dateOfShipment, string addresToCome, string weight, UserId userId,UserId receiverId,CategoryId categoryId)
        => new Parcel(id, trackNumber, addresToCome, dateOfShipment, weight, userId,receiverId,categoryId);
    public void UpdateDetails(int tracknumber , string weight,string addresToCome)
    {
        TrackNumber = tracknumber;
        Weight = weight;
        AddresToCome = addresToCome;
        DateOfShipment = DateTime.UtcNow;
    }
}