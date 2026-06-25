using AutoMapper;
using Questly.Data.Entities;
using Questly.Domain.Entities;
using Questly.Services.DTOs;
using Questly.UI.Models;
using Questly.UI.Models.Question;
using Questly.UI.Models.Survey;

namespace Questly.UI.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity -> DTO
            CreateMap<Survey, TakeSurveyDto>()
                .ForMember(dest => dest.SurveyId, opt => opt.MapFrom(src => src.Id));

            CreateMap<Question, TakeSurveyQuestionDto>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Id));

            CreateMap<QuestionOption, TakeSurveyOptionDto>()
                .ForMember(dest => dest.OptionId, opt => opt.MapFrom(src => src.Id));

            // DTO <-> ViewModel
            CreateMap<TakeSurveyDto, TakeSurveyViewModel>().ReverseMap();
            CreateMap<TakeSurveyQuestionDto, TakeSurveyQuestionViewModel>().ReverseMap();
            CreateMap<TakeSurveyOptionDto, TakeSurveyOptionViewModel>().ReverseMap();

            // Results
            CreateMap<SurveyResultsDto, SurveyResultsViewModel>().ReverseMap();
            CreateMap<QuestionResultDto, QuestionResultViewModel>().ReverseMap();
            CreateMap<OptionResultDto, OptionResultViewModel>().ReverseMap();
        }
    }
}
