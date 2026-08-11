using Application.Dtos.Position;
using Application.Dtos.Response;
using MediatR;


namespace Application.UseCases.Position.Commands.Create
{
    public class CreatePositionCommand : IRequest<BaseApiResponse>
    {
        public CreatePositionDto Dto { get; set; }
        public Guid OwnerId { get; set; }
    }
}
