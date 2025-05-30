using AutoMapper;
using BirthdayGifts.Models;
using BirthdayGifts.Services.DTOs.Employee;
using BirthdayGifts.Services.DTOs.Gift;
using BirthdayGifts.Services.DTOs.VotingSession;
using BirthdayGifts.Services.DTOs.Vote;
using BirthdayGifts.Repository.Interfaces;

namespace BirthdayGifts.Services.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Employee mappings
            CreateMap<Employee, EmployeeDto>();
            CreateMap<CreateEmployeeDto, Employee>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => SecurityHelper.HashPassword(src.Password)))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth));
            CreateMap<EmployeeUpdateDto, Employee>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<EmployeeFilterDto, EmployeeFilter>();

            // Gift mappings
            CreateMap<Gift, GiftDto>(); 
            CreateMap<GiftDto, Gift>();
            CreateMap<CreateGiftDto, Gift>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));
            CreateMap<GiftUpdateDto, Gift>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<GiftFilterDto, GiftFilter>();

            // VotingSession mappings
            CreateMap<VotingSession, VotingSessionDto>()
                .ForMember(dest => dest.VoteSessionId, opt => opt.MapFrom(src => src.VotingSessionId));
            CreateMap<CreateVotingSessionDto, VotingSession>()
                .ForMember(dest => dest.VotingSessionId, opt => opt.Ignore())
                .ForMember(dest => dest.VoteSessionCreatorId, opt => opt.MapFrom(src => src.VoteSessionCreatorId))
                .ForMember(dest => dest.BirthdayPersonId, opt => opt.MapFrom(src => src.BirthdayPersonId))
                .ForMember(dest => dest.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.StartedAt, opt => opt.MapFrom(src => src.StartedAt))
                .ForMember(dest => dest.EndedAt, opt => opt.MapFrom(src => src.EndedAt))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year));
            CreateMap<VotingSessionUpdateDto, VotingSession>()
                .ForMember(dest => dest.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<VotingSessionFilterDto, VotingSessionFilter>();

            // Vote mappings
            CreateMap<Vote, VoteDto>();
            CreateMap<CreateVoteDto, Vote>();
            CreateMap<VoteUpdateDto, Vote>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<VoteFilterDto, VoteFilter>();
        }
    }
} 