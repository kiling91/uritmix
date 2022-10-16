using System.ComponentModel;
using AutoMapper;
using DataAccess.Room;
using View.Room;
using FluentValidation;
using Helpers.Core;
using Helpers.Core.Extensions;
using MediatR;
using Microsoft.Extensions.Localization;
using Model;
using Model.Room;

namespace Command.Room;

public class CreateRoom
{
    [DisplayName("CreateRoom")]
    public record CreateRoomForm
    {
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
    }

    [DisplayName("CreateRoomCommand")]
    public record Command(CreateRoomForm Create) : IRequest<ResultResponse<RoomView>>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Create).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Create.Name)
                    .NotNull()
                    .NotEmpty()
                    .Length(ModelSettings.RoomNameMinLength, ModelSettings.RoomNameMaxLength);


                RuleFor(x => x.Create.Description)
                    .Must(x => x == null || x.Length <= ModelSettings.DescriptionMaxLength);
            });
        }
    }

    public class Handler : IRequestHandler<Command, ResultResponse<RoomView>>
    {
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;

        public Handler(IMapper mapper, IRoomRepository roomRepository, IStringLocalizer<Handler> localizer)
        {
            _mapper = mapper;
            _roomRepository = roomRepository;
            _localizer = localizer;
        }

        public async Task<ResultResponse<RoomView>> Handle(Command message, CancellationToken ct)
        {
            var create = message.Create with
            {
                Name = message.Create.Name.FirstLetterToUpper(),
                Description = message.Create.Description?.Trim()
            };

            var find = await _roomRepository.Find(create.Name);
            if (find != null)
                return ResultResponse<RoomView>.CreateError(_localizer["Room already exist"]);


            var result = await _roomRepository.Create(new RoomModel
            {
                Name = create.Name,
                Description = create.Description
            });

            return new ResultResponse<RoomView>(_mapper.Map<RoomView>(result));
        }
    }
}