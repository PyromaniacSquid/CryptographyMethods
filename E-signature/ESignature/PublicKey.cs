using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESignature
{
    internal class PublicKey
    {
        public string _ownerName;
        public byte[] publicKeyBlob;
        public byte[] ESignature;

        public PublicKey()
        {
            _ownerName = "";
            publicKeyBlob = new byte[] { };
            ESignature = null;
        }
        public PublicKey(string fname)
        {
            using (BinaryReader br = new BinaryReader(new FileStream(fname, FileMode.Open, FileAccess.Read)))
            {
                int namlen = br.ReadInt32();
                int pklen = br.ReadInt32();
                _ownerName = Encoding.UTF8.GetString(br.ReadBytes(namlen));
                publicKeyBlob = br.ReadBytes(pklen);
                if (br.BaseStream.Position < br.BaseStream.Length)
                    ESignature = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
                else ESignature = null;
            }
        }

        public void Save(string fname)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(fname, FileMode.Create, FileAccess.Write)))
            {
                bw.Write(_ownerName.Length);
                bw.Write(publicKeyBlob.Length);
                bw.Write(Encoding.UTF8.GetBytes(_ownerName));
                bw.Write(publicKeyBlob);
                if (ESignature != null)
                    bw.Write(ESignature);
            }
        }

    }
}
