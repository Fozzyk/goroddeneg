using AutoMapper;
using GorodDeneg.API.DTOs;
using GorodDeneg.API.Models;

namespace GorodDeneg.API.Services;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User
        CreateMap<AppUser, UserDto>()
            .ForMember(d => d.Role, o => o.Ignore()); // role resolved in service

        // Category
        CreateMap<Category, CategoryDto>();

        // Project → List
        CreateMap<Project, ProjectListDto>()
            .ForMember(d => d.CategoryName,  o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.CategoryIcon,  o => o.MapFrom(s => s.Category.Icon))
            .ForMember(d => d.DaysLeft,      o => o.MapFrom(s =>
                s.EndDate.HasValue ? Math.Max(0, (int)(s.EndDate.Value - DateTime.UtcNow).TotalDays) : 0))
            .ForMember(d => d.PercentFunded, o => o.MapFrom(s =>
                s.GoalAmount > 0 ? (int)(s.CollectedAmount * 100 / s.GoalAmount) : 0));

        // Project → Detail
        CreateMap<Project, ProjectDetailDto>()
            .IncludeBase<Project, ProjectListDto>()
            .ForMember(d => d.AuthorName,   o => o.MapFrom(s => s.Author.FirstName + " " + s.Author.LastName))
            .ForMember(d => d.AuthorAvatar, o => o.MapFrom(s => s.Author.AvatarUrl))
            .ForMember(d => d.AuthorBio,    o => o.MapFrom(s => s.Author.Bio));

        // Reward
        CreateMap<Reward, RewardDto>();

        // Media
        CreateMap<ProjectMedia, ProjectMediaDto>();

        // ProjectUpdate
        CreateMap<ProjectUpdate, ProjectUpdateDto>();

        // Transaction
        CreateMap<Transaction, TransactionDto>();

        // Pledge
        CreateMap<Pledge, PledgeDto>()
            .ForMember(d => d.ProjectTitle, o => o.MapFrom(s => s.Project.Title))
            .ForMember(d => d.RewardTitle,  o => o.MapFrom(s => s.Reward != null ? s.Reward.Title : null));

        // Admin project
        CreateMap<Project, AdminProjectDto>()
            .ForMember(d => d.AuthorEmail,   o => o.MapFrom(s => s.Author.Email))
            .ForMember(d => d.CategoryName,  o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.PercentFunded, o => o.MapFrom(s =>
                s.GoalAmount > 0 ? (int)(s.CollectedAmount * 100 / s.GoalAmount) : 0));
    }
}
