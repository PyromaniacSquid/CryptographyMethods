using System.Security.Cryptography;

namespace DocumentSignature
{
    public class DocumentSigner
    {
        private byte[] _userName;
        private byte[] _authorName;

        public DocumentSigner(byte[] authorName)
        {
            _authorName = authorName;
        }

    }
}