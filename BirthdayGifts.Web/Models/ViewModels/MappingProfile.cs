using AutoMapper;
using BirthdayGifts.Services.DTOs.Employee;
using BirthdayGifts.Services.DTOs.Gift;
using BirthdayGifts.Services.DTOs.VotingSession;
using BirthdayGifts.Services.DTOs.Vote;
using BirthdayGifts.Web.Models.ViewModels.Employee;
using BirthdayGifts.Web.Models.ViewModels.Gift;
using BirthdayGifts.Web.Models.ViewModels.Voting;

namespace BirthdayGifts.Web.Models.ViewModels
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Employee ViewModel mappings
            CreateMap<EmployeeDto, EmployeeViewModel>();
            CreateMap<CreateEmployeeViewModel, CreateEmployeeDto>();
            CreateMap<EmployeeDto, EditEmployeeViewModel>();
            CreateMap<EditEmployeeViewModel, EmployeeUpdateDto>();
            CreateMap<EmployeeFilterViewModel, EmployeeFilterDto>();

            // Gift ViewModel mappings
            CreateMap<GiftDto, GiftViewModel>();
            CreateMap<CreateGiftViewModel, CreateGiftDto>();
            CreateMap<GiftDto, EditGiftViewModel>();
            CreateMap<EditGiftViewModel, GiftUpdateDto>();
            CreateMap<GiftFilterViewModel, GiftFilterDto>();

            // Voting Session ViewModel mappings
            CreateMap<VotingSessionDto, VotingSessionViewModel>()
                .ForMember(dest => dest.VotingSessionId, opt => opt.MapFrom(src => src.VoteSessionId))
                .ForMember(dest => dest.VoteSessionCreatorId, opt => opt.MapFrom(src => src.VoteSessionCreatorId))
                .ForMember(dest => dest.BirthdayPersonId, opt => opt.MapFrom(src => src.BirthdayPersonId))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.StartedAt, opt => opt.MapFrom(src => src.StartedAt))
                .ForMember(dest => dest.EndedAt, opt => opt.MapFrom(src => src.EndedAt))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year));
            CreateMap<CreateVotingSessionViewModel, CreateVotingSessionDto>();

            // Vote ViewModel mappings
            CreateMap<VoteDto, VoteViewModel>();
            CreateMap<CreateVoteViewModel, CreateVoteDto>();
        }
    }
} 