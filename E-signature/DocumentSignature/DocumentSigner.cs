using System.Security.Cryptography;

namespace DocumentSignature
{
    public class DocumentSigner
    {
        private byte[] _userName;
        private byte[] _authorName;

        public DocumentSigner()
        {
            _userName = null;
            _authorName = null;
        }
        public DocumentSigner(byte[] authorName)
        {
            _authorName = authorName;
        }
        // REDO 
        public void SetName(byte[] authorName)
        {
            if (_authorName == null)
            {
                _authorName = authorName;
            }
            else
            {
                throw new Exception("User already defined");
            }
        }

    }
}