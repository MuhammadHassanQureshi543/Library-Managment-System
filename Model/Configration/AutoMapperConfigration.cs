using AutoMapper;

namespace LibraryMangamentSystem.Model.Configration
{
    public class AutoMapperConfigration : Profile
    {
        public AutoMapperConfigration()
        {
            // Map UserDTO to User and vice versa
            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.BorrowRecords, opt => opt.MapFrom(src => src.BorrowRecords));

            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.BorrowRecords, opt => opt.MapFrom(src => src.BorrowRecords));

            // Map BorrowRecordDTO to BorrowRecord and vice versa
            CreateMap<BorrowRecordDTO, BorrowRecords>();
            CreateMap<BorrowRecords, BorrowRecordDTO>();

            CreateMap<BooksDTO, Books>();
            CreateMap<Books, BooksDTO>();

            CreateMap<BorrowRequest, BorrowRecords>()
            .ForMember(dest => dest.BorrowId, opt => opt.Ignore()) // Ignore auto-generated fields
            .ForMember(dest => dest.Books, opt => opt.Ignore())      // Ignore navigation properties
            .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}
