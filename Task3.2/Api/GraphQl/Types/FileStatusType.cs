using Domain.Models;

namespace Api.GraphQl.Types
{
    public class FileStatusType : EnumType<FileStatus>
    {
        protected override void Configure(IEnumTypeDescriptor<FileStatus> descriptor) //that's very scary thing without proper policies it can be a disaster anyone can do anything...
        {
            descriptor.Name("FileStatus");
            descriptor.Value(FileStatus.Processing).Name("PROCESSING");
            descriptor.Value(FileStatus.Ready).Name("READY");
            descriptor.Value(FileStatus.Failed).Name("FAILED");
        }
    }
}
