using System.ComponentModel;
using AutoMapper;
using DataAccess.Room;
using Dto.Room;
using FluentValidation;
using Helpers.Core;
using Helpers.Core.Extensions;
using MediatR;
using Microsoft.Extensions.Localization;
using Model;

namespace Command.Room;

public class EditRoom
{
    [DisplayName("EditRoom")]
    public record EditRoomForm
    {
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
    }

    [DisplayName("EditRoomCommand")]
    public record Command(long RoomId, EditRoomForm Edit) : IRequest<ResultResponse<RoomView>>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Edit).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Edit.Name)
                    .NotNull()
                    .NotEmpty()
                    .Length(ModelSettings.RoomNameMinLength, ModelSettings.RoomNameMaxLength);


                RuleFor(x => x.Edit.Description)
                    .Must(x => x == null || x.Length <= ModelSettings.DescriptionMaxLength);
            });
        }
    }

    public class Handler : IRequestHandler<Command, ResultResponse<RoomView>>
    {
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;

        public Handler(IMapper mapper, IStringLocalizer<Handler> localizer, IRoomRepository roomRepository)
        {
            _mapper = mapper;
            _localizer = localizer;
            _roomRepository = roomRepository;
        }

        public async Task<ResultResponse<RoomView>> Handle(Command message, CancellationToken ct)
        {
            var edit = message.Edit with
            {
                Name = message.Edit.Name.FirstLetterToUpper(),
                Description = message.Edit.Description?.Trim()
            };

            var get = await _roomRepository.Get(message.RoomId);
            if (get == null)
                return ResultResponse<RoomView>.CreateError(_localizer["Room not found"]);

            if (get.Name != edit.Name)
            {
                var find = await _roomRepository.Find(edit.Name);
                if (find != null)
                    return ResultResponse<RoomView>.CreateError(_localizer["Room already exist"]);
            }

            var update = await _roomRepository.Update(get.Id,
                async model =>
                {
                    var update = model with
                    {
                        Name = edit.Name,
                        Description = edit.Description
                    };
                    return await update.AsTaskResult();
                });

            return new ResultResponse<RoomView>(_mapper.Map<RoomView>(update.New));
        }
    }
}