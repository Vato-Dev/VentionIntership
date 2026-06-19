using LibraryManager.Application.DTO_s.Requests;
using LibraryManager.Domain.Models;
using Mapster;

namespace LibraryManager.Application.Mappings
{
    public static class MapingConfigurationExtension
    {

        public static void RegisterAll()
        {
            RegisterBookMappings();
            RegisterReaderMappings();
        }
        public static void RegisterBookMappings()
        {
            TypeAdapterConfig<UpdateBookRequest, Book>.NewConfig()
                .IgnoreNullValues(true)
                .Map(dest =>
                    dest.Title, src =>
                    src.Title, src => !string.IsNullOrEmpty(src.Title))

                .Map(dest =>
                    dest.AuthorName, src =>
                    src.AuthorName, src => !string.IsNullOrEmpty(src.AuthorName));
        }

        public static void RegisterReaderMappings()
        {
            TypeAdapterConfig<UpdateReaderRequest, Reader>.NewConfig()
                .IgnoreNullValues(true)
                .Map(dest =>
                    dest.PersonalNumber, src =>
                    src.PersonalNumber, src => !string.IsNullOrEmpty(src.PersonalNumber))
                .Map(dest =>
                    dest.Name, src =>
                    src.Name, src => !string.IsNullOrEmpty(src.Name))
                .Map(dest =>
                    dest.EmailAddress, src =>
                    src.EmailAddress, src => !string.IsNullOrEmpty(src.EmailAddress));
        }
    }
}
