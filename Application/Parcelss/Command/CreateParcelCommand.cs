using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Category;
using Domain.Parcels;
using Domain.Users;
using MediatR;

namespace Application.Parcelss.Command
{
    public record CreateParcelCommand : IRequest<Result<Parcel, ParcelException>>
    {
        public required Guid ParcelId { get; init; }
        public required Guid UserId { get; init; }
        public required Guid ReceiverId { get; init; }
        public required Guid CategoryId { get; init; }
        public required int TrackNumber { get; init; }
        public required DateTime DateOfShipment { get; init; }
        public required string AddresToCome { get; init; }
        public required string Weight { get; init; }
        public required Guid UserIdFromToken { get; init; }
    }

    public class CreateParcelCommandHandler(
        IParcelRepository parcelRepository,
        IUserRepository userRepository)
        : IRequestHandler<CreateParcelCommand, Result<Parcel, ParcelException>>
    {
        public async Task<Result<Parcel, ParcelException>> Handle(CreateParcelCommand request, CancellationToken cancellationToken)
        {
            var userId = new UserId(request.UserId);
            var receiverId = new UserId(request.ReceiverId);
            var categoryId = new CategoryId(request.CategoryId);

            var user = await userRepository.GetById(userId, cancellationToken);

            return await user.Match<Task<Result<Parcel, ParcelException>>>( 
                async f =>
                {
                    var existingParcel = await parcelRepository.GetByWeightAndAdrres(
                        request.Weight,
                        request.AddresToCome,
                        cancellationToken);

                    return await existingParcel.Match(
                        u => Task.FromResult<Result<Parcel, ParcelException>>(new ParcelAlreadyExistsException(u.Id)),
                        async () => await CreateEntity(request.ParcelId, request.TrackNumber, request.DateOfShipment, request.AddresToCome, request.Weight, userId, receiverId,categoryId ,cancellationToken));
                },
                () => Task.FromResult<Result<Parcel, ParcelException>>(new UserParcelNotFoundException(userId)));
        }

        private async Task<Result<Parcel, ParcelException>> CreateEntity(
            Guid parcelId,
            int trackNumber,
            DateTime dateOfShipment,
            string addresToCome,
            string weight,
            UserId userId,
            UserId receiverId,
            CategoryId categoryId,
            CancellationToken cancellationToken)
        {
            try
            {
                var entity = Parcel.New(
                    new ParcelId(parcelId), 
                    trackNumber, 
                    dateOfShipment, 
                    addresToCome, 
                    weight, 
                    userId, 
                    receiverId,
                    categoryId);

                return await parcelRepository.Add(entity, cancellationToken);
            }
            catch (Exception exception)
            {
                return new ParcelUnknownException(ParcelId.Empty(), exception);
            }
        }
    }
}
