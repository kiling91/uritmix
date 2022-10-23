using System.ComponentModel;
using AutoMapper;
using DataAccess.Room;
using Helpers.Core;
using MediatR;
using Microsoft.Extensions.Localization;
using View.Room;

namespace Command.Room;

public class GetRoom
{
    [DisplayName("GetRoomQuery")]
    public record Query(long RoomId) : IRequest<ResultResponse<RoomView>>;

    public class Handler : IRequestHandler<Query, ResultResponse<RoomView>>
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IMapper _mapper;

        public Handler(IStringLocalizer<Handler> localizer, IMapper mapper, IRoomRepository roomRepository)
        {
            _localizer = localizer;
            _mapper = mapper;
            _roomRepository = roomRepository;
        }

        public async Task<ResultResponse<RoomView>> Handle(Query message, CancellationToken ct)
        {
            var room = await _roomRepository.Get(message.RoomId);
            if (room == null)
                return ResultResponse<RoomView>.CreateError(_localizer["Room not found"]);
            return new ResultResponse<RoomView>(_mapper.Map<RoomView>(room));
        }
    }
}