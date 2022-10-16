using System.ComponentModel;
using AutoMapper;
using DataAccess.Room;
using View.Room;
using FluentValidation;
using Helpers.Core;
using Helpers.Pagination;
using Helpers.WebApi.Extensions;
using MediatR;

namespace Command.Room;

public class GetRooms
{
    [DisplayName("GetRoomsQuery")]
    public record Query(Paginator Paginator) : IRequest<ResultResponse<PaginatedListViewModel<RoomView>>>;

    public class CommandValidator : AbstractValidator<Query>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Paginator).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Paginator).PaginatorValidate();
            });
        }
    }

    public class Handler : IRequestHandler<Query, ResultResponse<PaginatedListViewModel<RoomView>>>
    {
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;

        public Handler(IMapper mapper, IRoomRepository roomRepository)
        {
            _mapper = mapper;
            _roomRepository = roomRepository;
        }

        public async Task<ResultResponse<PaginatedListViewModel<RoomView>>> Handle(Query request,
            CancellationToken ct)
        {
            var items = await _roomRepository.Items(request.Paginator);
            var result = _mapper.Map<PaginatedListViewModel<RoomView>>(items);
            return new ResultResponse<PaginatedListViewModel<RoomView>>(result);
        }
    }
}