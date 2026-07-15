using AutoMapper;
using Questly.Data.Entities;
using Questly.Domain.Entities;
using Questly.Services.DTOs.Question;
using Questly.Services.DTOs.Survey;
using Questly.UI.Models;
using Questly.UI.Models.Question;
using Questly.UI.Models.Survey;
using System.Text.Json;

namespace Questly.UI.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity <-> Entity
            CreateMap<SurveyTemplate, Survey>();
            CreateMap<SurveyTemplateQuestion, Question>();
            CreateMap<SurveyTemplateOption, QuestionOption>();

            // Entity -> DTO
            CreateMap<Survey, TakeSurveyDto>().ReverseMap();
            CreateMap<Question, TakeSurveyQuestionDto>().ReverseMap();
            CreateMap<QuestionOption, TakeSurveyOptionDto>().ReverseMap();

            CreateMap<Question, GetQuestionDto>().ReverseMap();
            CreateMap<QuestionOption, GetQuestionOptionDto>().ReverseMap();

            CreateMap<Survey, GetSurveyDto>().ReverseMap();
            CreateMap<Question, GetQuestionDto>().ReverseMap();
            CreateMap<QuestionOption, GetQuestionOptionDto>().ReverseMap();

            CreateMap<Survey, CreateSurveyDto>().ReverseMap();
            CreateMap<Question, CreateSurveyQuestionDto>().ReverseMap();
            CreateMap<QuestionOption, CreateQuestionOptionDto>().ReverseMap();

            CreateMap<Survey, UpdateSurveyDto>().ReverseMap();
            CreateMap<Question, UpdateSurveyQuestionDto>().ReverseMap();
            CreateMap<QuestionOption, UpdateQuestionOptionDto>().ReverseMap();

            CreateMap<SurveyTemplate, SurveyTemplateDto>().ReverseMap();

            CreateMap<Question, TakeQuestionDto>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Text));

            // DTO -> Entity
            CreateMap<CreateQuestionDto, Question>()
                .ForMember(dest => dest.Options,
                    opt => opt.MapFrom(src => src.Options.Where(o => !string.IsNullOrWhiteSpace(o))
                        .Select((o, index) => new QuestionOption
                        { Text = o, DisplayOrder = index + 1 }).ToList()));

            CreateMap<UpdateQuestionDto, Question>()
                .ForMember(dest => dest.Options,
                    opt => opt.MapFrom(src => src.Options.Where(o => !string.IsNullOrWhiteSpace(o))
                        .Select((o, index) => new QuestionOption
                        { Text = o, DisplayOrder = index + 1 }).ToList()));

            CreateMap<TakeQuestionDto, SurveySessionAnswer>()
                .ForMember(dest => dest.SelectedOptionIdsJson,
                           opt => opt.MapFrom(src => JsonSerializer.Serialize(src.SelectedOptionIds)));


            // DTO <-> ViewModel
            CreateMap<TakeSurveyDto, TakeSurveyViewModel>().ReverseMap();
            CreateMap<TakeSurveyQuestionDto, TakeSurveyQuestionViewModel>().ReverseMap();
            CreateMap<TakeSurveyOptionDto, TakeSurveyOptionViewModel>().ReverseMap();

            CreateMap<CreateQuestionDto, CreateQuestionViewModel>().ReverseMap();
            CreateMap<UpdateQuestionDto, UpdateQuestionViewModel>().ReverseMap();

            CreateMap<GetSurveyDto, GetSurveyViewModel>().ReverseMap();
            CreateMap<GetQuestionDto, GetQuestionViewModel>().ReverseMap();
            CreateMap<GetQuestionOptionDto, GetQuestionOptionViewModel>().ReverseMap();

            CreateMap<CreateSurveyDto, CreateSurveyViewModel>().ReverseMap();

            CreateMap<UpdateSurveyDto, UpdateSurveyViewModel>().ReverseMap();

            CreateMap<SurveyResultsDto, SurveyResultsViewModel>().ReverseMap();
            CreateMap<QuestionResultDto, QuestionResultViewModel>().ReverseMap();
            CreateMap<OptionResultDto, OptionResultViewModel>().ReverseMap();

            CreateMap<DashboardDto, DashboardViewModel>().ReverseMap();
            CreateMap<DashboardSurveyItemDto, DashboardSurveyItemViewModel>().ReverseMap();

            CreateMap<SurveyTemplateDto, SurveyTemplateViewModel>().ReverseMap();

            CreateMap<SkipLogicDto, SkipLogicViewModel>().ReverseMap();
            CreateMap<SkipLogicOptionDto, SkipLogicOptionViewModel>().ReverseMap();
            CreateMap<QuestionLookupDto, QuestionLookupViewModel>().ReverseMap();

            CreateMap<TakeQuestionDto, TakeQuestionViewModel>().ReverseMap();

            // DTO <-> DTO
            CreateMap<GetSurveyDto, UpdateSurveyDto>().ReverseMap();
            CreateMap<GetQuestionDto, UpdateQuestionDto>().ReverseMap();
            CreateMap<GetQuestionOptionDto, UpdateQuestionOptionDto>().ReverseMap();
        }
    }
}
