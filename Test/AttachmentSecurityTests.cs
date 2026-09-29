using Controller;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Test;

public sealed class AttachmentSecurityTests
{
    [Fact]
    public async Task ExecutableFileIsRejected()
    {
        var file = CreateFile([0x4D, 0x5A, 0x90, 0x00], "malware.exe", "application/octet-stream");

        var isSafe = await UploadedFileSecurity.IsSafeAttachmentAsync(file, CancellationToken.None);

        Assert.False(isSafe);
    }

    [Fact]
    public async Task ExecutableDisguisedAsPdfIsRejected()
    {
        var file = CreateFile([0x4D, 0x5A, 0x90, 0x00], "invoice.pdf", "application/pdf");

        var isSafe = await UploadedFileSecurity.IsSafeAttachmentAsync(file, CancellationToken.None);

        Assert.False(isSafe);
    }

    [Fact]
    public async Task PdfWithValidSignatureIsAccepted()
    {
        var file = CreateFile("%PDF-1.7\n"u8.ToArray(), "document.pdf", "application/pdf");

        var isSafe = await UploadedFileSecurity.IsSafeAttachmentAsync(file, CancellationToken.None);

        Assert.True(isSafe);
    }

    [Fact]
    public async Task Utf8TextFileIsAccepted()
    {
        var file = CreateFile("Ticket notes"u8.ToArray(), "notes.txt", "text/plain");

        var isSafe = await UploadedFileSecurity.IsSafeAttachmentAsync(file, CancellationToken.None);

        Assert.True(isSafe);
    }

    private static FormFile CreateFile(byte[] contents, string fileName, string contentType)
    {
        var stream = new MemoryStream(contents);
        return new FormFile(stream, 0, contents.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
}
